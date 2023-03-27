namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackInitializedEvent(Guid Id, int Version, string Name, string? PictureUrl, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : SnackEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
