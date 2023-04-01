using System.Collections.Immutable;

namespace Vending.Domain.Abstractions.SnackMachines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineInitializeCommand
    (Guid MachineId,
     Money MoneyInside,
     IImmutableList<Slot> Slots,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackMachineCommand(TraceId, OperatedAt, OperatedBy);
