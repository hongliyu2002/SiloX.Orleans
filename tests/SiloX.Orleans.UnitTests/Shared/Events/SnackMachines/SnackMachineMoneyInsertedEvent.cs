using SiloX.Orleans.UnitTests.Shared.States;

namespace SiloX.Orleans.UnitTests.Shared.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineMoneyInsertedEvent(Guid Id, long Version, Money Money, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : SnackMachineEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
