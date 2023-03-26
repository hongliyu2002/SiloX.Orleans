using SiloX.Domain.Abstractions;

namespace SiloX.Orleans.UnitTests.Shared.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public record SnackMachineEvent(Guid Id, long Version, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : DomainEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
