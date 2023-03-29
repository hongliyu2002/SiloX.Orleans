﻿namespace Vending.Domain.Abstractions.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineRemoveCommand(Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) : SnackMachineCommand(TraceId, OperatedAt, OperatedBy);
