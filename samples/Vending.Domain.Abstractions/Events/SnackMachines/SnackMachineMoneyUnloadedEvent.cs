using Vending.Domain.Abstractions.States;

namespace Vending.Domain.Abstractions.Events;

[Immutable]
[Serializable]
[GenerateSerializer]
public sealed record SnackMachineMoneyUnloadedEvent(Guid Id, int Version, Money MoneyInside, Guid TraceId, DateTimeOffset OperatedAt, string OperatedBy) 
    : SnackMachineEvent(Id, Version, TraceId, OperatedAt, OperatedBy);
