using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Entities;

[PublicAPI]
[Serializable]
public sealed class SnackMachine
{
    public Guid Id { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset? LastModifiedAt { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public bool IsDeleted { get; set; }

    public Money MoneyInside { get; set; } = new();

    public decimal AmountInTransaction { get; set; }

    public IList<Slot> Slots { get; set; } = new List<Slot>();

    public int SlotsCount { get; set; }
    
    public int SnackCount { get; set; }
    
    public int SnackQuantity { get; set; }

    public decimal SnackAmount { get; set; }
    
    public int BoughtCount { get; set; }

    public decimal BoughtAmount { get; set; }
}
