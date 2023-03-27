﻿namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineRemovedEvent(Guid Id, int Version, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackMachineEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
