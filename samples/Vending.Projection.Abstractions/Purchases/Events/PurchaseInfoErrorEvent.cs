using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Purchases;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseInfoErrorEvent
    (Guid PurchaseId,
     int Version,
     int Code,
     IList<string> Reasons,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : PurchaseInfoEvent(PurchaseId, Version, TraceId, OperatedAt, OperatedBy), IDomainErrorEvent;
