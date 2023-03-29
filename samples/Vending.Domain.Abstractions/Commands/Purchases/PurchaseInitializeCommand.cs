namespace Vending.Domain.Abstractions.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseInitializeCommand(Guid MachineId, int Position, Guid SnackId, decimal BoughtPrice, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : PurchaseCommand(TraceId, OperatedAt, OperatedBy);
