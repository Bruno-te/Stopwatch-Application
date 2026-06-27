using System;

namespace StopwatchApp.Core
{
    /// <summary>
    /// Represents the current operational state of the stopwatch.
    /// </summary>
    public enum StopwatchState
    {
        /// <summary>The stopwatch has not been started or has been reset.</summary>
        Idle,
        /// <summary>The stopwatch is actively counting elapsed time.</summary>
        Running,
        /// <summary>The stopwatch is temporarily paused but retains elapsed time.</summary>
        Paused,
        /// <summary>The stopwatch has been stopped and retains last recorded time.</summary>
        Stopped
    }

    /// <summary>
    /// Core logic engine for the stopwatch application.
    /// Manages timer state, elapsed time tracking, and all stopwatch operations.
    /// This class is UI-agnostic and fully testable in isolation.
    /// </summary>
    public class StopwatchEngine
    {
        // ── Private fields ──────────────────────────────────────────────────────
        private DateTime _startTime;        // Wall-clock time when (re)started
        private TimeSpan _accumulatedTime;  // Time saved before the last pause
        private StopwatchState _state;

        /// <summary>
        /// Initialises a new <see cref="StopwatchEngine"/> in the <see cref="StopwatchState.Idle"/> state
        /// with elapsed time set to zero.
        /// </summary>
        public StopwatchEngine()
        {
            _accumulatedTime = TimeSpan.Zero;
            _state = StopwatchState.Idle;
        }

        // ── Public properties ────────────────────────────────────────────────────

        /// <summary>Gets the current operational state of the stopwatch.</summary>
        public StopwatchState State => _state;

        /// <summary>
        /// Gets the total elapsed time.
        /// While running, includes live time since last (re)start plus any accumulated time.
        /// While paused/stopped, returns the frozen accumulated time.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get
            {
                // If actively running, add live time to what was accumulated before the last pause
                if (_state == StopwatchState.Running)
                    return _accumulatedTime + (DateTime.UtcNow - _startTime);

                return _accumulatedTime;
            }
        }

        /// <summary>
        /// Returns the elapsed time formatted as <c>hh:mm:ss</c>.
        /// For example, 1 hour 5 minutes 9 seconds → <c>01:05:09</c>.
        /// </summary>
        public string FormattedTime
        {
            get
            {
                int totalSeconds = (int)Math.Floor(ElapsedTime.TotalSeconds);
                (int hours, int minutes, int seconds) = DecomposeSeconds(totalSeconds);
                return FormatHhMmSs(hours, minutes, seconds);
            }
        }

        /// <summary>
        /// Decomposes a total second count into hours, minutes, and seconds
        /// using loops to subtract each unit in turn.
        /// </summary>
        /// <param name="totalSeconds">The total elapsed seconds to decompose.</param>
        /// <returns>A tuple of (hours, minutes, seconds).</returns>
        private static (int hours, int minutes, int seconds) DecomposeSeconds(int totalSeconds)
        {
            int remaining = totalSeconds;
            int hours = 0;
            int minutes = 0;

            // Loop: peel off full hours
            while (remaining >= 3600)
            {
                hours++;
                remaining -= 3600;
            }

            // Loop: peel off full minutes from what remains
            while (remaining >= 60)
            {
                minutes++;
                remaining -= 60;
            }

            return (hours, minutes, remaining);
        }

        /// <summary>
        /// Formats hour, minute, and second components as <c>hh:mm:ss</c>.
        /// </summary>
        /// <param name="hours">Hour component (0–23+).</param>
        /// <param name="minutes">Minute component (0–59).</param>
        /// <param name="seconds">Second component (0–59).</param>
        /// <returns>A zero-padded time string, e.g. <c>01:05:09</c>.</returns>
        private static string FormatHhMmSs(int hours, int minutes, int seconds) =>
            $"{hours:D2}:{minutes:D2}:{seconds:D2}";

        // ── Public methods ───────────────────────────────────────────────────────

        /// <summary>
        /// Starts the stopwatch from <c>00:00:00</c>.
        /// If the stopwatch is already running this call is a no-op.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the stopwatch is in the <see cref="StopwatchState.Paused"/> state;
        /// call <see cref="Resume"/> instead.
        /// </exception>
        public void Start()
        {
            // Guard: paused state should use Resume
            if (_state == StopwatchState.Paused)
                throw new InvalidOperationException(
                    "Stopwatch is paused. Call Resume() to continue from the current time.");

            // Idempotent: already running → nothing to do
            if (_state == StopwatchState.Running)
                return;

            // Fresh start: zero everything out
            _accumulatedTime = TimeSpan.Zero;
            _startTime = DateTime.UtcNow;
            _state = StopwatchState.Running;
        }

        /// <summary>
        /// Pauses the stopwatch, preserving the current elapsed time.
        /// Displays the frozen time at the point of pause.
        /// </summary>
        /// <returns>
        /// The formatted elapsed time at the moment of pausing (<c>hh:mm:ss</c>).
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the stopwatch is not in the <see cref="StopwatchState.Running"/> state.
        /// </exception>
        public string Pause()
        {
            if (_state != StopwatchState.Running)
                throw new InvalidOperationException(
                    "Cannot pause: stopwatch is not running.");

            // Freeze the accumulated time at this exact moment
            _accumulatedTime += DateTime.UtcNow - _startTime;
            _state = StopwatchState.Paused;

            return FormattedTime;
        }

        /// <summary>
        /// Resumes the stopwatch from the last paused time.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the stopwatch is not in the <see cref="StopwatchState.Paused"/> state.
        /// </exception>
        public void Resume()
        {
            if (_state != StopwatchState.Paused)
                throw new InvalidOperationException(
                    "Cannot resume: stopwatch is not paused.");

            // Record the new start moment; _accumulatedTime already holds prior elapsed time
            _startTime = DateTime.UtcNow;
            _state = StopwatchState.Running;
        }

        /// <summary>
        /// Resets the stopwatch to <c>00:00:00</c> and returns to the
        /// <see cref="StopwatchState.Idle"/> state.
        /// Can be called from any state.
        /// </summary>
        public void Reset()
        {
            _accumulatedTime = TimeSpan.Zero;
            _state = StopwatchState.Idle;
        }

        /// <summary>
        /// Stops the stopwatch completely, preserving the last recorded time for display.
        /// </summary>
        /// <returns>
        /// The formatted elapsed time at the moment of stopping (<c>hh:mm:ss</c>).
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the stopwatch is not in the <see cref="StopwatchState.Running"/> or
        /// <see cref="StopwatchState.Paused"/> state.
        /// </exception>
        public string Stop()
        {
            if (_state != StopwatchState.Running && _state != StopwatchState.Paused)
                throw new InvalidOperationException(
                    "Cannot stop: stopwatch is neither running nor paused.");

            // If running, capture live time before transitioning
            if (_state == StopwatchState.Running)
                _accumulatedTime += DateTime.UtcNow - _startTime;

            _state = StopwatchState.Stopped;
            return FormattedTime;
        }
    }
}
