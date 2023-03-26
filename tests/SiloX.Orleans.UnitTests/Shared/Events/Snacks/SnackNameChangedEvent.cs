namespace SiloX.Orleans.UnitTests.Shared.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackNameChangedEvent(Guid Id, long Version, string Name, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) : SnackEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
