﻿namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineDeletedEvent
    (Guid MachineId,
     int Version,
     Money MoneyInside,
     decimal AmountInTransaction,
     IList<MachineSlot> Slots,
     int SlotCount,
     int SnackCount,
     int SnackQuantity,
     decimal SnackAmount,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy);
