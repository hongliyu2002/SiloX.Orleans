using System.Collections.Immutable;

namespace SiloX.Domain.Abstractions;

/// <summary>
///     An abstract class that represents a query for a domain list with searching feature.
///     It contains search criteria, sorting criteria and traceability information.
/// </summary>
/// <param name="SearchTerm">The criteria for searching.</param>
/// <param name="OrderBy">The sorting criteria.</param>
/// <param name="TraceId">The unique identifier for the trace.</param>
/// <param name="OperatedAt">The date and time when the operation was performed.</param>
/// <param name="OperatedBy">The name of the operator who performed the operation.</param>
[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record DomainSearchingListQuery
    (string? SearchTerm,
     IDictionary<string, bool>? OrderBy,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainListQuery(OrderBy, TraceId, OperatedAt, OperatedBy);
