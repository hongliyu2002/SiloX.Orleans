namespace Vending.Domain.Abstractions.Snacks;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackManagerDeleteManyCommand
    (IList<Guid> SnackIds,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackManagerCommand(TraceId, OperatedAt, OperatedBy);
