using Fluxera.Extensions.Hosting.Modules.Domain.Shared.Model;
using Vending.Domain.Abstractions.Commands;

namespace Vending.Domain.Abstractions.States;

/// <summary>
///     Represents a Snack Machine.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class SnackMachine : IAuditedObject, ISoftDeleteObject
{
    /// <summary>
    ///     The unique identifier of the Snack Machine.
    /// </summary>
    [Id(0)]
    public Guid Id { get; set; }

    /// <summary>
    ///     The date and time when the Snack Machine was created.
    /// </summary>
    [Id(1)]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     The user who created the Snack Machine.
    /// </summary>
    [Id(2)]
    public string? CreatedBy { get; set; }

    /// <summary>
    ///     Indicates whether the Snack Machine has been created.
    /// </summary>
    public bool IsCreated => CreatedAt != null;

    /// <summary>
    ///     The date and time when the Snack Machine was last modified.
    /// </summary>
    [Id(3)]
    public DateTimeOffset? LastModifiedAt { get; set; }

    /// <summary>
    ///     The user who last modified the Snack Machine.
    /// </summary>
    [Id(4)]
    public string? LastModifiedBy { get; set; }

    /// <summary>
    ///     The date and time when the Snack Machine was deleted.
    /// </summary>
    [Id(5)]
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    ///     The user who deleted the Snack Machine.
    /// </summary>
    [Id(6)]
    public string? DeletedBy { get; set; }

    /// <summary>
    ///     Indicates whether the Snack Machine has been deleted.
    /// </summary>
    [Id(7)]
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     The money inside the Snack Machine.
    /// </summary>
    [Id(8)]
    public Money MoneyInside { get; set; } = Money.Zero;

    /// <summary>
    ///     The amount of money in the transaction.
    /// </summary>
    [Id(9)]
    public decimal AmountInTransaction { get; set; }

    /// <summary>
    ///     The slots of the Snack Machine.
    /// </summary>
    [Id(10)]
    public IList<Slot> Slots { get; set; } = new List<Slot>();

    /// <summary>
    ///     The number of slots in the Snack Machine.
    /// </summary>
    public int SlotsCount => Slots.Count;

    /// <summary>
    ///     The number of snacks in the Snack Machine.
    /// </summary>
    public int SnackCount => Slots.Where(s => s.SnackPile != null).Select(s => s.SnackPile!.SnackId).Distinct().Count();

    /// <summary>
    ///     The quantity of snacks in the Snack Machine.
    /// </summary>
    public int SnackQuantity => Slots.Where(s => s.SnackPile != null).Sum(s => s.SnackPile!.Quantity);

    /// <summary>
    ///     The amount of snacks in the Snack Machine.
    /// </summary>
    public decimal SnackAmount => Slots.Where(s => s.SnackPile != null).Sum(s => s.SnackPile!.TotalAmount);

    /// <inheritdoc />
    public override string ToString()
    {
        return $"SnackMachine with Id:'{Id}' MoneyInside:'{MoneyInside}' AmountInTransaction:{AmountInTransaction} Slots:'{string.Join(';', Slots.Select(slot => slot.ToString()))}'";
    }

    #region Get Slot

    /// <summary>
    ///     Gets the slot at the specified position.
    /// </summary>
    /// <param name="position"> The position of the slot. </param>
    /// <param name="slot"> The slot at the specified position. </param>
    /// <returns> <c>true</c> if the slot exists; otherwise, <c>false</c>. </returns>
    public bool TryGetSlot(int position, out Slot? slot)
    {
        slot = Slots.FirstOrDefault(sl => sl.Position == position);
        return slot != null;
    }

    #endregion

    #region Apply

    public void Apply(SnackMachineInitializeCommand command)
    {
        Id = command.MachineId;
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
            if (slot.SnackPile != null && slot.SnackPile.SnackId == command.SnackPile.SnackId)
            {
                slot.SnackPile += command.SnackPile.Quantity;
            }
            else
            {
                slot.SnackPile = command.SnackPile;
            }
        }
        else
        {
            Slots.Add(new Slot { Position = command.Position, SnackPile = command.SnackPile });
        }
        LastModifiedAt = command.OperatedAt;
        LastModifiedBy = command.OperatedBy;
    }

    public void Apply(SnackMachineUnloadSnacksCommand command)
    {
        var slot = Slots.FirstOrDefault(sl => sl.Position == command.Position);
        if (slot != null)
        {
            slot.SnackPile = null;
        }
        else
        {
            Slots.Add(new Slot { Position = command.Position, SnackPile = null });
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
