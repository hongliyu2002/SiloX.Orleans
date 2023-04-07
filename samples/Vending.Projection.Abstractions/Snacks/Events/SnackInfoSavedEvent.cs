﻿namespace Vending.Projection.Abstractions.Snacks;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackInfoSavedEvent
    (Guid SnackId,
     int Version,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackInfoEvent(SnackId, Version, TraceId, OperatedAt, OperatedBy);
