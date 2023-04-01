namespace Vending.Domain.Abstractions.Snacks;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackManagerDeleteCommand
    (Guid SnackId,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackManagerCommand(TraceId, OperatedAt, OperatedBy);
