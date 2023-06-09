﻿using System.Collections.Immutable;
using Orleans.FluentResults;

namespace SiloX.Domain.Abstractions.Extensions;

/// <summary>
///     Provides extension methods for <see cref="IEnumerable{T}" /> objects.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    ///     Converts a collection of <see cref="IError" /> objects into a concatenated string of error messages.
    /// </summary>
    /// <param name="errors">The collection of <see cref="IError" /> objects to convert.</param>
    /// <returns>A concatenated string of error messages.</returns>
    public static string ToMessage(this IEnumerable<IError> errors)
    {
        return string.Join(';', errors.OfType<Error>().Select(error => error.ToString()));
    }

    /// <summary>
    ///     Converts a collection of <see cref="IError" /> objects into an immutable list of error messages.
    /// </summary>
    /// <param name="errors">The collection of <see cref="IError" /> objects to convert.</param>
    /// <returns>An immutable list of error messages.</returns>
    public static List<string> ToListMessages(this IEnumerable<IError> errors)
    {
        return errors.OfType<Error>().Select(error => error.ToString()).ToList();
    }
    
    /// <summary>
    ///     Converts a collection of <see cref="IError" /> objects into an immutable list of error messages.
    /// </summary>
    /// <param name="errors">The collection of <see cref="IError" /> objects to convert.</param>
    /// <returns>An immutable list of error messages.</returns>
    public static ImmutableList<string> ToImmutableListMessages(this IEnumerable<IError> errors)
    {
        return errors.OfType<Error>().Select(error => error.ToString()).ToImmutableList();
    }

    /// <summary>
    ///     Converts a collection of key-value pairs into a string representing the sorting criteria for a query.
    /// </summary>
    /// <param name="orderBy">The collection of key-value pairs representing the sorting criteria.</param>
    /// <returns>A string representing the sorting criteria for a query.</returns>
    public static string ToOrderByString(this IEnumerable<KeyValuePair<string, bool>> orderBy)
    {
        return string.Join(',', orderBy.Select(x => $"{x.Key}{(x.Value ? " DESC" : "")}"));
    }
    
    /// <summary>
    ///     Converts a collection of key-value pairs into a string representing the sorting criteria for a query.
    /// </summary>
    /// <param name="orderBy">The collection of key-value pairs representing the sorting criteria.</param>
    /// <returns>A string representing the sorting criteria for a query.</returns>
    public static string ToOrderByString(this IDictionary<string, bool> orderBy)
    {
        return string.Join(',', orderBy.Select(x => $"{x.Key}{(x.Value ? " DESC" : "")}"));
    }
}
