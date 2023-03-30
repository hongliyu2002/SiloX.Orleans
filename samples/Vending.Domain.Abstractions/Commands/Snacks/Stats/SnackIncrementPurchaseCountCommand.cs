namespace Vending.Domain.Abstractions.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackIncrementPurchaseCountCommand(int Number, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackCommand(TraceId, OperatedAt, OperatedBy);
