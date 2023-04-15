namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineSlotRemovedEvent
    (Guid MachineId,
     int Version,
     MachineSlot Slot,
     int SlotCount,
     int SnackCount,
     int SnackQuantity,
     decimal SnackAmount,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy);
