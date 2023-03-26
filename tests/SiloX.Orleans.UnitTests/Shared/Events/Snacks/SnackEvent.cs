using SiloX.Domain.Abstractions;

namespace SiloX.Orleans.UnitTests.Shared.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record SnackEvent(Guid Id, long Version, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : DomainEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
