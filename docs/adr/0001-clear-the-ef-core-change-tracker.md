# 1. Clear the EF Core change tracker

Date: 2024-11-05

## Status

2024-11-05 accepted

## Context

NOTE: the entirety of this document applies to EF Core functionality only, and is not the case for the `.EntityFramework` package.

When setting up data for a test, an EF Core database context remembers that entity (and any navigation properties it has populated) by persiting it in its change tracker in-memory.

This means that if you set up data for a test and immediately execute the test target with the same `DbContext` instance, you're executing the test with a non-empty change tracker and therefore you test doesn't have the same clean slate that it does when executing the test target in production.

This can, and has, lead to false positives in unit tests - meaning the unit test passes when running it, but manually testing the same functionality throws an error. 

An example of this is when you don't do an explicit `Include` of a navigation property which has been set up in the test. In this scenario, the entity and it's navigation property is in the change tracker. In production it's not, and will null ref.

Previously, we have reloaded (`dbContext.Entry.Reload()`) the entity, but have now removed this as it also reloads the navigation properties, which gives us the same issue as above.

For avoidance of doubt, when using EF Core's in-memory provider for unit tests clearing the change tracker won't help you spot false positives for missing `Include`s. If using a SQL DB, it's recommended you use a real SQL db for unit tests to minimize this risk.

## Decision

After every save to the EF Core database, we will clear the change tracker.

## Consequences

1. More production-like unit tests, minimizing the number of false-positives.
1. The returned entities are not tracked by the change tracker, so subsequent changes to them won't be persisted (this is made clear in the XML comments).