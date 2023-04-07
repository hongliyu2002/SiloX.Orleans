using System.Collections.Immutable;
using SiloX.Domain.Abstractions;

namespace Vending.Projection.Abstractions.Snacks;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackInfoErrorEvent
    (Guid SnackId,
     int Version,
     int Code,
     IImmutableList<string> Reasons,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackInfoEvent(SnackId, Version, TraceId, OperatedAt, OperatedBy), IDomainErrorEvent;
