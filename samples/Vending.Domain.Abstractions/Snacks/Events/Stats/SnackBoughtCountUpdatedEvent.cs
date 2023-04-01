﻿namespace Vending.Domain.Abstractions.Snacks;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackBoughtCountUpdatedEvent
    (Guid SnackId,
     int Count,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackEvent(SnackId, 0, TraceId, OperatedAt, OperatedBy);
