namespace Vending.Domain.Abstractions.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineSnackPurchaseInitializeCommand(Guid MachineId, int Position, Guid SnackId, decimal BoughtPrice, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackMachineSnackPurchaseCommand(TraceId, OperatedAt, OperatedBy);
