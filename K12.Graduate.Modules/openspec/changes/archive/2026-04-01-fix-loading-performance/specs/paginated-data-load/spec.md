## ADDED Requirements

### Requirement: Graduate record queries are bounded by a configurable page size
The system SHALL limit the number of graduate records loaded into `TestDic` per query to a defined maximum (`MaxGraduateLoadCount`), preventing unbounded memory consumption for large archives.

#### Scenario: Load is capped at MaxGraduateLoadCount
- **WHEN** the matching graduate count exceeds `MaxGraduateLoadCount`
- **THEN** at most `MaxGraduateLoadCount` records SHALL be loaded into memory

#### Scenario: User is notified of truncation
- **WHEN** the loaded result is truncated due to the page size limit
- **THEN** the UI SHALL display a status message indicating that results are limited and suggesting the user refine their filter

#### Scenario: Full result loads when within limit
- **WHEN** the matching graduate count is less than or equal to `MaxGraduateLoadCount`
- **THEN** all matching records SHALL be loaded without truncation or notification

### Requirement: Detail view fetches related records without a second round-trip
When a `GraduateDetailItem` or `WrittenInfomationItem` loads a graduate record, the system SHALL fetch the primary record and its related record (photo or written information) using a single batch query or the minimum number of queries, eliminating the N+1 pattern.

#### Scenario: Detail load uses at most two queries
- **WHEN** the user opens a graduate detail panel
- **THEN** the detail item SHALL issue at most two `Select<T>` calls to retrieve both the graduate record and any related UDT record

#### Scenario: Missing related record does not cause additional query
- **WHEN** no related record exists for a graduate (e.g., no photo on file)
- **THEN** the system SHALL not issue a fallback or retry query for the missing record
