namespace SiloX.Orleans.Clustering.UnitTests.Shared.Commands;

[Immutable]
[GenerateSerializer]
public abstract record DomainCommand(Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) : ITraceable;
