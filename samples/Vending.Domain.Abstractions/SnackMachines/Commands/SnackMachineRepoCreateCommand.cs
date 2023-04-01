namespace Vending.Domain.Abstractions.SnackMachines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineRepoCreateCommand
    (Money MoneyInside,
     IDictionary<int, SnackPile?> Slots,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackMachineRepoCommand(TraceId, OperatedAt, OperatedBy);
