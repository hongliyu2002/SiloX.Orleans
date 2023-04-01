using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.SnackMachines;

/// <summary>
///     A query for retrieving a paged list of snack machines.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineRetrieverPagedListQuery
    (Int32Range? SlotsCountRange,
     Int32Range? SnackCountRange,
     Int32Range? SnackQuantityRange,
     DecimalRange? SnackAmountRange,
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
