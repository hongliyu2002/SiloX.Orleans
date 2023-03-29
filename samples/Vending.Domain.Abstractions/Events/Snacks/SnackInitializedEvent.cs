﻿namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackInitializedEvent(Guid SnackId, int Version, string Name, string? PictureUrl, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : SnackEvent(SnackId, Version, TraceId, OperatedAt, OperatedBy);
