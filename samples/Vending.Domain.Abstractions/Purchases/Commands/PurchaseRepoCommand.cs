namespace Vending.Domain.Abstractions.Purchases;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record PurchaseRepoCommand
    (Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : PurchaseCommand(TraceId, OperatedAt, OperatedBy);
