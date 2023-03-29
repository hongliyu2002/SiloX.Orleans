namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackPictureUrlChangedEvent(Guid SnackId, int Version, string? PictureUrl, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : SnackEvent(SnackId, Version, TraceId, OperatedAt, OperatedBy);
