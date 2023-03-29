namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackStatsBoughtAmountUpdatedEvent(Guid Id, int Version, decimal Amount, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
