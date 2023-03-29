using System.Collections.Immutable;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineInitializeCommand(Guid Id, Money MoneyInside, IImmutableList<Slot> Slots, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) : SnackMachineCommand(TraceId, OperatedAt, OperatedBy);
