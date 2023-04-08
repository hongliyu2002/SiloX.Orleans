using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Purchases;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record PurchaseInfoEvent
    (Guid PurchaseId,
     int Version,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainEvent(Version, TraceId, OperatedAt, OperatedBy);
