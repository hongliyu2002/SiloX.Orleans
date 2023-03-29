using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record SnackMachineSnackPurchaseEvent(string PurchaseId, int Version, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : DomainEvent(PurchaseId, Version, TraceId, OperatedAt, OperatedBy)
{
    private readonly string _purchaseId = PurchaseId;
    public string PurchaseId
    {
        get => _purchaseId;
        init => _purchaseId = value;
    }
}
