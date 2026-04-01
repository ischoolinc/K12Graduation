## ADDED Requirements

### Requirement: Views load graduate data on a background thread
When a view's `SourceChanged` event fires, the system SHALL fetch graduate UDT records on a `BackgroundWorker` thread rather than the UI thread, so the application remains responsive during data retrieval.

#### Scenario: View data loads without blocking UI
- **WHEN** the user selects a filter or the active student list changes
- **THEN** the view SHALL initiate a `BackgroundWorker` to fetch data and the UI thread SHALL remain unblocked

#### Scenario: Stale background worker is cancelled before new load
- **WHEN** `SourceChanged` fires while a previous background load is still in progress
- **THEN** the previous `BackgroundWorker` SHALL be cancelled before a new one is started

#### Scenario: Completed load is discarded if cancelled
- **WHEN** a background load completes but was cancelled (e.g., selection changed again)
- **THEN** the view SHALL not update its tree or list with the stale result

### Requirement: Tree node construction is batched
When a view rebuilds its tree from loaded data, the system SHALL call `AdvTree.BeginUpdate()` before adding nodes and `AdvTree.EndUpdate()` after all nodes are added, preventing per-node redraws.

#### Scenario: Tree builds without per-node repaint
- **WHEN** a view constructs a tree with 100 or more nodes
- **THEN** the control SHALL not repaint between individual `Nodes.Add()` calls

### Requirement: View deduplication uses HashSet
When a view builds its internal UID or category lists, the system SHALL use `HashSet<string>` for deduplication instead of `List<string>.Contains()`, ensuring O(1) lookup complexity.

#### Scenario: Dedup does not degrade with dataset size
- **WHEN** a view processes 10,000 graduate records
- **THEN** deduplication SHALL complete in O(n) time with no observable quadratic slowdown
