namespace Vending.Domain.Abstractions.SnackMachines;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record SnackMachineRepoCommand
    (Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackMachineCommand(TraceId, OperatedAt, OperatedBy);
