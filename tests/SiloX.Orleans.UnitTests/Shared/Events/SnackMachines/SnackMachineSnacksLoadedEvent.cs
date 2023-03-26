using SiloX.Orleans.UnitTests.Shared.States;

namespace SiloX.Orleans.UnitTests.Shared.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineSnacksLoadedEvent(Guid Id, long Version, int Position, SnackPile SnackPile, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : SnackMachineEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
