﻿using System.Collections.Immutable;
using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseErrorEvent(string PurchaseId, Guid MachineId, int Position, Guid SnackId, int Code, IImmutableList<string> Reasons, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : PurchaseEvent(PurchaseId, MachineId, Position, SnackId, TraceId, OperatedAt, OperatedBy), IDomainErrorEvent;