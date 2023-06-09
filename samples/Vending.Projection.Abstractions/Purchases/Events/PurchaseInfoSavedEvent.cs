﻿namespace Vending.Projection.Abstractions.Purchases;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseInfoSavedEvent
    (Guid PurchaseId,
     int Version,
     PurchaseInfo Purchase,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : PurchaseInfoEvent(PurchaseId, Version, TraceId, OperatedAt, OperatedBy);
