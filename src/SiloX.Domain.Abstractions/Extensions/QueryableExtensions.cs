using System.Collections.Immutable;

namespace SiloX.Domain.Abstractions.Extensions;

/// <summary>
///     Provides extension methods for <see cref="IQueryable{T}" />.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    ///     Asynchronously converts an <see cref="IQueryable{T}" /> sequence to an immutable list.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the sequence.</typeparam>
    /// <param name="queryable">The <see cref="IQueryable{T}" /> to convert to an immutable list.</param>
    /// <param name="cancellationToken">The cancellation token that can be used to cancel the operation.</param>
    /// <returns>An immutable list that contains the elements of the input sequence.</returns>
    public static async Task<ImmutableList<TSource>> ToImmutableListAsync<TSource>(this IQueryable<TSource> queryable, CancellationToken cancellationToken = default)
    {
        var result = ImmutableList.CreateBuilder<TSource>();
        await foreach (var element in queryable.ToAsyncEnumerable().WithCancellation(cancellationToken))
        {
            result.Add(element);
        }
        return result.ToImmutable();
    }
}
