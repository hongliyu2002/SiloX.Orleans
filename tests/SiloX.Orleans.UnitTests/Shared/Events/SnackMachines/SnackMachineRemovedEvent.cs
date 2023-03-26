namespace SiloX.Orleans.UnitTests.Shared.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineRemovedEvent(Guid Id, long Version, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackMachineEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
