## Why

The K12 Graduation Modules desktop application has six remaining performance issues causing UI thread blocking and O(n²) complexity in search and view construction, degrading responsiveness for schools with large graduate datasets. While the most critical startup-blocking issues were addressed in commit `2366cc9`, the current `fast-loading` branch still has synchronous database queries on the UI thread, inefficient search loops, and unbounded memory loads that become painful at scale.

## What Changes

- Replace synchronous `SourceChanged` database queries in all three view classes (`TagView`, `ArchiveNoteView`, `GraduationYearView`) with `BackgroundWorker`-based async loading
- Consolidate six separate search `foreach` loops in `GraduationAdmin` into a single pass, replacing `List<T>.Contains()` deduplication with `HashSet<T>`
- Replace `List<string>.Contains()` deduplication in `TagView` and `ArchiveNoteView` loops with `HashSet<T>` to reduce O(n²) to O(n)
- Add pagination/batch limiting to the graduate record load in `GraduationAdmin` (max records per load)
- Eliminate N+1 queries in `GraduateDetailItem` and `WrittenInfomationItem` by batching related record fetches
- Add `BeginUpdate()`/`EndUpdate()` guards around tree node construction in all three view classes

## Capabilities

### New Capabilities

- `async-view-loading`: Views load graduate data asynchronously on background threads, keeping the UI thread unblocked during `SourceChanged` events
- `optimized-search`: Single-pass multi-field search with O(1) HashSet deduplication replaces six sequential O(n) loops
- `paginated-data-load`: Graduate record queries are bounded by a configurable page size to prevent unbounded memory consumption

### Modified Capabilities

<!-- No existing specs change behavioral requirements — these are pure implementation optimizations -->

## Impact

- **Files modified:** `GraduationAdmin.cs`, `View/TagView.cs`, `View/ArchiveNoteView.cs`, `View/GraduationYearView.cs`, `GraduateExtendControls/GraduateDetailItem.cs`, `GraduateExtendControls/WrittenInfomationItem.cs`
- **Dependencies:** No new NuGet packages required; uses existing `System.Threading`, `System.ComponentModel.BackgroundWorker`, and `System.Collections.Generic.HashSet<T>`
- **Behavioral impact:** Search results and view content remain identical; only execution path changes
- **Breaking changes:** None — public interfaces and plugin API surface unchanged
