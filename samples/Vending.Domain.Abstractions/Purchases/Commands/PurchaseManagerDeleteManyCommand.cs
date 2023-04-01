namespace Vending.Domain.Abstractions.Purchases;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseManagerDeleteManyCommand
    (IList<Guid> PurchaseIds,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : PurchaseManagerCommand(TraceId, OperatedAt, OperatedBy);
