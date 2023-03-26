namespace SiloX.Orleans.UnitTests.Shared.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineSnackBoughtEvent(Guid Id, long Version, int Position, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackMachineEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
