using Fluxera.Guards;
using Vending.Domain.Abstractions.Events;

namespace Vending.Domain.Abstractions.States;

[Serializable]
[GenerateSerializer]
public sealed class SnackMachine
{
    public SnackMachine()
    {
    }

    public SnackMachine(Guid id, Money moneyInside, decimal amountInTransaction, IList<Slot> slots)
    {
        Id = Guard.Against.Empty(id, nameof(id));
        MoneyInside = Guard.Against.Null(moneyInside, nameof(moneyInside));
        AmountInTransaction = Guard.Against.Negative(amountInTransaction, nameof(amountInTransaction));
        Slots = Guard.Against.Null(slots, nameof(slots));
    }

    [Id(0)]
    public Guid Id { get; set; }

    [Id(1)]
    public DateTimeOffset? CreatedAt { get; set; }

    [Id(2)]
    public string? CreatedBy { get; set; }

    public bool IsCreated => CreatedAt != null;

    [Id(3)]
    public DateTimeOffset? LastModifiedAt { get; set; }

    [Id(4)]
    public string? LastModifiedBy { get; set; }

    [Id(5)]
    public DateTimeOffset? DeletedAt { get; set; }

    [Id(6)]
    public string? DeletedBy { get; set; }

    [Id(7)]
    public bool IsDeleted { get; set; }

    [Id(8)]
    public Money MoneyInside { get; set; } = Money.Zero;

    [Id(9)]
    public decimal AmountInTransaction { get; set; }

    [Id(10)]
    public IList<Slot> Slots { get; set; } = new List<Slot>();

    public int SlotsCount => Slots.Count;

    public int SnackCount => Slots.Where(s => s.SnackPile != null).Select(s => s.SnackPile!.SnackId).Distinct().Count();

    public int SnackQuantity => Slots.Where(s => s.SnackPile != null).Select(s => s.SnackPile!.Quantity).Sum();

    public decimal SnackAmount => Slots.Where(s => s.SnackPile != null).Select(s => s.SnackPile!.TotalPrice).Sum();

    public override string ToString()
    {
        return $"SnackMachine with Id:'{Id}' MoneyInside:'{MoneyInside}' AmountInTransaction:{AmountInTransaction} Slots:'{string.Join(';', Slots.Select(slot => slot.ToString()))}'";
    }

    #region Get Slot

    public bool TryGetSlot(int position, out Slot? slot)
    {
        slot = Slots.FirstOrDefault(x => x.Position == position);
        return slot != null;
    }

    #endregion

    #region Apply

    public void Apply(SnackMachineInitializedEvent evt)
    {
        Id = evt.Id;
        MoneyInside = evt.MoneyInside;
        Slots = evt.Slots.ToList();
        CreatedAt = evt.OperatedAt;
        CreatedBy = evt.OperatedBy;
    }

    public void Apply(SnackMachineRemovedEvent evt)
    {
        DeletedAt = evt.OperatedAt;
        DeletedBy = evt.OperatedBy;
        IsDeleted = true;
    }

    public void Apply(SnackMachineMoneyLoadedEvent evt)
    {
        MoneyInside = evt.MoneyInside;
        LastModifiedAt = evt.OperatedAt;
        LastModifiedBy = evt.OperatedBy;
    }

    public void Apply(SnackMachineMoneyUnloadedEvent evt)
    {
        MoneyInside = Money.Zero;
        LastModifiedAt = evt.OperatedAt;
        LastModifiedBy = evt.OperatedBy;
    }

    public void Apply(SnackMachineMoneyInsertedEvent evt)
    {
        MoneyInside = evt.MoneyInside;
        AmountInTransaction = evt.AmountInTransaction;
        LastModifiedAt = evt.OperatedAt;
        LastModifiedBy = evt.OperatedBy;
    }

    public void Apply(SnackMachineMoneyReturnedEvent evt)
    {
        MoneyInside = evt.MoneyInside;
        AmountInTransaction = 0;
        LastModifiedAt = evt.OperatedAt;
        LastModifiedBy = evt.OperatedBy;
    }

    public void Apply(SnackMachineSnacksLoadedEvent evt)
    {
        var slot = Slots.FirstOrDefault(sl => sl == evt.Slot || (sl.MachineId == evt.Slot.MachineId && sl.Position == evt.Slot.Position));
        if (slot != null)
        {
            slot.SnackPile = evt.Slot.SnackPile;
        }
        LastModifiedAt = evt.OperatedAt;
        LastModifiedBy = evt.OperatedBy;
    }

    public void Apply(SnackMachineSnackBoughtEvent evt)
    {
        var slot = Slots.FirstOrDefault(sl => sl == evt.Slot || (sl.MachineId == evt.Slot.MachineId && sl.Position == evt.Slot.Position));
        if (slot != null)
        {
            slot.SnackPile = evt.Slot.SnackPile;
        }
        AmountInTransaction = evt.AmountInTransaction;
        LastModifiedAt = evt.OperatedAt;
        LastModifiedBy = evt.OperatedBy;
    }

    #endregion

}
