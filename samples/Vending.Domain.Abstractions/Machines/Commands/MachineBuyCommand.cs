﻿namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineBuyCommand
    (int Position,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineCommand(TraceId, OperatedAt, OperatedBy);
