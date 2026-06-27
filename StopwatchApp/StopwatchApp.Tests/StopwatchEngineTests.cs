using NUnit.Framework;
using StopwatchApp.Core;
using System;
using System.Threading;

namespace StopwatchApp.Tests
{
    /// <summary>
    /// NUnit test suite for <see cref="StopwatchEngine"/>.
    /// Tests are written in Red → Green → Refactor (TDD) order.
    /// Each test is self-contained and uses the AAA (Arrange-Act-Assert) pattern.
    /// </summary>
    [TestFixture]
    public class StopwatchEngineTests
    {
        // ── Helper: fresh engine for each test ──────────────────────────────────
        private StopwatchEngine _engine;

        /// <summary>Creates a brand-new engine before every individual test.</summary>
        [SetUp]
        public void SetUp() => _engine = new StopwatchEngine();

        // ════════════════════════════════════════════════════════════════════════
        // 1. INITIAL STATE
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>A new engine should be in Idle state.</summary>
        [Test]
        public void NewEngine_StateIsIdle()
        {
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Idle));
        }

        /// <summary>A new engine should report zero elapsed time.</summary>
        [Test]
        public void NewEngine_ElapsedTimeIsZero()
        {
            Assert.That(_engine.ElapsedTime, Is.EqualTo(TimeSpan.Zero));
        }

        /// <summary>A new engine's formatted time must be 00:00:00.</summary>
        [Test]
        public void NewEngine_FormattedTimeIsZero()
        {
            Assert.That(_engine.FormattedTime, Is.EqualTo("00:00:00"));
        }

        // ════════════════════════════════════════════════════════════════════════
        // 2. START
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>After Start(), the state should be Running.</summary>
        [Test]
        public void Start_SetsStateToRunning()
        {
            _engine.Start();
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Running));
        }

        /// <summary>After Start(), elapsed time should be >= zero (not negative).</summary>
        [Test]
        public void Start_ElapsedTimeIsNonNegative()
        {
            _engine.Start();
            Assert.That(_engine.ElapsedTime.TotalSeconds, Is.GreaterThanOrEqualTo(0));
        }

        /// <summary>Calling Start() while already running should be a no-op (idempotent).</summary>
        [Test]
        public void Start_WhenAlreadyRunning_IsNoOp()
        {
            _engine.Start();
            Assert.DoesNotThrow(() => _engine.Start());
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Running));
        }

        /// <summary>Calling Start() while paused should throw InvalidOperationException.</summary>
        [Test]
        public void Start_WhenPaused_ThrowsInvalidOperationException()
        {
            _engine.Start();
            _engine.Pause();
            Assert.Throws<InvalidOperationException>(() => _engine.Start());
        }

        // ════════════════════════════════════════════════════════════════════════
        // 3. PAUSE
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>After Pause(), state should be Paused.</summary>
        [Test]
        public void Pause_SetsStateToPaused()
        {
            _engine.Start();
            _engine.Pause();
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Paused));
        }

        /// <summary>Pause() should return a formatted time string in hh:mm:ss.</summary>
        [Test]
        public void Pause_ReturnsFormattedTime()
        {
            _engine.Start();
            string time = _engine.Pause();
            // Must match hh:mm:ss pattern
            Assert.That(time, Does.Match(@"^\d{2}:\d{2}:\d{2}$"));
        }

        /// <summary>Elapsed time should not increase while paused.</summary>
        [Test]
        public void Pause_FreezesElapsedTime()
        {
            _engine.Start();
            _engine.Pause();
            TimeSpan frozen = _engine.ElapsedTime;
            Thread.Sleep(100); // 100ms passes...
            TimeSpan later = _engine.ElapsedTime;
            Assert.That(later, Is.EqualTo(frozen));
        }

        /// <summary>Pause() when not running should throw.</summary>
        [Test]
        public void Pause_WhenIdle_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _engine.Pause());
        }

        // ════════════════════════════════════════════════════════════════════════
        // 4. RESUME
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>After Resume(), state should be Running.</summary>
        [Test]
        public void Resume_SetsStateToRunning()
        {
            _engine.Start();
            _engine.Pause();
            _engine.Resume();
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Running));
        }

        /// <summary>Elapsed time after Resume() must be >= time at pause.</summary>
        [Test]
        public void Resume_ContinuesFromPausedTime()
        {
            _engine.Start();
            Thread.Sleep(50);
            _engine.Pause();
            TimeSpan atPause = _engine.ElapsedTime;
            _engine.Resume();
            Thread.Sleep(50);
            Assert.That(_engine.ElapsedTime, Is.GreaterThanOrEqualTo(atPause));
        }

        /// <summary>Resume() when not paused should throw.</summary>
        [Test]
        public void Resume_WhenRunning_ThrowsInvalidOperationException()
        {
            _engine.Start();
            Assert.Throws<InvalidOperationException>(() => _engine.Resume());
        }

        /// <summary>Resume() from Idle should throw.</summary>
        [Test]
        public void Resume_WhenIdle_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _engine.Resume());
        }

        // ════════════════════════════════════════════════════════════════════════
        // 5. RESET
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>After Reset(), state should be Idle.</summary>
        [Test]
        public void Reset_SetsStateToIdle()
        {
            _engine.Start();
            _engine.Reset();
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Idle));
        }

        /// <summary>After Reset(), elapsed time should be zero.</summary>
        [Test]
        public void Reset_ZeroesElapsedTime()
        {
            _engine.Start();
            Thread.Sleep(50);
            _engine.Reset();
            Assert.That(_engine.ElapsedTime, Is.EqualTo(TimeSpan.Zero));
        }

        /// <summary>After Reset(), FormattedTime should display 00:00:00.</summary>
        [Test]
        public void Reset_FormattedTimeIsZero()
        {
            _engine.Start();
            _engine.Pause();
            _engine.Reset();
            Assert.That(_engine.FormattedTime, Is.EqualTo("00:00:00"));
        }

        /// <summary>Reset() can be called from any state without throwing.</summary>
        [Test]
        public void Reset_CanBeCalledFromAnyState()
        {
            // From Idle
            Assert.DoesNotThrow(() => _engine.Reset());

            // From Running
            _engine.Start();
            Assert.DoesNotThrow(() => _engine.Reset());

            // From Paused
            _engine.Start();
            _engine.Pause();
            Assert.DoesNotThrow(() => _engine.Reset());
        }

        // ════════════════════════════════════════════════════════════════════════
        // 6. STOP
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>After Stop(), state should be Stopped.</summary>
        [Test]
        public void Stop_SetsStateToStopped()
        {
            _engine.Start();
            _engine.Stop();
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Stopped));
        }

        /// <summary>Stop() should return the last formatted time in hh:mm:ss.</summary>
        [Test]
        public void Stop_ReturnsFormattedTime()
        {
            _engine.Start();
            string time = _engine.Stop();
            Assert.That(time, Does.Match(@"^\d{2}:\d{2}:\d{2}$"));
        }

        /// <summary>Elapsed time should not change after Stop().</summary>
        [Test]
        public void Stop_FreezesElapsedTime()
        {
            _engine.Start();
            _engine.Stop();
            TimeSpan atStop = _engine.ElapsedTime;
            Thread.Sleep(100);
            Assert.That(_engine.ElapsedTime, Is.EqualTo(atStop));
        }

        /// <summary>Stop() when Idle should throw.</summary>
        [Test]
        public void Stop_WhenIdle_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _engine.Stop());
        }

        /// <summary>Stop() from Paused state should succeed and freeze time.</summary>
        [Test]
        public void Stop_WhenPaused_Succeeds()
        {
            _engine.Start();
            _engine.Pause();
            Assert.DoesNotThrow(() => _engine.Stop());
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Stopped));
        }

        // ════════════════════════════════════════════════════════════════════════
        // 7. FORMATTED TIME
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>FormattedTime must always follow the hh:mm:ss pattern.</summary>
        [Test]
        public void FormattedTime_AlwaysMatchesHhMmSsPattern()
        {
            // Idle
            Assert.That(_engine.FormattedTime, Does.Match(@"^\d{2}:\d{2}:\d{2}$"));

            // Running
            _engine.Start();
            Assert.That(_engine.FormattedTime, Does.Match(@"^\d{2}:\d{2}:\d{2}$"));

            // Paused
            _engine.Pause();
            Assert.That(_engine.FormattedTime, Does.Match(@"^\d{2}:\d{2}:\d{2}$"));
        }

        // ════════════════════════════════════════════════════════════════════════
        // 8. FULL WORKFLOW
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Verifies the complete Start → Pause → Resume → Stop → Reset workflow.
        /// </summary>
        [Test]
        public void FullWorkflow_StartPauseResumeStopReset()
        {
            // Start
            _engine.Start();
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Running));

            // Pause
            _engine.Pause();
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Paused));
            TimeSpan afterPause = _engine.ElapsedTime;

            // Resume — time should be >= afterPause
            _engine.Resume();
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Running));
            Assert.That(_engine.ElapsedTime, Is.GreaterThanOrEqualTo(afterPause));

            // Stop
            string lastTime = _engine.Stop();
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Stopped));
            Assert.That(lastTime, Does.Match(@"^\d{2}:\d{2}:\d{2}$"));

            // Reset
            _engine.Reset();
            Assert.That(_engine.State, Is.EqualTo(StopwatchState.Idle));
            Assert.That(_engine.FormattedTime, Is.EqualTo("00:00:00"));
        }
    }
}
