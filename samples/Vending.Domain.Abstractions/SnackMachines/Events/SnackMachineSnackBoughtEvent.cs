namespace Vending.Domain.Abstractions.SnackMachines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineSnackBoughtEvent
    (Guid MachineId,
     int Version,
     decimal AmountInTransaction,
     Slot Slot,
     int SnackQuantity,
     decimal SnackAmount,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackMachineEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy);
