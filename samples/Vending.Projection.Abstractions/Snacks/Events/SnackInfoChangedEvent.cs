using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Snacks.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackInfoChangedEvent
    (Guid SnackId,
     int Version,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainEvent(Version, TraceId, OperatedAt, OperatedBy);
