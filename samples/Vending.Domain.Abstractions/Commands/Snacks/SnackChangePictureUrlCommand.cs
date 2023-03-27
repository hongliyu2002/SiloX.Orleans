using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackChangePictureUrlCommand(string? PictureUrl, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : DomainCommand(TraceId, OperatedAt, OperatedBy);
