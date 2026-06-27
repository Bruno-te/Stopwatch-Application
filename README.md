# Stopwatch Application

A C# stopwatch with a **Windows Forms** user interface and a separate **core logic** library tested with **NUnit**. The app displays elapsed time in `hh:mm:ss` format (`00:00:00`) and supports Start, Pause, Resume, Reset, and Stop.

**Repository:** [https://github.com/Bruno-te/Stopwatch-Application](https://github.com/Bruno-te/Stopwatch-Application)

---

## How It Works

### Architecture

| Project | Purpose |
|---------|---------|
| `StopwatchApp.Core` | Timer logic, state machine, time formatting (no UI) |
| `StopwatchApp.UI` | Windows Forms window with buttons and display |
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
- **Windows** (required for the Windows Forms UI)

> **Note:** You are developing on macOS. The WinForms UI only runs on Windows. Use a Windows PC, VM, or cloud Windows machine to run the UI, capture your screenshot, and record your demo video.

---

## How to Run

### Clone the repository

```bash
git clone https://github.com/Bruno-te/Stopwatch-Application.git
cd Stopwatch-Application/StopwatchApp
```

### Run tests

```bash
dotnet test StopwatchApp.sln
```

All **26 tests** should pass.

### Run the application (Windows only)

```bash
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
│   │   ├── MainForm.cs             ← WinForms UI + button handlers
│   │   └── Program.cs
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
| Screen recording (3–5 min) | **You record this on Windows** |
| Screenshot (time ≠ 00:00:00) | **You capture this on Windows** |

See [docs/SUBMISSION.md](docs/SUBMISSION.md) for step-by-step submission guidance.

---

## Uploading to GitHub

If you have not pushed yet, from the repo root:

```bash
git add .
git commit -m "Add stopwatch application with WinForms UI and NUnit tests"
git push -u origin main
```
