namespace Vending.Domain.Abstractions.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineIncrementBoughtAmountCommand(decimal Amount, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackMachineCommand(TraceId, OperatedAt, OperatedBy);
