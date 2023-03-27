namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackPictureUrlChangedEvent(Guid Id, int Version, string? PictureUrl, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : SnackEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
