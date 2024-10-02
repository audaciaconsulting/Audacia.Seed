namespace Audacia.Seed.Extensions;

/// <summary>
/// Extensions for the <see cref="Dictionary{TKey,TValue}"/> type.
/// </summary>
internal static class DictionaryExtensions
{
    /// <summary>
    /// Get the value from the dictionary if it exists, otherwise return the default value.
    /// </summary>
    /// <param name="dictionary">The dictionary to check.</param>
    /// <param name="key">The key to look up.</param>
    /// <typeparam name="TKey">The type of the key of the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the value to return.</typeparam>
    /// <returns>The value of the dictionary entry, or the default of <typeparamref name="TValue"/>.</returns>
    public static TValue SafeGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull
    {
        return dictionary.SafeGetValue(key, () => default!);
    }

    /// <summary>
    /// Get the value from the dictionary if it exists, otherwise return the default value.
    /// </summary>
    /// <param name="dictionary">The dictionary to check.</param>
    /// <param name="key">The key to look up.</param>
    /// <param name="getDefault">The default value to add to the dictionary, if the key doesn't exist.</param>
    /// <typeparam name="TKey">The type of the key of the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the value to return.</typeparam>
    /// <returns>The value of the dictionary entry, or the invocation of <paramref name="getDefault"/>.</returns>
    public static TValue SafeGetValue<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TValue> getDefault) where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        var newValue = getDefault();
        dictionary.Add(key, newValue);
        return newValue;
    }
}