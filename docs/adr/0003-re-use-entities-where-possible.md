# 3. Re-use entities where possible

Date: 2025-03-07

## Status

2025-03-07 accepted

## Context

As per the README, one of the main aims for this library is "conservative seeding". One of the main drawbacks when using libraries like AutoFixture is that it will "explode" out when seeding.

The library that was used as a predecessor to this did two things:
- Saved per entity being created.
- Created a new parent for every child when seeding multple.

As such, we experienced performance issues which were exasperated when seeding low-down entities (i.e with many parents, grandparents, etc). The time it took to seed these entities grew exponentially the lower down you went due to the amount of saves growing, and the number of entitites being saved.

## Decision

We will re-use unsaved entities when seeding where possible, and save once per seeding action i.e call to `Seed` or `SeedMany`.

When seeding many children, we'll check the database's change tracker before creating a new parent in case an appropriate match exists. 

A `WithDifferent` method would be provided to circumvent this default behaviour.

## Consequences

No known consequences. The purpose of this ADR is to explicitly articulate the approach & reasoning for it.
