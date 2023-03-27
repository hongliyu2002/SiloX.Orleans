using System.Collections.Immutable;
using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackErrorEvent(Guid Id, int Version, int Code, IImmutableList<string> Reasons, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : SnackEvent(Id, Version, TraceId, OperatedAt, OperatedBy), IDomainErrorEvent;
