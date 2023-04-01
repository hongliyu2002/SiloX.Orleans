using System.Collections.Immutable;

namespace Vending.Domain.Abstractions.SnackMachines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineInitializedEvent
    (Guid MachineId,
     int Version,
     Money MoneyInside,
     IImmutableList<Slot> Slots,
     int SlotsCount,
     int SnackCount,
     int SnackQuantity,
     decimal SnackAmount,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : SnackMachineEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy);
