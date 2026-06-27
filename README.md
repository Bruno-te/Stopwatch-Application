# Stopwatch Application

A C# stopwatch with a **Windows Forms** UI (Windows), a **console UI** (macOS/Linux), and a separate **core logic** library tested with **NUnit**. The app displays elapsed time in `hh:mm:ss` format (`00:00:00`) and supports Start, Pause, Resume, Reset, and Stop.

**Repository:** [https://github.com/Bruno-te/Stopwatch-Application](https://github.com/Bruno-te/Stopwatch-Application)

---

## How It Works

### Architecture

| Project | Purpose |
|---------|---------|
| `StopwatchApp.Core` | Timer logic, state machine, time formatting (no UI) |
| `StopwatchApp.UI` | Windows Forms window with buttons and display (**Windows only**) |
| `StopwatchApp.Console` | Terminal UI for macOS/Linux (same features as WinForms) |
| `StopwatchApp.Tests` | NUnit tests (26 tests, TDD-style) |

The UI calls methods on `StopwatchEngine` and reads `State` / `FormattedTime`. A `System.Windows.Forms.Timer` fires **every second** while running, updating the display in a recurring loop.

### Button Behaviour

| Button | Action |
|--------|--------|
| **Start** | Starts from `00:00:00`; time increments every second |
| **Pause** | Freezes the timer and shows the current time |
| **Resume** | Continues from the last paused time |
| **Reset** | Returns to `00:00:00` from any state |
| **Stop** | Stops completely and displays the last recorded time |

Button enabled/disabled states are controlled with **if/else** blocks in `UpdateDisplay()` based on the current `StopwatchState`.

### Time Formatting

Elapsed seconds are decomposed into hours, minutes, and seconds using **while loops** in `DecomposeSeconds()`, then formatted as `hh:mm:ss` by `FormatHhMmSs()`.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## Why macOS Uses the Console UI (Not WinForms)

This project includes a **Windows Forms** UI (`StopwatchApp.UI`) as required by the assignment. However, **WinForms cannot run on a MacBook**:

- WinForms is built on **Windows-only APIs** (`System.Windows.Forms`) that do not exist on macOS.
- The UI project targets `net8.0-windows`, which the .NET SDK refuses to build on macOS unless Windows targeting is enabled — and even then, the app **still would not launch** because the underlying GUI framework is Windows-only.
- You may see this error on Mac if you try to run the WinForms project:

  ```
  error NETSDK1100: To build a project targeting Windows on this operating system...
  ```

For that reason, a **cross-platform console UI** (`StopwatchApp.Console`) was added. It uses the same `StopwatchEngine` core logic and supports all five operations (Start, Pause, Resume, Reset, Stop). Run the console version on your MacBook; use the WinForms version on Windows if a graphical demo is required.

---

## How to Run

### Clone the repository

```bash
git clone https://github.com/Bruno-te/Stopwatch-Application.git
cd Stopwatch-Application/StopwatchApp
```

### Run tests

From the `StopwatchApp` folder:

```bash
dotnet test StopwatchApp.Tests/StopwatchApp.Tests.csproj
```

All **26 tests** should pass.

> On macOS, test the Core/Tests projects only. The full solution includes the WinForms project, which cannot build on Mac.

### Run the application

**On macOS / Linux (MacBook)** — use the console UI:

```bash
# From the repo root:
cd StopwatchApp

# Then run (do NOT cd into StopwatchApp again):
dotnet run --project StopwatchApp.Console/StopwatchApp.Console.csproj
```

Use single keys `1` Start, `2` Pause, `3` Resume, `4` Reset, `5` Stop, `Q` Quit — **no Enter needed**.

**On Windows** — use the WinForms UI:

```bash
cd StopwatchApp
dotnet run --project StopwatchApp.UI/StopwatchApp.UI.csproj
```

Or open `StopwatchApp/StopwatchApp.sln` in Visual Studio and press **F5**.

---

## Project Structure

```
Stopwatch-Application/
├── README.md                       ← this file
├── StopwatchApp/
│   ├── StopwatchApp.sln
│   ├── StopwatchApp.Core/
│   │   └── StopwatchEngine.cs      ← core logic + XML docs
│   ├── StopwatchApp.UI/
│   │   ├── MainForm.cs             ← WinForms UI (Windows only)
│   │   └── Program.cs
│   ├── StopwatchApp.Console/
│   │   └── Program.cs              ← Console UI (macOS/Linux)
│   └── StopwatchApp.Tests/
│       └── StopwatchEngineTests.cs
└── docs/
    └── SUBMISSION.md               ← deliverables checklist
```

---

## XML Documentation

All public types and methods include `///` XML documentation comments.

Generate the XML doc file:

```bash
cd StopwatchApp
dotnet build StopwatchApp.Core/StopwatchApp.Core.csproj
```

Output: `StopwatchApp.Core/bin/Debug/net8.0/StopwatchApp.Core.xml`

---

## Assignment Deliverables Checklist

| Deliverable | Status |
|-------------|--------|
| Windows Forms UI with all 5 buttons | Done |
| Time displayed as `00:00:00` (hh:mm:ss) | Done |
| Loops and functions for timer operations | Done |
| if/else for button logic | Done |
| XML documentation on methods | Done |
| README with run instructions | Done |
| GitHub repository | [Bruno-te/Stopwatch-Application](https://github.com/Bruno-te/Stopwatch-Application) |
| Screen recording (3–5 min) | Record using the console UI on Mac, or WinForms on Windows |
| Screenshot (time ≠ 00:00:00) | Capture terminal (Mac) or WinForms window (Windows) |

See [docs/SUBMISSION.md](docs/SUBMISSION.md) for step-by-step submission guidance.

---

## Uploading to GitHub

If you have not pushed yet, from the repo root:

```bash
git add .
git commit -m "Add stopwatch application with WinForms UI and NUnit tests"
git push -u origin main
```
