﻿namespace Vending.Domain.Abstractions.SnackMachines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineBoughtCountUpdatedEvent
    (Guid MachineId,
     int Count,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackMachineEvent(MachineId, 0, TraceId, OperatedAt, OperatedBy);