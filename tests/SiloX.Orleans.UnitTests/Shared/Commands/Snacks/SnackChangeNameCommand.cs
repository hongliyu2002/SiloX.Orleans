﻿using SiloX.Domain.Abstractions;

namespace SiloX.Orleans.UnitTests.Shared.Commands;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackChangeNameCommand(string Name, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy)
    : DomainCommand(TraceId, OperatedAt, OperatedBy);
