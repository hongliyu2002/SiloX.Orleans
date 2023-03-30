using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record PurchaseEvent(Guid PurchaseId, Guid MachineId, int Position, Guid SnackId, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : DomainEvent(0, TraceId, OperatedAt, OperatedBy);
