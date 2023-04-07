using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Snacks;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record SnackInfoEvent
    (Guid SnackId,
     int Version,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainEvent(Version, TraceId, OperatedAt, OperatedBy);
