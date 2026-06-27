using System;
using System.Windows.Forms;

namespace StopwatchApp.UI
{
    /// <summary>
    /// Application entry point.
    /// Configures high-DPI awareness and launches the main window.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enable high-DPI support for sharp rendering on 4K/HiDPI displays
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
