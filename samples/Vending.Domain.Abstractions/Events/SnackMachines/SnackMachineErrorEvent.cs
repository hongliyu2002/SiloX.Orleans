using System.Collections.Immutable;
using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineErrorEvent(Guid Id, int Version, int Code, IImmutableList<string> Reasons, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : SnackMachineEvent(Id, Version, TraceId, OperatedAt, OperatedBy), IDomainErrorEvent;
