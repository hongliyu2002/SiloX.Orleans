using System.Collections.Immutable;

namespace SiloX.Domain.Abstractions;

/// <summary>
///     An abstract class that represents a query for a domain list.
///     It contains sorting criteria and traceability information.
/// </summary>
/// <param name="Sortings">TAn immutable list of key-value pairs representing the sorting criteria.</param>
/// <param name="TraceId">The unique identifier for the trace.</param>
/// <param name="OperatedAt">The date and time when the operation was performed.</param>
/// <param name="OperatedBy">The name of the operator who performed the operation.</param>
[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record DomainListQuery(IImmutableList<KeyValuePair<string, bool>> Sortings, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : DomainQuery(TraceId, OperatedAt, OperatedBy);
