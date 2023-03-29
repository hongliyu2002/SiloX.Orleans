namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackRemovedEvent(Guid SnackId, int Version, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackEvent(SnackId, Version, TraceId, OperatedAt, OperatedBy);
