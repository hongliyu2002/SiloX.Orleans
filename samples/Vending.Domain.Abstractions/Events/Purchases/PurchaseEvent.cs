using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record PurchaseEvent(string PurchaseId, Guid MachineId, int Position, Guid SnackId, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : DomainEvent(PurchaseId, 0, TraceId, OperatedAt, OperatedBy);
