namespace Vending.Domain.Abstractions.Purchases;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseManagerCreateCommand
    (Guid MachineId,
     int Position,
     Guid SnackId,
     decimal BoughtPrice,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : PurchaseManagerCommand(TraceId, OperatedAt, OperatedBy);
