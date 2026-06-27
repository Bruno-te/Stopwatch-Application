using StopwatchApp.Core;

namespace StopwatchApp.ConsoleUI
{
    /// <summary>
    /// Cross-platform console UI for the stopwatch.
    /// Use this on macOS/Linux; use <c>StopwatchApp.UI</c> (WinForms) on Windows.
    /// </summary>
    internal static class Program
    {
        /// <summary>Application entry point.</summary>
        static void Main()
        {
            var app = new ConsoleStopwatchApp();
            app.Run();
        }
    }

    /// <summary>
    /// Interactive terminal stopwatch with Start, Pause, Resume, Reset, and Stop.
    /// Displays elapsed time in <c>hh:mm:ss</c> format.
    /// </summary>
    internal sealed class ConsoleStopwatchApp
    {
        private readonly StopwatchEngine _engine = new();
        private bool _running = true;

        /// <summary>
        /// Runs the main input loop until the user quits.
        /// </summary>
        public void Run()
        {
            PrintHeader();

            while (_running)
            {
                Render();
                PrintPrompt();

                if (_engine.State == StopwatchState.Running)
                {
                    // While running, poll for keys so Pause/Stop/Quit work immediately
                    if (!WaitForKeyOrTick(TimeSpan.FromSeconds(1)))
                        break;
                }
                else
                {
                    HandleCommand(ReadCommandKey());
                }
            }
        }

        /// <summary>
        /// Waits up to one second for a key press, refreshing the timer each interval.
        /// </summary>
        /// <param name="interval">How long to wait before the next display refresh.</param>
        /// <returns><see langword="false"/> if the app should exit; otherwise <see langword="true"/>.</returns>
        private bool WaitForKeyOrTick(TimeSpan interval)
        {
            long deadline = Environment.TickCount64 + (long)interval.TotalMilliseconds;

            while (Environment.TickCount64 < deadline && _running)
            {
                if (Console.KeyAvailable)
                {
                    HandleCommand(ReadCommandKey());
                    return _running;
                }

                Thread.Sleep(50);
            }

            return _running;
        }

        /// <summary>Reads a single key command from the console.</summary>
        /// <returns>The upper-cased key character as a string.</returns>
        private static string ReadCommandKey()
        {
            ConsoleKeyInfo key = Console.ReadKey(intercept: true);
            Console.WriteLine(key.KeyChar);
            return char.ToUpperInvariant(key.KeyChar).ToString();
        }

        /// <summary>Prints the welcome banner and control legend.</summary>
        private static void PrintHeader()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║         STOPWATCH APPLICATION        ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.WriteLine();
            Console.WriteLine("  1 = Start   2 = Pause   3 = Resume");
            Console.WriteLine("  4 = Reset   5 = Stop    Q = Quit");
            Console.WriteLine();
            Console.WriteLine("  Press a single key (no Enter needed).");
            Console.WriteLine();
        }

        /// <summary>Prints the command prompt line.</summary>
        private static void PrintPrompt() =>
            Console.Write("  Key [1=Start 2=Pause 3=Resume 4=Reset 5=Stop Q=Quit]: ");

        /// <summary>
        /// Handles a single user command using if/else branching.
        /// </summary>
        /// <param name="command">The key the user pressed.</param>
        private void HandleCommand(string? command)
        {
            if (command == "1")
            {
                TryAction(() => _engine.Start(), "Started from 00:00:00");
            }
            else if (command == "2")
            {
                TryAction(() =>
                {
                    string time = _engine.Pause();
                    Console.WriteLine($"  Paused at {time}");
                });
            }
            else if (command == "3")
            {
                TryAction(() => _engine.Resume(), "Resumed");
            }
            else if (command == "4")
            {
                _engine.Reset();
                Console.WriteLine("  Reset to 00:00:00");
            }
            else if (command == "5")
            {
                TryAction(() =>
                {
                    string last = _engine.Stop();
                    Console.WriteLine($"  Stopped — last time: {last}");
                });
            }
            else if (command == "Q")
            {
                _running = false;
                Console.WriteLine("  Goodbye!");
            }
            else
            {
                Console.WriteLine("  Unknown key. Use 1–5 or Q.");
            }
        }

        /// <summary>
        /// Executes a stopwatch action and prints any error message.
        /// </summary>
        /// <param name="action">The engine operation to perform.</param>
        /// <param name="successMessage">Optional message shown on success.</param>
        private static void TryAction(Action action, string? successMessage = null)
        {
            try
            {
                action();
                if (successMessage != null)
                    Console.WriteLine($"  {successMessage}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"  Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Redraws the time display and current status on the console.
        /// </summary>
        private void Render()
        {
            Console.WriteLine();
            Console.WriteLine($"  ┌─────────────────┐");
            Console.WriteLine($"  │   {_engine.FormattedTime}   │");
            Console.WriteLine($"  └─────────────────┘");
            Console.WriteLine($"  Status: {StatusLabel()}");
        }

        /// <summary>
        /// Returns a human-readable label for the current engine state.
        /// </summary>
        /// <returns>Status text such as "IDLE" or "RUNNING".</returns>
        private string StatusLabel()
        {
            if (_engine.State == StopwatchState.Idle)
                return "IDLE";
            if (_engine.State == StopwatchState.Running)
                return "RUNNING";
            if (_engine.State == StopwatchState.Paused)
                return "PAUSED";
            return "STOPPED";
        }
    }
}
