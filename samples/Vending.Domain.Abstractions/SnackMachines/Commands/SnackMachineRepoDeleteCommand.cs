namespace Vending.Domain.Abstractions.SnackMachines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineRepoDeleteCommand
    (Guid MachineId,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackMachineRepoCommand(TraceId, OperatedAt, OperatedBy);
