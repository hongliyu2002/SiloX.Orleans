﻿using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Purchases;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseErrorEvent
    (Guid PurchaseId,
     Guid MachineId,
     int Position,
     Guid SnackId,
     int Code,
     IList<string> Reasons,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : PurchaseEvent(PurchaseId, MachineId, Position, SnackId, TraceId, OperatedAt, OperatedBy), IDomainErrorEvent;
