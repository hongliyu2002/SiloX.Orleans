using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Snacks;

/// <summary>
///     A query for a list of snacks with searching feature.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackRetrieverSearchingListQuery
    (string? SearchCriteria,
     Int32Range? MachineCountRange,
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
     string OperatedBy) : DomainSearchingListQuery(SearchCriteria, Sortings, TraceId, OperatedAt, OperatedBy);
