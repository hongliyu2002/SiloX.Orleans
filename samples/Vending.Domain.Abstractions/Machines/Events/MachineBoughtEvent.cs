namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineBoughtEvent
    (Guid MachineId,
     int Version,
     decimal AmountInTransaction,
     Slot Slot,
     int SnackQuantity,
     decimal SnackAmount,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy);
