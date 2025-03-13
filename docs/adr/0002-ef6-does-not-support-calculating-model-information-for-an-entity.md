# 2. EF6 does not support calculating model information for an entity

Date: 2025-03-07

## Status

2025-03-07 accepted

## Context

When there isn't a `EntitySeed<T>` for a given `T`, we need to do our best to construct a valid instance of `T` automatically. Constructing a valid `T` also includes determining which navigation properties are required, and which aren't.

It might be possible to infer this using the nullability of the navigation property, however not all consumers will have nullable reference types enabled.

For EF Core, we can use the [IModel interface](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.metadata.imodel), to determine exactly what navigation properties are required for saving.

Conversely, there is no known equivalent for this with EF6 so it is potentially impossible to have this information provided by the EF6 DbContext.

## Decision

We won't support calculating the model information in EF6 due to small number of consumers expected to be on EF6 and the value that it would add.
Instead, EF6 will re-use the [reflection-based seeding](../../README.md#reflection-based-seeding) like with the in-memory seeding.

## Consequences

It's more likely that when using EF6, custom `EntitySeed<T>`s will need to be added in order to prevent issues on save.