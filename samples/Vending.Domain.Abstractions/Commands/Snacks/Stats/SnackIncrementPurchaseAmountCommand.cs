namespace Vending.Domain.Abstractions.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackIncrementPurchaseAmountCommand(decimal Amount, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackCommand(TraceId, OperatedAt, OperatedBy);
