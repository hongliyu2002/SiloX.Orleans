namespace Vending.Domain.Abstractions.Purchases;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseInitializeCommand
    (Guid PurchaseId,
     Guid MachineId,
     int Position,
     Guid SnackId,
     decimal BoughtPrice,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : PurchaseCommand(TraceId, OperatedAt, OperatedBy);
