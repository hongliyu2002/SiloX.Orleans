using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record MachineCommand
    (Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : DomainCommand(TraceId, OperatedAt, OperatedBy);
