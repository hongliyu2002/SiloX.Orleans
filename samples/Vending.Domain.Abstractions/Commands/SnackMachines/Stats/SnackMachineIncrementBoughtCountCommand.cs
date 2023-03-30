namespace Vending.Domain.Abstractions.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineIncrementBoughtCountCommand(int Number, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackMachineCommand(TraceId, OperatedAt, OperatedBy);
