namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineMoneyReturnedEvent
    (Guid MachineId,
     int Version,
     Money MoneyInside,
     decimal AmountInTransaction,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy);
