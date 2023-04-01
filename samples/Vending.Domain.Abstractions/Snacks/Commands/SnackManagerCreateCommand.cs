namespace Vending.Domain.Abstractions.Snacks;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackManagerCreateCommand
    (string Name,
     string? PictureUrl,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackManagerCommand(TraceId, OperatedAt, OperatedBy);
