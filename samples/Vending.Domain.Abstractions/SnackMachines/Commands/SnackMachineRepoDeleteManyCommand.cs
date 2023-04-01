namespace Vending.Domain.Abstractions.SnackMachines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineRepoDeleteManyCommand
    (IList<Guid> MachineIds,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackMachineRepoCommand(TraceId, OperatedAt, OperatedBy);
