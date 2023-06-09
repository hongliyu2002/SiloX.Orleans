﻿namespace Vending.Domain.Abstractions.Machines;

/// <summary>
///     Represents a snack mMachine.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class Machine
{
    /// <summary>
    ///     The unique identifier of the snack mMachine.
    /// </summary>
    [Id(0)]
    public Guid Id { get; set; }

    /// <summary>
    ///     The money inside the snack mMachine.
    /// </summary>
    [Id(1)]
    public Money MoneyInside { get; set; } = Money.Zero;

    /// <summary>
    ///     The amount of money in the transaction.
    /// </summary>
    [Id(2)]
    public decimal AmountInTransaction { get; set; }

    /// <summary>
    ///     The slots of the snack mMachine.
    /// </summary>
    [Id(3)]
    public IList<MachineSlot> Slots { get; set; } = new List<MachineSlot>();

    /// <summary>
    ///     The snack statistics of the snack mMachine.
    /// </summary>
    [Id(4)]
    public IList<MachineSnackStat> SnackStats { get; set; } = new List<MachineSnackStat>();

    /// <summary>
    ///     The date and time when the snack mMachine was created.
    /// </summary>
    [Id(5)]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     The user who created the snack mMachine.
    /// </summary>
    [Id(6)]
    public string? CreatedBy { get; set; }

    /// <summary>
    ///     Indicates whether the snack mMachine has been created.
    /// </summary>
    public bool IsCreated => CreatedAt.HasValue;

    /// <summary>
    ///     The date and time when the snack mMachine was last modified.
    /// </summary>
    [Id(7)]
    public DateTimeOffset? LastModifiedAt { get; set; }

    /// <summary>
    ///     The user who last modified the snack mMachine.
    /// </summary>
    [Id(8)]
    public string? LastModifiedBy { get; set; }

    /// <summary>
    ///     The date and time when the snack mMachine was deleted.
    /// </summary>
    [Id(9)]
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    ///     The user who deleted the snack mMachine.
    /// </summary>
    [Id(10)]
    public string? DeletedBy { get; set; }

    /// <summary>
    ///     Indicates whether the snack mMachine has been deleted.
    /// </summary>
    [Id(11)]
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     The number of slots in the snack mMachine.
    /// </summary>
    [Id(12)]
    public int SlotCount { get; set; }

    /// <summary>
    ///     The number of snacks in the snack mMachine.
    /// </summary>
    [Id(13)]
    public int SnackCount { get; set; }

    /// <summary>
    ///     The quantity of snacks in the snack mMachine.
    /// </summary>
    [Id(14)]
    public int SnackQuantity { get; set; }

    /// <summary>
    ///     The amount of snacks in the snack mMachine.
    /// </summary>
    [Id(15)]
    public decimal SnackAmount { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Machine with Id:'{Id}' MoneyInside:'{MoneyInside}' AmountInTransaction:{AmountInTransaction} Slots:'{string.Join(';', Slots.Select(slot => slot.ToString()))}'";
    }

    #region Get Slot

    /// <summary>
    ///     Gets the machineSlot at the specified position.
    /// </summary>
    /// <param name="position"> The position of the machineSlot. </param>
    /// <param name="slot"> The machineSlot at the specified position. </param>
    /// <returns> <c>true</c> if the machineSlot exists; otherwise, <c>false</c>. </returns>
    public bool TryGetSlot(int position, out MachineSlot? slot)
    {
        slot = Slots.FirstOrDefault(ms => ms.Position == position);
        return slot != null;
    }

    #endregion

    #region Update Stats

    private void UpdateStats()
    {
        SlotCount = Slots.Count;
        SnackStats = Slots.Where(ms => ms.SnackPile != null)
                          .Select(ms => new
                                        {
                                            ms.MachineId,
                                            ms.SnackPile!.SnackId,
                                            ms.SnackPile.Quantity,
                                            TotalAmount = ms.SnackPile.Amount
                                        })
                          .GroupBy(r => new { r.MachineId, r.SnackId })
                          .Select(g => new MachineSnackStat(g.Key.MachineId, g.Key.SnackId, g.Sum(r => r.Quantity), g.Sum(r => r.TotalAmount)))
                          .ToList();
        SnackCount = SnackStats.Select(ss => ss.SnackId).Count();
        SnackQuantity = SnackStats.Sum(ss => ss.TotalQuantity);
        SnackAmount = SnackStats.Sum(ss => ss.TotalAmount);
    }

    #endregion

    #region Apply

    public void Apply(MachineInitializeCommand command)
    {
        Id = command.MachineId;
        MoneyInside = command.MoneyInside;
        Slots = command.Slots.Select(pair => new MachineSlot(command.MachineId, pair.Key, pair.Value)).ToList();
        UpdateStats();
        CreatedAt = command.OperatedAt;
        CreatedBy = command.OperatedBy;
    }

    public void Apply(MachineDeleteCommand command)
    {
        DeletedAt = command.OperatedAt;
        DeletedBy = command.OperatedBy;
        IsDeleted = true;
    }
    
    public void Apply(MachineUpdateCommand command)
    {
        MoneyInside = command.MoneyInside;
        Slots = command.Slots.Select(pair => new MachineSlot(Id, pair.Key, pair.Value)).ToList();
        UpdateStats();
        LastModifiedAt = command.OperatedAt;
        LastModifiedBy = command.OperatedBy;
    }
    
    public void Apply(MachineAddSlotCommand command)
    {
        var slot = Slots.FirstOrDefault(ms => ms.Position == command.Position);
        if (slot == null)
        {
            Slots.Add(new MachineSlot(Id, command.Position, command.SnackPile));
            UpdateStats();
            LastModifiedAt = command.OperatedAt;
            LastModifiedBy = command.OperatedBy;
        }
    }

    public void Apply(MachineRemoveSlotCommand command)
    {
        var slot = Slots.FirstOrDefault(ms => ms.Position == command.Position);
        if (slot != null && Slots.Remove(slot))
        {
            UpdateStats();
            LastModifiedAt = command.OperatedAt;
            LastModifiedBy = command.OperatedBy;
        }
    }

    public void Apply(MachineLoadMoneyCommand command)
    {
        MoneyInside.Add(command.Money);
        LastModifiedAt = command.OperatedAt;
        LastModifiedBy = command.OperatedBy;
    }

    public void Apply(MachineUnloadMoneyCommand command)
    {
        MoneyInside = Money.Zero;
        LastModifiedAt = command.OperatedAt;
        LastModifiedBy = command.OperatedBy;
    }

    public void Apply(MachineInsertMoneyCommand command)
    {
        MoneyInside.Add(command.Money);
        AmountInTransaction += command.Money.Amount;
        LastModifiedAt = command.OperatedAt;
        LastModifiedBy = command.OperatedBy;
    }

    public void Apply(MachineReturnMoneyCommand command)
    {
        if (MoneyInside.CanAllocate(AmountInTransaction, out var moneyToReturn))
        {
            MoneyInside.Subtract(moneyToReturn);
            AmountInTransaction -= moneyToReturn.Amount;
            LastModifiedAt = command.OperatedAt;
            LastModifiedBy = command.OperatedBy;
        }
    }

    public void Apply(MachineLoadSnacksCommand command)
    {
        var slot = Slots.FirstOrDefault(ms => ms.Position == command.Position);
        if (slot != null)
        {
            if (slot.SnackPile != null)
            {
                if (slot.SnackPile.SnackId == command.SnackPile.SnackId)
                {
                    slot.SnackPile.Add(command.SnackPile.Quantity);
                }
            }
            else
            {
                slot.SnackPile = command.SnackPile;
            }
            UpdateStats();
            LastModifiedAt = command.OperatedAt;
            LastModifiedBy = command.OperatedBy;
        }
    }

    public void Apply(MachineUnloadSnacksCommand command)
    {
        var slot = Slots.FirstOrDefault(ms => ms.Position == command.Position);
        if (slot != null)
        {
            slot.SnackPile = null;
            UpdateStats();
            LastModifiedAt = command.OperatedAt;
            LastModifiedBy = command.OperatedBy;
        }
    }

    public void Apply(MachineBuySnackCommand command)
    {
        var slot = Slots.FirstOrDefault(ms => ms.Position == command.Position);
        if (slot is { SnackPile: not null })
        {
            slot.SnackPile.Subtract(1);
            AmountInTransaction -= slot.SnackPile.Price;
            UpdateStats();
            LastModifiedAt = command.OperatedAt;
            LastModifiedBy = command.OperatedBy;
        }
    }

    #endregion

}
