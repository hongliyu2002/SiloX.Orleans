namespace Vending.Domain.Abstractions.SnackMachines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineMoneyLoadedEvent
    (Guid MachineId,
     int Version,
     Money MoneyInside,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackMachineEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy);
