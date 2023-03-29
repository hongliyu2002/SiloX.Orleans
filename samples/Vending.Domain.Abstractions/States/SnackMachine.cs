using Vending.Domain.Abstractions.Commands;

namespace Vending.Domain.Abstractions.States;

[Serializable]
[GenerateSerializer]
public sealed class SnackMachine
{
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

    public int SnackQuantity => Slots.Where(s => s.SnackPile != null).Sum(s => s.SnackPile!.Quantity);

    public decimal SnackAmount => Slots.Where(s => s.SnackPile != null).Sum(s => s.SnackPile!.TotalAmount);

    public override string ToString()
    {
        return $"SnackMachine with Id:'{Id}' MoneyInside:'{MoneyInside}' AmountInTransaction:{AmountInTransaction} Slots:'{string.Join(';', Slots.Select(slot => slot.ToString()))}'";
    }

    #region Get Slot

    public bool TryGetSlot(int position, out Slot? slot)
    {
        slot = Slots.FirstOrDefault(sl => sl.Position == position);
        return slot != null;
    }

    #endregion

    #region Apply

    public void Apply(SnackMachineInitializeCommand command)
    {
        Id = command.Id;
        MoneyInside = command.MoneyInside;
        Slots = command.Slots.ToList();
        CreatedAt = command.OperatedAt;
        CreatedBy = command.OperatedBy;
    }

    public void Apply(SnackMachineRemoveCommand command)
    {
        DeletedAt = command.OperatedAt;
        DeletedBy = command.OperatedBy;
        IsDeleted = true;
    }

    public void Apply(SnackMachineLoadMoneyCommand command)
    {
        MoneyInside += command.Money;
        LastModifiedAt = command.OperatedAt;
        LastModifiedBy = command.OperatedBy;
    }

    public void Apply(SnackMachineUnloadMoneyCommand command)
    {
        MoneyInside = Money.Zero;
        LastModifiedAt = command.OperatedAt;
        LastModifiedBy = command.OperatedBy;
    }

    public void Apply(SnackMachineInsertMoneyCommand command)
    {
        MoneyInside += command.Money;
        AmountInTransaction += command.Money.Amount;
        LastModifiedAt = command.OperatedAt;
        LastModifiedBy = command.OperatedBy;
    }

    public void Apply(SnackMachineReturnMoneyCommand command)
    {
        if (MoneyInside.CanAllocate(AmountInTransaction, out var moneyToReturn))
        {
            MoneyInside -= moneyToReturn;
            AmountInTransaction = 0;
            LastModifiedAt = command.OperatedAt;
            LastModifiedBy = command.OperatedBy;
        }
    }

    public void Apply(SnackMachineLoadSnacksCommand command)
    {
        var slot = Slots.FirstOrDefault(sl => sl.Position == command.Position);
        if (slot != null)
        {
            slot.SnackPile = command.SnackPile;
        }
        else
        {
            Slots.Add(new Slot { Position = command.Position, SnackPile = command.SnackPile });
        }
        LastModifiedAt = command.OperatedAt;
        LastModifiedBy = command.OperatedBy;
    }

    public void Apply(SnackMachineBuySnackCommand command)
    {
        var slot = Slots.FirstOrDefault(sl => sl.Position == command.Position);
        if (slot is { SnackPile: { } })
        {
            slot.SnackPile -= 1;
            AmountInTransaction -= slot.SnackPile.Price;
            LastModifiedAt = command.OperatedAt;
            LastModifiedBy = command.OperatedBy;
        }
    }

    #endregion

}
