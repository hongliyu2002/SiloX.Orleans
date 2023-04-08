using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineInfoErrorEvent
    (Guid MachineId,
     int Version,
     int Code,
     IList<string> Reasons,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineInfoEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy), IDomainErrorEvent;
