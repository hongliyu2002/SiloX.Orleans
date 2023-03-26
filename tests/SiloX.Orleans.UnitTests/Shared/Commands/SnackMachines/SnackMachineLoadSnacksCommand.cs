using SiloX.Domain.Abstractions;
using SiloX.Orleans.UnitTests.Shared.States;

namespace SiloX.Orleans.UnitTests.Shared.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineLoadSnacksCommand(int Position, SnackPile SnackPile, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : DomainCommand(TraceId, OperatedAt, OperatedBy);
