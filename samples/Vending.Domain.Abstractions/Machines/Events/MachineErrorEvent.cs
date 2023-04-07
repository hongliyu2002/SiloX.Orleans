using System.Collections.Immutable;
using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineErrorEvent
    (Guid MachineId,
     int Version,
     int Code,
     IImmutableList<string> Reasons,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy), IDomainErrorEvent;
