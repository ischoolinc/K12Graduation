## 1. Async View Loading — BackgroundWorker Refactor

- [x] 1.1 Add a `BackgroundWorker` field to `TagView` and wire up `DoWork` / `RunWorkerCompleted` handlers
- [x] 1.2 Move the `_AccessHelper.Select<GraduateUDT>()` call in `TagView.SourceChanged` into the BGW `DoWork` handler; build `TagDic` on the background thread
- [x] 1.3 In `TagView.RunWorkerCompleted`, check `e.Cancelled`; if not cancelled, call the existing tree-build method on the UI thread
- [x] 1.4 Cancel and restart the BGW when `SourceChanged` fires while a previous load is in progress (`TagView`)
- [x] 1.5 Apply the same BGW pattern to `ArchiveNoteView` (move `Select<GraduateUDT>` + dict building to DoWork)
- [x] 1.6 Apply the same BGW pattern to `GraduationYearView` (move `Select<GraduateUDT>` + dict building to DoWork)

## 2. Tree Build Performance — BeginUpdate / EndUpdate

- [x] 2.1 Wrap the node-append loop in `TagView.BuildTree()` with `AdvTree.BeginUpdate()` / `AdvTree.EndUpdate()`
- [x] 2.2 Wrap the node-append loop in `ArchiveNoteView.BuildTree()` with `AdvTree.BeginUpdate()` / `AdvTree.EndUpdate()`
- [x] 2.3 Wrap the node-append loop in `GraduationYearView.BuildTree()` with `AdvTree.BeginUpdate()` / `AdvTree.EndUpdate()`

## 3. View Deduplication — Replace List with HashSet

- [x] 3.1 In `TagView`, replace every `List<string> list` used for UID/tag deduplication with `HashSet<string>` (lines ~49, ~119, ~147); remove the `.Contains()` guard; rely on `HashSet.Add()` return value
- [x] 3.2 In `ArchiveNoteView`, replace the `List<string>` dedup in `_Source` building (lines ~178-185) with `TestList.Select(x => x.UID).Distinct().ToList()`
- [ ] 3.3 Verify all three view classes compile and produce the same tree output after HashSet changes

## 4. Search Optimization — Single Pass + HashSet Results

- [x] 4.1 In `GraduationAdmin`, locate the six sequential `foreach (string each in TestDic.Keys)` loops (lines 354-448) and replace with a single loop
- [x] 4.2 Inside the single loop, check all six fields (`Name`, `StudentNumber`, `IDNumber`, `ClassName`, `Address`, `Remarks`) with `rx.IsMatch()` using OR logic
- [x] 4.3 Declare the result accumulator as `HashSet<string> results` instead of `List<string>`; remove the `if (!results.Contains(each))` guard
- [x] 4.4 Construct the `Regex rx` object once before the loop (verify it is not already inside any per-field loop)
- [x] 4.5 Convert `results` to `List<string>` after the loop if the caller (`SetListPaneSource`) requires a `List<T>`

## 5. Paginated Data Load — Bound Graduate Queries

- [x] 5.1 Add `private const int MaxGraduateLoadCount = 2000;` to `GraduationAdmin`
- [x] 5.2 Verify whether `AccessHelper.Select<T>(condition)` supports a SQL `LIMIT` clause directly in the condition string; document the finding in a code comment
- [ ] 5.3 If LIMIT is supported: append `LIMIT {MaxGraduateLoadCount}` to the condition string in the `Select<GraduateUDT>` call (line ~471)
- [x] 5.4 If LIMIT is not supported: trim `TestList` to `MaxGraduateLoadCount` entries after the Select call
- [x] 5.5 After loading, if `TestList.Count == MaxGraduateLoadCount`, display a status-bar or label message: "顯示前 2,000 筆資料，請縮小搜尋範圍以查看更多。"

## 6. N+1 Query Elimination — Detail Views

- [x] 6.1 In `GraduateDetailItem.DoWork`, check whether fetching photo by `RefUDT_ID` can be combined into the graduate query condition; if not, keep as two queries but confirm no additional queries exist
- [x] 6.2 In `WrittenInfomationItem.DoWork`, apply the same review; if the two queries are already the minimum, add a comment documenting why they are separate
- [x] 6.3 Ensure both detail items only issue queries inside `DoWork` (background thread), not in `RunWorkerCompleted` or any UI-thread method

## 7. Verification

- [ ] 7.1 Run the application and load a graduate list with > 500 records; confirm the UI does not freeze during view tab switching
- [ ] 7.2 Run a search with a broad term; confirm results match the pre-optimization behavior
- [ ] 7.3 Open a graduate detail panel; confirm photo and written info load correctly
- [ ] 7.4 Trigger a `SourceChanged` rapidly (switch filters twice quickly); confirm no exception or stale data appears in the tree
- [ ] 7.5 Confirm the truncation message appears when a dataset exceeds `MaxGraduateLoadCount`
