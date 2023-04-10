using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Machines;

/// <summary>
///     A query for retrieving a paged list of machines.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineRetrieverPagedListQuery
    (int? SkipCount,
     int? MaxResultCount,
     IDictionary<string, bool>? OrderBy,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy,
     DecimalRange? MoneyInsideAmountRange = null,
     DecimalRange? AmountInTransactionRange = null,
     Int32Range? SlotCountRange = null,
     Int32Range? SnackCountRange = null,
     Int32Range? SnackQuantityRange = null,
     DecimalRange? SnackAmountRange = null,
     Int32Range? BoughtCountRange = null,
     DecimalRange? BoughtAmountRange = null,
     DateTimeOffsetRange? CreatedAtRange = null,
     string? CreatedBy = null,
     DateTimeOffsetRange? LastModifiedAtRange = null,
     string? LastModifiedBy = null,
     DateTimeOffsetRange? DeletedAtRange = null,
     string? DeletedBy = null,
     bool? IsDeleted = false) : DomainPagedListQuery(SkipCount, MaxResultCount, OrderBy, TraceId, OperatedAt, OperatedBy);