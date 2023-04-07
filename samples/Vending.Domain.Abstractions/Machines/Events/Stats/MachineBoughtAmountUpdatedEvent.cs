namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineBoughtAmountUpdatedEvent
    (Guid MachineId,
     decimal BoughtAmount,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineEvent(MachineId, 0, TraceId, OperatedAt, OperatedBy);
