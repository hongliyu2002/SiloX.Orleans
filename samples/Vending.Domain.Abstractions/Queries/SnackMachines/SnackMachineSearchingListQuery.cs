using System.Collections.Immutable;
using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Queries;

/// <summary>
///     A class that represents a query for a snack machines with searching feature.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineSearchingListQuery(string? SearchCriteria, DateTimeOffsetRange? CreatedAtRange, string? CreatedBy, DateTimeOffsetRange? LastModifiedAtRange, string? LastModifiedBy, DateTimeOffsetRange? DeletedAtRange, string? DeletedBy,
                                                    bool? IsDeleted, IImmutableList<KeyValuePair<string, bool>>? Sortings, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : DomainSearchingListQuery(SearchCriteria, Sortings, TraceId, OperatedAt, OperatedBy);
