using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record SnackEvent(Guid Id, int Version, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : DomainEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
