namespace Vending.Domain.Abstractions.SnackMachines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineManagerDeleteCommand
    (Guid MachineId,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackMachineManagerCommand(TraceId, OperatedAt, OperatedBy);
