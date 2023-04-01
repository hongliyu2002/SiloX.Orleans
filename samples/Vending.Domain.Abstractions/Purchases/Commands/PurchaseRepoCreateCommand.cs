namespace Vending.Domain.Abstractions.Purchases;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseRepoCreateCommand
    (Guid MachineId,
     int Position,
     Guid SnackId,
     decimal BoughtPrice,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : PurchaseRepoCommand(TraceId, OperatedAt, OperatedBy);
