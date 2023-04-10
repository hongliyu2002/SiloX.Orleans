using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Machines;

/// <summary>
///     A query for a list of machines.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineRetrieverListQuery
    (IDictionary<string, bool>? OrderBy,
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
     bool? IsDeleted = false) : DomainListQuery(OrderBy, TraceId, OperatedAt, OperatedBy);