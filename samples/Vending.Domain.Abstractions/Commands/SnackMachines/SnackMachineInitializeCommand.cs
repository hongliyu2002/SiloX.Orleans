using System.Collections.Immutable;
using SiloX.Domain.Abstractions;
using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineInitializeCommand(Money MoneyInside, IImmutableList<Slot> Slots, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : DomainCommand(TraceId, OperatedAt, OperatedBy);
