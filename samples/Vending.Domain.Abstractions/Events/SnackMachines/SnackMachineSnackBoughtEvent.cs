using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineSnackBoughtEvent(Guid Id, int Version, decimal AmountInTransaction, Slot Slot, int SnackQuantity, decimal SnackAmount, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) :
    SnackMachineEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
