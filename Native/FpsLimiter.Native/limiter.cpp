#include <Windows.h>
#include <stdint.h>
#include <atomic>
#include "include/TrayFpsProtocol.h"
#include "include/DeadlineStepper.h"

static LARGE_INTEGER g_frequency = {};
static INIT_ONCE g_clockOnce = INIT_ONCE_STATIC_INIT;
static BOOL CALLBACK initClock(PINIT_ONCE, PVOID, PVOID*) { return QueryPerformanceFrequency(&g_frequency); }
struct ThreadTimer { HANDLE handle = nullptr; ~ThreadTimer() { if (handle) CloseHandle(handle); } };
static thread_local ThreadTimer g_timer;
static std::atomic<LONG> g_enabled(0);
static std::atomic<double> g_rate(60.0);
static SRWLOCK g_configLock = SRWLOCK_INIT;
struct PaceState { const void* stream; DeadlineStepper step; };
static PaceState g_states[128] = {};
static SRWLOCK g_statesLock = SRWLOCK_INIT;

static int64_t nowTicks() { LARGE_INTEGER value; QueryPerformanceCounter(&value); return value.QuadPart; }

static bool readConfig(TrayFpsConfig* c) {
    if (!c || c->size != sizeof(TrayFpsConfig) || c->version != TRAYFPS_PROTOCOL_VERSION) return false;
    LONG before, after;
    for (int attempt = 0; attempt < 4; ++attempt) {
        before = InterlockedCompareExchange(&c->sequence, 0, 0);
        if (before & 1) { YieldProcessor(); continue; }
        const LONG enabled = InterlockedCompareExchange(&c->enabled, 0, 0);
        const DWORD heartbeat = static_cast<DWORD>(InterlockedCompareExchange(&c->heartbeatTick, 0, 0));
        const double rate = c->framesPerSecond;
        after = InterlockedCompareExchange(&c->sequence, 0, 0);
        if (before == after && !(after & 1)) {
            AcquireSRWLockExclusive(&g_configLock);
            if (enabled && static_cast<DWORD>(GetTickCount() - heartbeat) < 2500 && rate >= 10.0 && rate <= 1000.0) {
                if (!g_enabled.load(std::memory_order_relaxed) || rate != g_rate.load(std::memory_order_relaxed)) { AcquireSRWLockExclusive(&g_statesLock); ZeroMemory(g_states, sizeof(g_states)); ReleaseSRWLockExclusive(&g_statesLock); }
                g_rate.store(rate, std::memory_order_release); g_enabled.store(1, std::memory_order_release);
            } else { g_enabled.store(0, std::memory_order_release); AcquireSRWLockExclusive(&g_statesLock); ZeroMemory(g_states, sizeof(g_states)); ReleaseSRWLockExclusive(&g_statesLock); }
            ReleaseSRWLockExclusive(&g_configLock);
            return true;
        }
    }
    return false;
}

static void pace(const void* stream) {
    if (!g_enabled.load(std::memory_order_acquire)) return;
    const int64_t now = nowTicks();
    int64_t deadline = 0; bool first = false;
    AcquireSRWLockExclusive(&g_statesLock);
    PaceState* state = nullptr; PaceState* empty = nullptr;
    for (auto& candidate : g_states) {
        if (candidate.stream && candidate.step.started && now - candidate.step.deadline > g_frequency.QuadPart * 2) candidate = {};
        if (candidate.stream == stream) { state = &candidate; break; }
        if (!candidate.stream && !empty) empty = &candidate;
    }
    if (!state) { state = empty ? empty : &g_states[(reinterpret_cast<uintptr_t>(stream) >> 4) % 128]; *state = {}; state->stream = stream; }
    first = !AdvanceDeadline(state->step, now, g_frequency.QuadPart, g_rate.load(std::memory_order_acquire));
    if (!first) deadline = state->step.deadline;
    ReleaseSRWLockExclusive(&g_statesLock);
    if (first) return;
    const int64_t remaining = deadline - nowTicks();
    if (remaining <= 0) {
        AcquireSRWLockExclusive(&g_statesLock);
        for (auto& candidate : g_states) if (candidate.stream == stream) { candidate.step.deadline = nowTicks(); candidate.step.fraction = 0.0; }
        ReleaseSRWLockExclusive(&g_statesLock);
        return;
    }

    // Relative Windows timer units are 100 ns; never pass QPC timestamps as absolute time.
    const int64_t spinTicks = g_frequency.QuadPart / 5000; // bounded 200 us tail
    const int64_t sleepTicks = remaining - spinTicks;
    if (sleepTicks > 0) {
        if (!g_timer.handle) g_timer.handle = CreateWaitableTimerExW(nullptr, nullptr, CREATE_WAITABLE_TIMER_HIGH_RESOLUTION, TIMER_ALL_ACCESS);
        if (!g_timer.handle) g_timer.handle = CreateWaitableTimerW(nullptr, FALSE, nullptr);
        if (g_timer.handle) {
            LARGE_INTEGER due; due.QuadPart = -static_cast<LONGLONG>((sleepTicks * 10000000LL) / g_frequency.QuadPart);
            const BOOL armed = SetWaitableTimer(g_timer.handle, &due, 0, nullptr, nullptr, FALSE);
            const DWORD waited = armed ? WaitForSingleObject(g_timer.handle, INFINITE) : WAIT_FAILED;
            if (waited != WAIT_OBJECT_0) {
                CloseHandle(g_timer.handle);
                g_timer.handle = nullptr;
                const int64_t fallbackTicks = deadline - nowTicks() - spinTicks;
                if (fallbackTicks > g_frequency.QuadPart / 1000) {
                    const int64_t fallbackMs = fallbackTicks * 1000 / g_frequency.QuadPart;
                    if (fallbackMs > 1) Sleep(static_cast<DWORD>(fallbackMs - 1));
                }
            }
        } else {
            const int64_t fallbackTicks = deadline - nowTicks() - spinTicks;
            if (fallbackTicks > g_frequency.QuadPart / 1000) {
                const int64_t fallbackMs = fallbackTicks * 1000 / g_frequency.QuadPart;
                if (fallbackMs > 1) Sleep(static_cast<DWORD>(fallbackMs - 1));
            }
        }
    }
    while (nowTicks() < deadline) YieldProcessor();
}

// Called by the presentation adapter immediately before the real Present.
extern "C" __declspec(dllexport) void TrayFpsUpdateConfig(TrayFpsConfig* config) {
    InitOnceExecuteOnce(&g_clockOnce, initClock, nullptr, nullptr);
    if (!readConfig(config)) {
        g_enabled.store(0, std::memory_order_release);
        AcquireSRWLockExclusive(&g_statesLock); ZeroMemory(g_states, sizeof(g_states)); ReleaseSRWLockExclusive(&g_statesLock);
    }
}
extern "C" __declspec(dllexport) void TrayFpsBeforePresent(const void* stream) {
    pace(stream);
}

extern "C" __declspec(dllexport) void TrayFpsStop() {
    g_enabled.store(0, std::memory_order_release); AcquireSRWLockExclusive(&g_statesLock); ZeroMemory(g_states, sizeof(g_states)); ReleaseSRWLockExclusive(&g_statesLock);
}

BOOL WINAPI DllMain(HINSTANCE, DWORD, LPVOID) { return TRUE; }
