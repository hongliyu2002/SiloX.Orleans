using SiloX.Domain.Abstractions;

namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public abstract record SnackMachineEvent(Guid MachineId, int Version, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : DomainEvent(MachineId.ToString(), Version, TraceId, OperatedAt, OperatedBy)
{
    private readonly Guid _machineId = MachineId;
    public Guid MachineId
    {
        get => _machineId;
        init => _machineId = value;
    }
}
