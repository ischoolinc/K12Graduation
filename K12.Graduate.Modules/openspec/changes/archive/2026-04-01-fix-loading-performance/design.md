## Context

The K12 Graduation Modules plugin runs inside the FISCA host application as a Windows Forms panel. After commit `2366cc9` moved startup initialization off the UI thread, six performance issues remain open (see `system_loading_delay.md`). These span three areas: view data loading (`TagView`, `ArchiveNoteView`, `GraduationYearView`), search execution (`GraduationAdmin`), and memory management (unbounded record loads, N+1 queries). All fixes must stay within the existing FISCA/K12.Data/DevComponents stack — no new dependencies.

## Goals / Non-Goals

**Goals:**
- Move all view `SourceChanged` DB queries off the UI thread using `BackgroundWorker`
- Reduce search complexity from O(6n) with O(n) list lookups to O(n) with O(1) HashSet lookups
- Replace O(n²) `List.Contains()` dedup loops in views with O(n) `HashSet<T>` equivalents
- Bound graduate record queries with a configurable page size constant
- Eliminate N+1 queries in detail views by fetching related records together where possible
- Wrap tree node construction with `BeginUpdate()`/`EndUpdate()` in all three views

**Non-Goals:**
- Rewriting the FISCA data access layer or AccessHelper
- Introducing virtual scrolling or server-side paging (FISCA framework does not support this)
- Adding unit tests (no existing test infrastructure in this project)
- Changing the user-visible behavior, search results, or UI layout

## Decisions

### Decision 1: BackgroundWorker over Task.Run for View Loading

Views already use `BackgroundWorker` for detail panel loading (`GraduateDetailItem`, `WrittenInfomationItem`). Adopting the same pattern for view `SourceChanged` handlers ensures consistency and avoids introducing `async/await` to a .NET 4.8 WinForms codebase that has not yet adopted it broadly.

**Alternative considered:** `Task.Run()` with `Control.Invoke()` — already used in `Program.cs` and `GraduationAdmin.cs`. Rejected here because views have progress-indicator needs (show loading state while tree is being built) that `BackgroundWorker.ReportProgress` handles more cleanly than `Task.ContinueWith`.

### Decision 2: Single-Pass Search with HashSet Results

Replace the six sequential `foreach` loops in `GraduationAdmin.Search()` with one loop that checks all six fields and accumulates results in a `HashSet<string>`. The regex `Regex` object is compiled once before the loop.

**Alternative considered:** Pre-building per-field inverted indexes at load time. Rejected because the graduate dataset is loaded once per session and re-loaded on refresh; the index would need invalidation logic that adds complexity without proportional gain at typical school sizes (< 20,000 graduates).

### Decision 3: Constant Page Size for Graduate Queries

Introduce a `private const int MaxGraduateLoadCount = 2000` in `GraduationAdmin`. The `Select<GraduateUDT>` call appends `LIMIT {MaxGraduateLoadCount}` if the FISCA AccessHelper supports it, otherwise the list is trimmed post-fetch. A UI notice is shown when the result is truncated.

**Alternative considered:** Full lazy/virtual paging. Rejected — the FISCA ListPane does not expose a virtual-scroll API, and implementing it would require significant framework work beyond this change's scope.

### Decision 4: Co-locate Related UDT Fetches (N+1 Mitigation)

In `GraduateDetailItem` and `WrittenInfomationItem`, replace two sequential `Select<T>` calls with a single batch fetch using the `ref_uid IN (...)` pattern already used in the view classes. Since detail items load exactly one record at a time, the gain is one saved round-trip per open; the implementation is straightforward.

### Decision 5: BeginUpdate/EndUpdate for Tree Construction

All three view `BuildTree()` methods will wrap their node-append loops with `AdvTree.BeginUpdate()` / `AdvTree.EndUpdate()` (DevComponents API). This prevents the control from re-rendering after each `Nodes.Add()` call, which is the dominant cost in tree builds with hundreds of nodes.

## Risks / Trade-offs

- **BackgroundWorker cancellation** → If the user changes selection while a view is loading, the in-flight BGW result must be discarded. Mitigation: check `e.Cancelled` in `RunWorkerCompleted`; cancel the previous worker before starting a new one.
- **Page-size truncation UX** → Showing fewer records than expected without explanation confuses users. Mitigation: display a status-bar message "Showing first 2,000 records — refine your search to see more."
- **Thread safety on TestDic** → `GraduationAdmin.TestDic` is written by the main BGW and read by search on the UI thread. These do not currently overlap, but the BGW refactor must ensure `TestDic` is only assigned in `RunWorkerCompleted` (UI thread), not in `DoWork`.

## Migration Plan

All changes are in-process and require no schema migration, no data migration, and no configuration changes. The fix is applied by replacing the relevant method bodies in the listed files. The application is redeployed as a new DLL via the existing FISCA module distribution mechanism.

Rollback: revert to the previous commit on the `fast-loading` branch.

## Open Questions

- Does the FISCA `AccessHelper.Select<T>()` accept a SQL `LIMIT` clause directly in the condition string, or must the list be trimmed in application code? — Needs verification against AccessHelper source or docs before implementing Decision 3.
