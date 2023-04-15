namespace SiloX.Domain.Abstractions.Extensions;

/// <summary>
///     Extension methods for <see cref="Dictionary{TKey,TValue}" />.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    ///     Returns a dictionary that contains the elements that are present in the first dictionary, but not in the second dictionary.
    /// </summary>
    /// <param name="first">The first dictionary to compare.</param>
    /// <param name="second">The second dictionary to compare.</param>
    /// <param name="keyOnly">If true, the comparison is done only on the keys. If false, the comparison is done on the keys and values.</param>
    /// <typeparam name="TKey">The type of the keys in the dictionaries.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionaries. The value type must be a reference type.</typeparam>
    /// <returns>A dictionary that contains the elements that are present in the first dictionary, but not in the second dictionary.</returns>
    public static Dictionary<TKey, TValue> Except<TKey, TValue>(this Dictionary<TKey, TValue> first, Dictionary<TKey, TValue> second, bool keyOnly = false)
        where TKey : notnull
    {
        var result = new Dictionary<TKey, TValue>(first.Count);
        if (keyOnly)
        {
            foreach (var kvp in first.Where(kvp => !second.ContainsKey(kvp.Key)))
            {
                result.Add(kvp.Key, kvp.Value);
            }
        }
        else
        {
            foreach (var kvp in first.Where(kvp => !second.Contains(kvp)))
            {
                result.Add(kvp.Key, kvp.Value);
            }
        }
        return result;
    }
}