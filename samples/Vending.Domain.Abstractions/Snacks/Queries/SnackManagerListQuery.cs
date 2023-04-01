using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Snacks;

/// <summary>
///     A query for a list of snacks.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackManagerListQuery
    (DateTimeOffsetRange? CreatedAtRange,
     string? CreatedBy,
     DateTimeOffsetRange? LastModifiedAtRange,
     string? LastModifiedBy,
     DateTimeOffsetRange? DeletedAtRange,
     string? DeletedBy,
     bool? IsDeleted,
     IDictionary<string, bool>? Sortings,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainListQuery(Sortings, TraceId, OperatedAt, OperatedBy);
