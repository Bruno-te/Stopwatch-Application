# Stopwatch Application — TDD with NUnit

A C# stopwatch built using **Test-Driven Development (TDD)** and **NUnit**.  
The project separates business logic from the UI so every feature is covered by automated tests.

---

## Project Structure

```
StopwatchApp/
├── StopwatchApp.sln
├── StopwatchApp.Core/          # Business logic — no UI dependency
│   └── StopwatchEngine.cs      # State machine + time tracking
├── StopwatchApp.Tests/         # NUnit test project
│   └── StopwatchEngineTests.cs # 25+ tests covering all states
└── StopwatchApp.UI/            # Windows Forms front-end
    ├── MainForm.cs             # UI layout + button handlers
    └── Program.cs              # Entry point
```

---

## Features

| Button | Action |
|--------|--------|
| ▶ Start | Starts from `00:00:00` |
| ⏸ Pause | Freezes time, displays current value |
| ⏵ Resume | Continues from paused time |
| ↺ Reset | Returns to `00:00:00` (Idle state) |
| ■ Stop | Stops completely, displays last time |

- Time is displayed in `hh:mm:ss` format.
- Button colours change per state (green running, amber paused, red stopped).
- Disabled buttons are visually dimmed so only valid actions are available.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows (for the Windows Forms UI project)

---

## How to Run

### 1 — Clone / download the repo

```bash
git clone https://github.com/<your-username>/StopwatchApp.git
cd StopwatchApp
```

### 2 — Run the tests

```bash
dotnet test StopwatchApp.Tests/StopwatchApp.Tests.csproj
```

All 25 tests should pass, covering:
- Initial state
- Start / Pause / Resume / Reset / Stop behaviour
- Invalid-state guards (exceptions)
- Full workflow integration

### 3 — Run the UI application

```bash
dotnet run --project StopwatchApp.UI/StopwatchApp.UI.csproj
```

> The UI project targets `net8.0-windows` and requires Windows.  
> To run cross-platform, use the core library with a console or Avalonia UI front-end.

---

## TDD Approach

Each feature was implemented in three phases:

1. **Red** — Write a failing test that describes the desired behaviour.
2. **Green** — Write the minimum code to make the test pass.
3. **Refactor** — Improve structure/comments without breaking any tests.

Example cycle for the **Pause** feature:

```csharp
// 1. RED — test written first, StopwatchEngine.Pause() doesn't exist yet
[Test]
public void Pause_FreezesElapsedTime()
{
    _engine.Start();
    _engine.Pause();
    TimeSpan frozen = _engine.ElapsedTime;
    Thread.Sleep(100);
    Assert.That(_engine.ElapsedTime, Is.EqualTo(frozen));
}

// 2. GREEN — implement Pause() in StopwatchEngine
public string Pause()
{
    _accumulatedTime += DateTime.UtcNow - _startTime;
    _state = StopwatchState.Paused;
    return FormattedTime;
}

// 3. REFACTOR — add XML docs, guard clauses, exception messages
```

---

## XML Documentation

All public members include `/// <summary>` XML documentation.  
Generate the HTML docs with:

```bash
dotnet build StopwatchApp.Core/StopwatchApp.Core.csproj
# XML file appears at: StopwatchApp.Core/bin/Debug/net8.0/StopwatchApp.Core.xml
```

---

## Architecture Notes

- `StopwatchEngine` is a pure C# class with **no UI dependency** — easily unit-tested.
- `MainForm` only calls public methods on `StopwatchEngine` and reads `State` / `FormattedTime`.
- A `System.Windows.Forms.Timer` (250 ms interval) refreshes the label while running.
- `if/else` blocks in `UpdateDisplay()` enforce the correct button states per `StopwatchState` enum value.

---

## Deliverables Checklist

- [x] Windows Forms UI with all five buttons
- [x] Time displayed as `hh:mm:ss`
- [x] TDD with NUnit (25+ tests)
- [x] XML documentation on all methods
- [x] Well-structured, commented code
- [x] GitHub repository — [Bruno-te/Stopwatch-Application](https://github.com/Bruno-te/Stopwatch-Application)
- [ ] Screen recording (3–5 min demo) — see [../docs/SUBMISSION.md](../docs/SUBMISSION.md)
- [ ] Screenshot of running time ≠ `00:00:00` — see [../docs/SUBMISSION.md](../docs/SUBMISSION.md)
