namespace Vending.Domain.Abstractions.Purchases;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseManagerDeleteCommand
    (Guid PurchaseId,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : PurchaseManagerCommand(TraceId, OperatedAt, OperatedBy);
