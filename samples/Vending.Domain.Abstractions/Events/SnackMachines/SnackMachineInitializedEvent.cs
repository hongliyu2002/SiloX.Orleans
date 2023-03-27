using System.Collections.Immutable;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineInitializedEvent(Guid Id, int Version, Money MoneyInside, IImmutableList<Slot> Slots, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : SnackMachineEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
