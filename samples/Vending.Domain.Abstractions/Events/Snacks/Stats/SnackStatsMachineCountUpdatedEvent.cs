namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackStatsMachineCountUpdatedEvent(Guid SnackId, int Version, int Count, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackEvent(SnackId, Version, TraceId, OperatedAt, OperatedBy);
