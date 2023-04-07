using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Machines;

/// <summary>
///     A query for a list of machines.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineRetrieverListQuery
    (DecimalRange? MoneyInsideAmountRange,
     DecimalRange? AmountInTransactionRange,
     Int32Range? SlotsCountRange,
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
     IDictionary<string, bool>? Sortings,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainListQuery(Sortings, TraceId, OperatedAt, OperatedBy);
