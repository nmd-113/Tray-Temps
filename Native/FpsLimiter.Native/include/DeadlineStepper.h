#pragma once
#include <stdint.h>

struct DeadlineStepper {
    int64_t deadline;
    double fraction;
    bool started;
    DeadlineStepper() : deadline(0), fraction(0.0), started(false) {}
};

// Returns true when this frame has a future deadline to wait for. Missed frames
// reset the phase instead of scheduling catch-up frames.
inline bool AdvanceDeadline(DeadlineStepper& s, int64_t now, int64_t frequency, double rate) {
    if (!s.started || now - s.deadline > frequency * 2) {
        s.deadline = now; s.fraction = 0.0; s.started = true; return false;
    }
    const double exact = static_cast<double>(frequency) / rate + s.fraction;
    const int64_t interval = static_cast<int64_t>(exact);
    s.fraction = exact - interval;
    s.deadline += interval;
    if (s.deadline <= now) { s.deadline = now; s.fraction = 0.0; return false; }
    return true;
}
