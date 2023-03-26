namespace SiloX.Orleans.UnitTests.Shared.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record SnackRemovedEvent(Guid Id, long Version, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) : SnackEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
