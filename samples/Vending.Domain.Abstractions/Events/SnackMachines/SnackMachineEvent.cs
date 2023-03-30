﻿using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record SnackMachineEvent(Guid MachineId, int Version, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : DomainEvent(Version, TraceId, OperatedAt, OperatedBy);
