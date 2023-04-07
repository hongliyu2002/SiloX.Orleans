namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineRepoDeleteManyCommand
    (IList<Guid> MachineIds,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineRepoCommand(TraceId, OperatedAt, OperatedBy);
