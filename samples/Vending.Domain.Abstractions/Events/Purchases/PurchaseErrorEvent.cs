using System.Collections.Immutable;
using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record PurchaseErrorEvent(string PurchaseId, int Version, int Code, IImmutableList<string> Reasons, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : PurchaseEvent(PurchaseId, Version, TraceId, OperatedAt, OperatedBy), IDomainErrorEvent;
