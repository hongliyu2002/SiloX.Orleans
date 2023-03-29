﻿using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineInsertMoneyCommand(Money Money, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackMachineCommand(TraceId, OperatedAt, OperatedBy);
