using System.Collections.Immutable;
using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Queries;

/// <summary>
///     This class represents a query for retrieving a paged list of snack machines.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachinePagedListQuery(DateTimeOffsetRange? CreatedAtRange, string? CreatedBy, DateTimeOffsetRange? LastModifiedAtRange, string? LastModifiedBy, DateTimeOffsetRange? DeletedAtRange, string? DeletedBy, bool? IsDeleted, int SkipCount,
                                                int MaxResultCount, IImmutableList<KeyValuePair<string, bool>>? Sortings, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : DomainPagedListQuery(SkipCount, MaxResultCount, Sortings, TraceId, OperatedAt, OperatedBy);
