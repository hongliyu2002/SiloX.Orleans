namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineBoughtCountUpdatedEvent
    (Guid MachineId,
     int Count,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineEvent(MachineId, 0, TraceId, OperatedAt, OperatedBy);
