using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Snacks;

/// <summary>
///     A query for a list of snacks.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackRetrieverListQuery
    (IDictionary<string, bool>? OrderBy,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy,
     Int32Range? MachineCountRange = null,
     Int32Range? TotalQuantityRange = null,
     DecimalRange? TotalAmountRange = null,
     Int32Range? BoughtCountRange = null,
     DecimalRange? BoughtAmountRange = null,
     DateTimeOffsetRange? CreatedAtRange = null,
     string? CreatedBy = null,
     DateTimeOffsetRange? LastModifiedAtRange = null,
     string? LastModifiedBy = null,
     DateTimeOffsetRange? DeletedAtRange = null,
     string? DeletedBy = null,
     bool? IsDeleted = false) : DomainListQuery(OrderBy, TraceId, OperatedAt, OperatedBy);