# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade `src\Audacia.Seed\Audacia.Seed.csproj`
4. Upgrade `tests\helpers\Audacia.Seed.Tests.ExampleProject\Audacia.Seed.Tests.ExampleProject.csproj`
5. Upgrade `src\Audacia.Seed.EntityFrameworkCore\Audacia.Seed.EntityFrameworkCore.csproj`
6. Upgrade `src\Audacia.Seed.EntityFramework\Audacia.Seed.EntityFramework.csproj`
7. Upgrade `tests\Audacia.Seed.Tests\Audacia.Seed.Tests.csproj`

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name | Description |
|:------------|:-----------:|

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                           | Current Version | New Version | Description                      |
|:---------------------------------------|:---------------:|:-----------:|:---------------------------------|
| EntityFramework                        |     6.4.4       |   6.5.1     | Deprecated / recommended upgrade |
| Microsoft.EntityFrameworkCore          |      8.0.0      |   10.0.2    | Recommended for .NET 10.0        |
| Microsoft.EntityFrameworkCore.Relational|     8.0.0      |   10.0.2    | Recommended for .NET 10.0        |
| Microsoft.EntityFrameworkCore.SqlServer|     8.0.0       |   10.0.2    | Recommended for .NET 10.0        |

### Project upgrade details

#### `src\Audacia.Seed\Audacia.Seed.csproj` modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### `tests\helpers\Audacia.Seed.Tests.ExampleProject\Audacia.Seed.Tests.ExampleProject.csproj` modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore should be updated from `8.0.0` to `10.0.2` (*recommended for .NET 10.0*)

#### `src\Audacia.Seed.EntityFrameworkCore\Audacia.Seed.EntityFrameworkCore.csproj` modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore should be updated from `8.0.0` to `10.0.2` (*recommended for .NET 10.0*)
  - Microsoft.EntityFrameworkCore.Relational should be updated from `8.0.0` to `10.0.2` (*recommended for .NET 10.0*)

#### `src\Audacia.Seed.EntityFramework\Audacia.Seed.EntityFramework.csproj` modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - EntityFramework should be updated from `6.4.4` to `6.5.1` (*deprecated / recommended upgrade*)

#### `tests\Audacia.Seed.Tests\Audacia.Seed.Tests.csproj` modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore.SqlServer should be updated from `8.0.0` to `10.0.2` (*recommended for .NET 10.0*)
