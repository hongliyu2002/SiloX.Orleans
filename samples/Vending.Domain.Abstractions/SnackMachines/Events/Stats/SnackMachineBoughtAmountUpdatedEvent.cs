namespace Vending.Domain.Abstractions.SnackMachines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineBoughtAmountUpdatedEvent
    (Guid MachineId,
     decimal Amount,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackMachineEvent(MachineId, 0, TraceId, OperatedAt, OperatedBy);
