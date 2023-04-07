namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineRepoCreateCommand
    (Money MoneyInside,
     IDictionary<int, SnackPile?> Slots,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineRepoCommand(TraceId, OperatedAt, OperatedBy);
