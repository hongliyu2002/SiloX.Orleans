using System.Collections.Immutable;

namespace Vending.Domain.Abstractions.Machines;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record MachineRemovedEvent
    (Guid MachineId,
     int Version,
     Money MoneyInside,
     decimal AmountInTransaction,
     IList<Slot> Slots,
     int SlotsCount,
     int SnackCount,
     int SnackQuantity,
     decimal SnackAmount,
     Guid TraceId,
     DateTimeOffset OperatedAt,
     string OperatedBy) : MachineEvent(MachineId, Version, TraceId, OperatedAt, OperatedBy);
