namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineBoughtCountUpdatedEvent(Guid MachineId, int Version, int Count, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackMachineEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy);
