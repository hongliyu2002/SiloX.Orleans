namespace Vending.Domain.Abstractions.SnackMachines;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record SnackMachineManagerCommand
    (Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackMachineCommand(TraceId, OperatedAt, OperatedBy);
