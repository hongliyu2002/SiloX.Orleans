﻿namespace Vending.Domain.Abstractions.SnackMachines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineMoneyReturnedEvent
    (Guid MachineId,
     int Version,
     Money MoneyInside,
     decimal AmountInTransaction,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackMachineEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy);