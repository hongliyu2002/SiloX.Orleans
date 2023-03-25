using System.Collections.Immutable;

namespace SiloX.Domain.Abstractions;

/// <summary>
///     An abstract record represents a domain error event.
/// </summary>
/// <param name="Id">The unique identifier of the original object who raises this event.</param>
/// <param name="Version">The version of the domain event.</param>
/// <param name="Code">The error code.</param>
/// <param name="Reasons">List of strings containing the reasons for the error.</param>
/// <param name="TraceId">The unique identifier for the trace.</param>
/// <param name="OperatedAt">The date and time when the operation was performed.</param>
/// <param name="OperatedBy">The name of the operator who performed the operation.</param>
[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record DomainErrorEvent(Guid Id, long Version, int Code, IImmutableList<string> Reasons, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) : DomainEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
