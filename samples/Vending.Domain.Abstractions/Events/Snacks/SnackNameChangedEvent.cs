﻿namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackNameChangedEvent(Guid SnackId, int Version, string Name, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackEvent(SnackId, Version, TraceId, OperatedAt, OperatedBy);
