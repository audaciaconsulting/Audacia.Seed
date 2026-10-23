# Changelog

## 3.0.4 - 2026-10-23

### Changed

- Enabled TreatWarningsAsErrors on all projects, to ensure all warnings are handled immediately. (#183182).
- Refactored to remove warnings in the codebase, avoiding warnings that will present as errors. (#183182).
- Updated Audacia.CodeAnalysis to version 2.1.0, to get the latest code analysis rules and fixes. (#183182).
- Moved common project tags to Directory.Build.props, in line with other public multiple-package solutions. (#183182).

### Fixed

- Bug Fix: `.With()` ignored multiple values for properties reached via a cast, e.g. `x => ((EmployeeAsset)x.Asset).Employee` (#214683).
- Bug Fix: `.With()` on a property of a navigation supplied via `.WithNew()` seeded an extra, orphaned prerequisite (#215442).
- Bug Fix: Multiple seeds supplied via `.WithNew()` were not preserved when `.With()` was used on a property of that navigation (#215442).
- Bug Fix: A derived seed via `.WithNew()`, e.g. `EntitySeed<EmployeeAsset>`, was discarded when `.With()` was used on a property of that navigation (#215442).
- Bug Fix: A seed supplied via `.WithNew()` didn't populate its own prerequisites when built later, if `.With()` had set a property of that navigation (#215442).
- Bug Fix: Entities could be added to the in-memory repository multiple times, when using `Build` methods (#215443).
- Bug Fix: `.WithDifferent()` on a nested navigation changed the seed's options, so it no longer reused entities in other navigations or later builds (#215444).

## 3.0.3 - 2026-10-07

### Fixed

- Bug Fix: Fixed expression-chain composition losing preceding member accesses when a cast occurs mid-chain (#214579).

### Changed

- Replaced FluentAssertions with Shouldly in the test project. (#214579).

## 3.0.2 - 2026-10-05

### Changed

- Updated FluentAssertions to version 6.12.2
- Updated Microsoft.EntityFrameworkCore to version 10.0.12
- Updated Microsoft.EntityFrameworkCore.Relational to version 10.0.12
- Updated Microsoft.EntityFrameworkCore.SqlServer to version 10.0.12
- Updated Microsoft.NET.Test.Sdk to version 18.10.1

## 3.0.1 - 2026-05-28

### Changed

- Updated Audacia Dependecies to .NET 10.0 compatible versions
- Replaced XUnit with XUnit v3 in the test project.

## 3.0.0 - 2026-02-05

### Changed

- Updated target framework to .NET 10.0

## 2.0.3 - 2025-02-13

### Fixed

- EF Core only: Clear the change tracker when seeding begins to ensure WithNew is respected (#190707).
- Fix issue where the CHANGELOG & README files were appearing in consuming projects (#188898).

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
