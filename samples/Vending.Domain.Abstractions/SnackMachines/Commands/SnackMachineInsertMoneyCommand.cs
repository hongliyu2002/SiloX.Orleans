﻿namespace Vending.Domain.Abstractions.SnackMachines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineInsertMoneyCommand
    (Money Money,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackMachineCommand(TraceId, OperatedAt, OperatedBy);