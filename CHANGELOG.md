# Changelog

## 2.0.2 - 2024-11-05
### Fixed
- Bug Fix: don't reload entities after saving them to protect against false-positives in unit tests (#182829).

## 2.0.1 - 2024-10-30
### Fixed
- Bug Fix: entities not saved when seeding many using the `IEntitySeed.PerformSeeding` method (#182830).

## 2.0.0 - 2024-09-24
### Added
- Complete rewrite of the project based on existing need, and lack of uptake on the current project.
- Support for EF Core, EF6, and in-memory seeding 

### Changed
- All existing functionality removed:
   - `web.confg` based configuration not supported.
   - `Autofixture` reference removed.

## 1.1.0 - 2023-10-05
### Added
- No new functionality added

### Changed
- Prepare codebase for GitHub migration

## 1.0.0 - 2020-04-30
### Added
- No new functionality added

### Changed
- Changed build pipeline name to date format

## 0.1.0 - 2019-09-16
### Added
- Added stylecop static analysis

### Changed
- Updated EF6 project format 
- Simplified build pipeline