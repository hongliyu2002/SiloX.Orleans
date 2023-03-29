using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record PurchaseCommand(Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : DomainCommand(TraceId, OperatedAt, OperatedBy);
