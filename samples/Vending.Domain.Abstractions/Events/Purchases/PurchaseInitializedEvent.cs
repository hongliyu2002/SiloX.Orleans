namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseInitializedEvent(string PurchaseId, int Version, Guid MachineId, int Position, Guid SnackId, decimal BoughtPrice, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : PurchaseEvent(PurchaseId, Version, TraceId, OperatedAt, OperatedBy);
