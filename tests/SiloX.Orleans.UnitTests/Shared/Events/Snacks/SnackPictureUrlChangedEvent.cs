namespace SiloX.Orleans.UnitTests.Shared.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackPictureUrlChangedEvent(Guid Id, long Version, string? PictureUrl, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : SnackEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
