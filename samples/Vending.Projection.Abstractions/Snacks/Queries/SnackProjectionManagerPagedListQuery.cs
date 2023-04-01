using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Snacks;

/// <summary>
///     This class represents a query for retrieving a paged list of snacks.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackProjectionManagerPagedListQuery
    (Int32Range? MachineCountRange,
     Int32Range? BoughtCountRange,
     DecimalRange? BoughtAmountRange,
     DateTimeOffsetRange? CreatedAtRange,
     string? CreatedBy,
     DateTimeOffsetRange? LastModifiedAtRange,
     string? LastModifiedBy,
     DateTimeOffsetRange? DeletedAtRange,
     string? DeletedBy,
     bool? IsDeleted,
     int? SkipCount,
     int? MaxResultCount,
     IDictionary<string, bool>? Sortings,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainPagedListQuery(SkipCount, MaxResultCount, Sortings, TraceId, OperatedAt, OperatedBy);
