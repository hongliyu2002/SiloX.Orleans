namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackStatsBoughtAmountUpdatedEvent(Guid SnackId, int Version, decimal Amount, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackEvent(SnackId, Version, TraceId, OperatedAt, OperatedBy);
