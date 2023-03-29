namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineSnackPurchaseInitializedEvent(string PurchaseId, int Version, Guid MachineId, int Position, Guid SnackId, decimal BoughtPrice, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackMachineSnackPurchaseEvent(PurchaseId, Version, TraceId, OperatedAt, OperatedBy);
