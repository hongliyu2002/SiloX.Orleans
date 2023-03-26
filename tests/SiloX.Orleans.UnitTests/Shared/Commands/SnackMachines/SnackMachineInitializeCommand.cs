using System.Collections.Immutable;
using SiloX.Domain.Abstractions;
using SiloX.Orleans.UnitTests.Shared.States;

namespace SiloX.Orleans.UnitTests.Shared.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineInitializeCommand(Money MoneyInside, IImmutableList<Slot> Slots, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : DomainCommand(TraceId, OperatedAt, OperatedBy);
