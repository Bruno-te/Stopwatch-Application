using System;
using System.Drawing;
using System.Windows.Forms;
using StopwatchApp.Core;

namespace StopwatchApp.UI
{
    /// <summary>
    /// Main application window for the Stopwatch.
    /// Displays elapsed time in <c>hh:mm:ss</c> format and provides
    /// Start, Pause, Resume, Reset, and Stop controls.
    /// </summary>
    public partial class MainForm : Form
    {
        // ── Fields ───────────────────────────────────────────────────────────────
        private readonly StopwatchEngine _engine;   // Core logic (no UI dependency)
        private readonly System.Windows.Forms.Timer _uiTimer; // Fires every second to refresh display

        // ── UI Controls (declared here; laid out in InitializeComponent) ─────────
        private Label _lblTime        = null!;
        private Label _lblStatus      = null!;
        private Button _btnStart      = null!;
        private Button _btnPause      = null!;
        private Button _btnResume     = null!;
        private Button _btnReset      = null!;
        private Button _btnStop       = null!;
        private Panel _pnlHeader      = null!;
        private Panel _pnlButtons     = null!;

        // ── Colour palette ────────────────────────────────────────────────────────
        private static readonly Color BgDark     = Color.FromArgb(18, 18, 30);
        private static readonly Color AccentBlue = Color.FromArgb(0, 120, 215);
        private static readonly Color AccentGreen= Color.FromArgb(22, 198, 12);
        private static readonly Color AccentRed  = Color.FromArgb(232, 17, 35);
        private static readonly Color AccentGray = Color.FromArgb(90, 90, 110);
        private static readonly Color AccentAmber= Color.FromArgb(255, 185, 0);
        private static readonly Color TextWhite  = Color.FromArgb(240, 240, 250);

        /// <summary>
        /// Initialises the form, creates the stopwatch engine and the UI refresh timer.
        /// </summary>
        public MainForm()
        {
            _engine = new StopwatchEngine();

            // UI refresh timer — fires every second while the stopwatch is running
            _uiTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _uiTimer.Tick += UiTimer_Tick;

            InitializeComponent();
            UpdateDisplay();
        }

        // ════════════════════════════════════════════════════════════════════════
        // Layout
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Builds the form layout: header panel (time + status label)
        /// and a button panel with all five controls.
        /// </summary>
        private void InitializeComponent()
        {
            // ── Form ─────────────────────────────────────────────────────────────
            Text            = "Stopwatch";
            Size            = new Size(480, 340);
            MinimumSize     = Size;
            MaximumSize     = Size;
            BackColor       = BgDark;
            ForeColor       = TextWhite;
            StartPosition   = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox     = false;

            // ── Header panel: shows the timer ────────────────────────────────────
            _pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 160,
                BackColor = Color.FromArgb(24, 24, 40)
            };
            Controls.Add(_pnlHeader);

            _lblTime = new Label
            {
                Text      = "00:00:00",
                Font      = new Font("Consolas", 54, FontStyle.Bold),
                ForeColor = AccentBlue,
                BackColor = Color.Transparent,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock      = DockStyle.Top,
                Height    = 110
            };
            _pnlHeader.Controls.Add(_lblTime);

            _lblStatus = new Label
            {
                Text      = "● IDLE",
                Font      = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = AccentGray,
                BackColor = Color.Transparent,
                AutoSize  = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock      = DockStyle.Fill
            };
            _pnlHeader.Controls.Add(_lblStatus);

            // ── Button panel ─────────────────────────────────────────────────────
            _pnlButtons = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = BgDark,
                Padding   = new Padding(20, 16, 20, 16)
            };
            Controls.Add(_pnlButtons);

            // Five buttons in a horizontal flow layout
            var flow = new FlowLayoutPanel
            {
                Dock        = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents  = false,
                AutoSize      = false
            };
            _pnlButtons.Controls.Add(flow);

            _btnStart  = MakeButton("▶  Start",  AccentGreen, BtnStart_Click);
            _btnPause  = MakeButton("⏸  Pause",  AccentAmber, BtnPause_Click);
            _btnResume = MakeButton("⏵  Resume", AccentBlue,  BtnResume_Click);
            _btnReset  = MakeButton("↺  Reset",  AccentGray,  BtnReset_Click);
            _btnStop   = MakeButton("■  Stop",   AccentRed,   BtnStop_Click);

            flow.Controls.AddRange(new Control[]
            {
                _btnStart, _btnPause, _btnResume, _btnReset, _btnStop
            });
        }

        /// <summary>
        /// Factory helper that creates a consistently styled button.
        /// </summary>
        /// <param name="text">Label text displayed on the button.</param>
        /// <param name="colour">Background (accent) colour.</param>
        /// <param name="handler">Click event handler to attach.</param>
        /// <returns>A fully configured <see cref="Button"/>.</returns>
        private static Button MakeButton(string text, Color colour, EventHandler handler)
        {
            var btn = new Button
            {
                Text      = text,
                Size      = new Size(76, 110),
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = colour,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin    = new Padding(2, 0, 2, 0),
                Cursor    = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize  = 0;
            btn.FlatAppearance.MouseOverBackColor =
                ControlPaint.Light(colour, 0.3f);
            btn.Click += handler;
            return btn;
        }

        // ════════════════════════════════════════════════════════════════════════
        // Button handlers
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Handles the Start button click.
        /// If the engine is Paused, delegates to <see cref="BtnResume_Click"/> instead.
        /// </summary>
        private void BtnStart_Click(object? sender, EventArgs e)
        {
            try
            {
                _engine.Start();
                _uiTimer.Start();
                UpdateDisplay();
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Handles the Pause button click.
        /// Freezes the elapsed time and stops the UI refresh timer.
        /// </summary>
        private void BtnPause_Click(object? sender, EventArgs e)
        {
            try
            {
                string time = _engine.Pause();
                _uiTimer.Stop();
                UpdateDisplay();
                _lblStatus.Text = $"⏸ PAUSED at {time}";
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Handles the Resume button click.
        /// Restarts the UI refresh timer from the frozen time.
        /// </summary>
        private void BtnResume_Click(object? sender, EventArgs e)
        {
            try
            {
                _engine.Resume();
                _uiTimer.Start();
                UpdateDisplay();
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Handles the Reset button click.
        /// Stops the UI timer and resets all display back to 00:00:00.
        /// </summary>
        private void BtnReset_Click(object? sender, EventArgs e)
        {
            _uiTimer.Stop();
            _engine.Reset();
            UpdateDisplay();
        }

        /// <summary>
        /// Handles the Stop button click.
        /// Stops counting and displays the last recorded time.
        /// </summary>
        private void BtnStop_Click(object? sender, EventArgs e)
        {
            try
            {
                string last = _engine.Stop();
                _uiTimer.Stop();
                UpdateDisplay();
                _lblStatus.Text = $"■ STOPPED — last time: {last}";
            }
            catch (InvalidOperationException ex)
            {
                ShowError(ex.Message);
            }
        }

        // ════════════════════════════════════════════════════════════════════════
        // Display helpers
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Timer tick handler — refreshes the time label every second while running.
        /// </summary>
        private void UiTimer_Tick(object? sender, EventArgs e) => UpdateDisplay();

        /// <summary>
        /// Synchronises the UI label, status text, and button enabled-states
        /// with the current <see cref="StopwatchEngine"/> state.
        /// </summary>
        private void UpdateDisplay()
        {
            _lblTime.Text = _engine.FormattedTime;

            // If/else block to handle each possible state
            if (_engine.State == StopwatchState.Idle)
            {
                _lblTime.ForeColor  = AccentBlue;
                _lblStatus.Text     = "● IDLE";
                _lblStatus.ForeColor= AccentGray;
                SetButtonStates(start: true, pause: false, resume: false, reset: true, stop: false);
            }
            else if (_engine.State == StopwatchState.Running)
            {
                _lblTime.ForeColor  = AccentGreen;
                _lblStatus.Text     = "▶ RUNNING";
                _lblStatus.ForeColor= AccentGreen;
                SetButtonStates(start: false, pause: true, resume: false, reset: false, stop: true);
            }
            else if (_engine.State == StopwatchState.Paused)
            {
                _lblTime.ForeColor  = AccentAmber;
                _lblStatus.ForeColor= AccentAmber;
                // status text set by handler (includes the pause time)
                SetButtonStates(start: false, pause: false, resume: true, reset: true, stop: true);
            }
            else // Stopped
            {
                _lblTime.ForeColor  = AccentRed;
                _lblStatus.ForeColor= AccentRed;
                // status text set by handler (includes the stop time)
                SetButtonStates(start: true, pause: false, resume: false, reset: true, stop: false);
            }
        }

        /// <summary>
        /// Enables or disables each button based on the current state,
        /// visually dimming disabled buttons for clear feedback.
        /// </summary>
        /// <param name="start">Whether the Start button should be enabled.</param>
        /// <param name="pause">Whether the Pause button should be enabled.</param>
        /// <param name="resume">Whether the Resume button should be enabled.</param>
        /// <param name="reset">Whether the Reset button should be enabled.</param>
        /// <param name="stop">Whether the Stop button should be enabled.</param>
        private void SetButtonStates(bool start, bool pause, bool resume, bool reset, bool stop)
        {
            SetBtn(_btnStart,  start,  AccentGreen);
            SetBtn(_btnPause,  pause,  AccentAmber);
            SetBtn(_btnResume, resume, AccentBlue);
            SetBtn(_btnReset,  reset,  AccentGray);
            SetBtn(_btnStop,   stop,   AccentRed);
        }

        /// <summary>
        /// Applies enabled state and visual dimming to a single button.
        /// </summary>
        /// <param name="btn">The button to configure.</param>
        /// <param name="enabled">Whether the button is active.</param>
        /// <param name="activeColour">The full-brightness colour when enabled.</param>
        private static void SetBtn(Button btn, bool enabled, Color activeColour)
        {
            btn.Enabled   = enabled;
            btn.BackColor = enabled
                ? activeColour
                : Color.FromArgb(45, 45, 55);   // dim when disabled
        }

        /// <summary>
        /// Displays an error dialog with the given message.
        /// </summary>
        /// <param name="message">The error text to display.</param>
        private void ShowError(string message) =>
            MessageBox.Show(message, "Stopwatch Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);

        /// <summary>
        /// Cleans up managed resources (the UI timer) on form dispose.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> if managed resources should be disposed.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _uiTimer.Dispose();
            base.Dispose(disposing);
        }
    }
}
