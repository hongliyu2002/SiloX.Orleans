﻿namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackRemovedEvent(Guid Id, int Version, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackEvent(Id, Version, TraceId, OperatedAt, OperatedBy);