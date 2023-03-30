namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineBoughtAmountUpdatedEvent(Guid MachineId, int Version, decimal Amount, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackMachineEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy);
