## ADDED Requirements

### Requirement: Search executes in a single dictionary pass
The system SHALL search all six graduate fields (Name, StudentNumber, IDNumber, ClassName, Address, Remarks) in a single iteration over `TestDic`, rather than performing a separate full iteration per field.

#### Scenario: Single-pass search completes faster than six-pass
- **WHEN** the user enters a search term against a dataset of 10,000 graduates
- **THEN** the search SHALL iterate `TestDic` exactly once, checking all fields per entry

#### Scenario: Search results are identical to pre-optimization
- **WHEN** a search term matches graduates across multiple fields
- **THEN** the result set SHALL contain the same UIDs as the previous six-loop implementation

### Requirement: Search result deduplication uses HashSet
The system SHALL accumulate search results in a `HashSet<string>` to guarantee O(1) duplicate checks, replacing the previous `List<string>` with O(n) `Contains()` calls.

#### Scenario: No duplicate UIDs in search results
- **WHEN** a graduate's Name AND StudentNumber both match the search term
- **THEN** that graduate's UID SHALL appear exactly once in the result set

#### Scenario: HashSet dedup does not degrade with result size
- **WHEN** a broad search term matches 5,000 of 10,000 graduates
- **THEN** deduplication overhead SHALL remain O(1) per result entry

### Requirement: Regex is compiled once per search invocation
The system SHALL construct a single `Regex` object before the search loop and reuse it for all field matches, rather than compiling a new pattern per field or per entry.

#### Scenario: Regex compiled once per search call
- **WHEN** a search is executed over 10,000 graduates across 6 fields
- **THEN** the `Regex` SHALL be instantiated exactly once per search invocation
