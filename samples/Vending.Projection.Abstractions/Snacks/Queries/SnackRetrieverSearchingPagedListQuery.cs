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
     int? SkipCount,
     int? MaxResultCount,
     IDictionary<string, bool>? OrderBy,
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
     bool? IsDeleted = false) : DomainSearchingPagedListQuery(SearchTerm, SkipCount, MaxResultCount, OrderBy, TraceId, OperatedAt, OperatedBy);