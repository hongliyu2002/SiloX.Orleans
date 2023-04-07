namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record MachineRepoCommand
    (Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineCommand(TraceId, OperatedAt, OperatedBy);
