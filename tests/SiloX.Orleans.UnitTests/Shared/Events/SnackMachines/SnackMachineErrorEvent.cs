using System.Collections.Immutable;
using SiloX.Domain.Abstractions;

namespace SiloX.Orleans.UnitTests.Shared.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineErrorEvent(Guid Id, long Version, int Code, IImmutableList<string> Reasons, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : SnackEvent(Id, Version, TraceId, OperatedAt, OperatedBy), IDomainErrorEvent;
