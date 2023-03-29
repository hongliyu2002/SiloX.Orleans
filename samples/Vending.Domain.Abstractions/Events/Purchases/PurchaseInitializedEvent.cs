namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseInitializedEvent(string PurchaseId, Guid MachineId, int Position, Guid SnackId, decimal BoughtPrice, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : PurchaseEvent(PurchaseId, MachineId, Position, SnackId, TraceId, OperatedAt, OperatedBy);
