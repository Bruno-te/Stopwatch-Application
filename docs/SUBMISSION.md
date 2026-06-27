# Submission Guide

Use this checklist before turning in the assignment.

---

## 1. GitHub Repository

Your repo is configured at:

**https://github.com/Bruno-te/Stopwatch-Application**

Push your code (from the project root):

```bash
cd Stopwatch-Application
git add .
git commit -m "Add stopwatch application with WinForms UI and NUnit tests"
git push -u origin main
```

Verify on GitHub that these files are visible:

- `README.md`
- `StopwatchApp/StopwatchApp.sln`
- `StopwatchApp/StopwatchApp.Core/StopwatchEngine.cs`
- `StopwatchApp/StopwatchApp.UI/MainForm.cs`
- `StopwatchApp/StopwatchApp.Tests/StopwatchEngineTests.cs`

---

## 2. Screenshot (elapsed time ≠ 00:00:00)

1. On a **Windows** machine, run:

   ```bash
   cd StopwatchApp
   dotnet run --project StopwatchApp.UI/StopwatchApp.UI.csproj
   ```

2. Click **Start** and wait at least a few seconds (e.g. until `00:00:05` or higher).

3. Take a screenshot showing:
   - The time display **not** at `00:00:00`
   - The running/paused/stopped status label
   - The five buttons

4. Save as `docs/screenshot-running.png` (optional, for your records) and submit per your course instructions.

---

## 3. Screen Recording (3–5 minutes)

Record a demo that shows:

1. **Start** — timer begins at `00:00:00` and counts up each second
2. **Pause** — time freezes; status shows paused time
3. **Resume** — counting continues from paused value
4. **Reset** — returns to `00:00:00`
5. **Start again**, then **Stop** — shows last recorded time
6. (Optional) Run `dotnet test StopwatchApp.sln` to show tests passing

**Windows:** Use Xbox Game Bar (`Win + G`) or Snipping Tool → Record.

**macOS (tests only):** You can record the terminal running tests; the WinForms UI must be recorded on Windows.

---

## 4. XML Documentation Checkpoint

Reviewers will check XML docs on methods. Key files:

- `StopwatchApp.Core/StopwatchEngine.cs` — all public members documented
- `StopwatchApp.UI/MainForm.cs` — form, handlers, and helpers documented
- `StopwatchApp.UI/Program.cs` — entry point documented

Build to confirm docs compile:

```bash
cd StopwatchApp
dotnet build StopwatchApp.sln
```

---

## 5. Technical Requirements Mapping

| Requirement | Where implemented |
|-------------|-------------------|
| Start from 00:00:00 | `StopwatchEngine.Start()` |
| Increment every second | `MainForm` UI timer (1000 ms interval) |
| Pause / display time | `StopwatchEngine.Pause()` + status label |
| Resume from pause | `StopwatchEngine.Resume()` |
| Reset to 00:00:00 | `StopwatchEngine.Reset()` |
| Stop / show last time | `StopwatchEngine.Stop()` + status label |
| Loops | `DecomposeSeconds()` while loops |
| Functions | `Start`, `Pause`, `Resume`, `Reset`, `Stop`, `FormatHhMmSs` |
| if/else button logic | `MainForm.UpdateDisplay()` |
| hh:mm:ss format | `FormattedTime` property |
