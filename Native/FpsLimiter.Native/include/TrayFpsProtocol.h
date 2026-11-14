#pragma once
#include <Windows.h>
#include <stdint.h>

// Shared across x86/x64. Fixed-width fields, 8-byte alignment, no pointers.
// Mapping name is scoped to the logon session and target PID.
#define TRAYFPS_PROTOCOL_VERSION 1u
typedef struct __declspec(align(8)) TrayFpsConfig {
    uint32_t size;
    uint32_t version;
    volatile LONG sequence;
    volatile LONG enabled;
    uint32_t targetPid;
    volatile LONG heartbeatTick;
    double framesPerSecond;
} TrayFpsConfig;

