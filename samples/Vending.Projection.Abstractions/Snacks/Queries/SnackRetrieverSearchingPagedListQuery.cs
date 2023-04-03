using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Snacks;

/// <summary>
///     A query for retrieving a paged list of snacks with searching feature.
/// </summary>
[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackRetrieverSearchingPagedListQuery
    (string? SearchTerm,
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
     int? SkipCount,
     int? MaxResultCount,
     IDictionary<string, bool>? Sortings,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainSearchingPagedListQuery(SearchTerm, SkipCount, MaxResultCount, Sortings, TraceId, OperatedAt, OperatedBy);
