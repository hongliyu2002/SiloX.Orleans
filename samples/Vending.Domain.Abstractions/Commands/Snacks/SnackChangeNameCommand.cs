using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackChangeNameCommand(string Name, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : DomainCommand(TraceId, OperatedAt, OperatedBy);
