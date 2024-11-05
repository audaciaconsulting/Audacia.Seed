using System.Linq.Expressions;
using System.Reflection;
using Audacia.Seed.Constants;
using Audacia.Seed.Exceptions;

namespace Audacia.Seed.Extensions;

/// <summary>
/// Extensions for <see cref="Type"/>.
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    /// Gets the properties on the provided <paramref name="type"/> that are required navigation properties.
    /// <br/>
    /// This is based on a naming convention of MyNavigation property having a corresponding MyNavigationId property.
    /// </summary>
    /// <param name="type">The type to find the required navigation properties for.</param>
    /// <returns>The properties on the provided <paramref name="type"/> that are required navigation properties.</returns>
    public static IEnumerable<PropertyInfo> GetRequiredNavigationProperties(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        const string foreignKeySuffix = SeedingConstants.ForeignKeySuffix;

        // Get a groups of properties that have the same name, except for the foreign key suffix.
        var foreignKeyGroups = type.GetProperties()
            .GroupBy(p => p.Name.EndsWith(foreignKeySuffix)
                ? p.Name[..^foreignKeySuffix.Length]
                : p.Name)
            .Where(p => p.Count() == 2);

        return ForeignKeyGroupsIterator();

        IEnumerable<PropertyInfo> ForeignKeyGroupsIterator()
        {
            foreach (var foreignKeyGroup in foreignKeyGroups)
            {
                var foreignKeyProperties = foreignKeyGroup.ToArray();
                var foreignKeyProperty = foreignKeyProperties.First(p => p.Name.EndsWith(foreignKeySuffix));
                var navigationProperty = foreignKeyProperties.First(p => !p.Name.EndsWith(foreignKeySuffix));
                var isRequiredNavigation = Nullable.GetUnderlyingType(foreignKeyProperty.PropertyType) == null;
                if (isRequiredNavigation && navigationProperty.PropertyType.IsClass)
                {
                    yield return navigationProperty;
                }
            }
        }
    }

    /// <summary>
    /// Get an example value for the provided <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type to get an example value of.</param>
    /// <returns>An example value for <paramref name="type"/>.</returns>
    /// <exception cref="DataSeedingException">If no example value can be found.</exception>
    public static object ExampleValue(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (type == typeof(string))
        {
            return Guid.NewGuid().ToString();
        }

        Expression<Func<object>> getDefaultValueExpression = Expression.Lambda<Func<object>>(
            Expression.Convert(
                Expression.Default(type), typeof(object)));

        var value = getDefaultValueExpression.Compile()();

        return value ?? throw new DataSeedingException($"Unable to get the default value of type {type.Name}.");
    }

    /// <summary>
    /// Format the provided <paramref name="type"/> name, including details of any generic arguments.
    /// </summary>
    /// <param name="type">The type to format.</param>
    /// <returns>A formatted display name to use in developer messages.</returns>
    public static string GetFormattedName(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (type.IsGenericType)
        {
            string genericArguments = type.GetGenericArguments()
                .Select(x => x.Name)
                .Aggregate((first, second) => $"{first}, {second}");
            return $"{type.Name.Substring(0, type.Name.IndexOf("`", StringComparison.Ordinal))}<{genericArguments}>";
        }

        return type.Name;
    }
}