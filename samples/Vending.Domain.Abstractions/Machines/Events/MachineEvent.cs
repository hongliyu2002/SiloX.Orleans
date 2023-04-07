using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record MachineEvent
    (Guid MachineId,
     int Version,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainEvent(Version, TraceId, OperatedAt, OperatedBy);
