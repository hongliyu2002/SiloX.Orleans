namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackBoughtAmountUpdatedEvent(Guid SnackId, decimal Amount, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackEvent(SnackId, 0, TraceId, OperatedAt, OperatedBy);
