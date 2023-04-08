using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record MachineInfoEvent
    (Guid MachineId,
     int Version,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainEvent(Version, TraceId, OperatedAt, OperatedBy);
