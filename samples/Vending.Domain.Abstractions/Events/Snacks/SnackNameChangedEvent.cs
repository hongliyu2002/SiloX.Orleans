namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackNameChangedEvent(Guid Id, int Version, string Name, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
