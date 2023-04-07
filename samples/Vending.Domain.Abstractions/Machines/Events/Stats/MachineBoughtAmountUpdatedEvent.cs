namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineBoughtAmountUpdatedEvent
    (Guid MachineId,
     decimal Amount,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineEvent(MachineId, 0, TraceId, OperatedAt, OperatedBy);
