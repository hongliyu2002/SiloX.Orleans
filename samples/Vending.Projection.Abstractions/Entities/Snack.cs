using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Entities;

[PublicAPI]
[Serializable]
public sealed class Snack
{
    public Guid Id { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset? LastModifiedAt { get; set; }

    public string? LastModifiedBy { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public bool IsDeleted { get; set; }

    public string Name { get; set; } = null!;

    public string? PictureUrl { get; set; }

    public int MachineCount { get; set; }
    
    public int BoughtCount { get; set; }

    public decimal BoughtAmount { get; set; }
}
