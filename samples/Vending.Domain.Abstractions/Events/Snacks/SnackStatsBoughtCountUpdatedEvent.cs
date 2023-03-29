namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackStatsBoughtCountUpdatedEvent(Guid Id, int Version, int Count, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
