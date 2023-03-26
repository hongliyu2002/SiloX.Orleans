using SiloX.Domain.Abstractions;

namespace SiloX.Orleans.UnitTests.Shared.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackInitializeCommand(string Name, string? PictureUrl, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : DomainCommand(TraceId, OperatedAt, OperatedBy);
