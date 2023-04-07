namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineRemoveCommand
    (Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineCommand(TraceId, OperatedAt, OperatedBy);
