using Fluxera.Guards;
using SiloX.Orleans.UnitTests.Shared.Events;

namespace SiloX.Orleans.UnitTests.Shared.States;

[Serializable]
[GenerateSerializer]
public sealed class SnackMachine
{
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
    public Money MoneyInside { get; set; }

    [Id(9)]
    public decimal AmountInTransaction { get; set; }

    [Id(10)]
    public IList<Slot> Slots { get; set; }

    public int SlotsCount => Slots.Count;

    public decimal TotalPrice => Slots.Where(s => s.SnackPile != null).Select(s => s.SnackPile!).Sum(sp => sp.TotalPrice);

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
        MoneyInside += evt.Money;
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
        AmountInTransaction += evt.Money.Amount;
        MoneyInside += evt.Money;
        LastModifiedAt = evt.OperatedAt;
        LastModifiedBy = evt.OperatedBy;
    }

    public void Apply(SnackMachineMoneyReturnedEvent evt)
    {
        if (!MoneyInside.TryAllocate(AmountInTransaction, out var moneyAllocated))
        {
            return;
        }
        MoneyInside -= moneyAllocated;
        LastModifiedAt = evt.OperatedAt;
        LastModifiedBy = evt.OperatedBy;
    }

    public void Apply(SnackMachineSnacksLoadedEvent evt)
    {
        if (!TryGetSlot(evt.Position, out var slot) || slot == null)
        {
            return;
        }
        slot.SnackPile = evt.SnackPile;
        LastModifiedAt = evt.OperatedAt;
        LastModifiedBy = evt.OperatedBy;
    }

    public void Apply(SnackMachineSnackBoughtEvent evt)
    {
        if (!TryGetSlot(evt.Position, out var slot) || slot?.SnackPile == null || !slot.SnackPile.TryPopOne(out var snackPilePopped) || snackPilePopped == null)
        {
            return;
        }
        slot.SnackPile = snackPilePopped;
        AmountInTransaction -= snackPilePopped.Price;
        LastModifiedAt = evt.OperatedAt;
        LastModifiedBy = evt.OperatedBy;
    }

    #endregion

}
