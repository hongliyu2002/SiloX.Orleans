using SiloX.Domain.Abstractions;

namespace SiloX.Orleans.UnitTests.Shared.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record SnackMachineEvent(Guid Id, int Version, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : DomainEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
