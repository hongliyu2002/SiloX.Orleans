namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineMoneyLoadedEvent
    (Guid MachineId,
     int Version,
     Money MoneyInside,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy);
