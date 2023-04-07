namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineDecrementBoughtCountCommand
    (int Number,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineCommand(TraceId, OperatedAt, OperatedBy);
