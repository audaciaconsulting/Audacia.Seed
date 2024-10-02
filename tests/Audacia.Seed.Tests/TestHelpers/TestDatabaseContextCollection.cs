using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Audacia.Seed.Tests.TestHelpers;

[SuppressMessage("Naming", "CA1711: Identifiers should not have incorrect suffix",
    Justification = "'Collection' is meaningful in this context.")]
[CollectionDefinition(CollectionNames.TestDatabaseContextCollection)]
public class TestDatabaseContextCollection : ICollectionFixture<TestDatabaseContextFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}