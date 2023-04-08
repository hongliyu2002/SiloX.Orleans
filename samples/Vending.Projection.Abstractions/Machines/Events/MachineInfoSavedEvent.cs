namespace Vending.Projection.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineInfoSavedEvent
    (Guid MachineId,
     int Version,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineInfoEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy);
