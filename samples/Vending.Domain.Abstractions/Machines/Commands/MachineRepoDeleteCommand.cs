namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineRepoDeleteCommand
    (Guid MachineId,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineRepoCommand(TraceId, OperatedAt, OperatedBy);
