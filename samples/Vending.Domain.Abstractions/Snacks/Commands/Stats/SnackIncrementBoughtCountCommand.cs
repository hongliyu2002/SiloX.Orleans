﻿namespace Vending.Domain.Abstractions.Snacks;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackIncrementBoughtCountCommand
    (int Number,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackCommand(TraceId, OperatedAt, OperatedBy);
