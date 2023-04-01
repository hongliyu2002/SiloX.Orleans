namespace Vending.Domain.Abstractions.SnackMachines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineManagerDeleteManyCommand
    (IList<Guid> MachineIds,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackMachineManagerCommand(TraceId, OperatedAt, OperatedBy);
