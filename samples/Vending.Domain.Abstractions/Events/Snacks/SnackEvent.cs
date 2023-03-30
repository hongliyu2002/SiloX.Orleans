﻿using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record SnackEvent(Guid SnackId, int Version, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : DomainEvent(Version, TraceId, OperatedAt, OperatedBy);
