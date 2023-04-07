using System.Collections.Immutable;

namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineInitializedEvent
    (Guid MachineId,
     int Version,
     Money MoneyInside,
     IList<MachineSlot> Slots,
     int SlotsCount,
     int SnackCount,
     int SnackQuantity,
     decimal SnackAmount,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy);
