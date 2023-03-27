using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Entities;

[PublicAPI]
[Serializable]
public sealed class SnackBought
{
    public Guid MachineId { get; set; }

    public int Position { get; set; }
    
    public Guid SnackId { get; set; }
    
    public string SnackName { get; set; } = null!;

    public string? SnackPictureUrl { get; set; }
    
    public decimal Price { get; set; }
    
    public DateTimeOffset? BoughtAt { get; set; }

    public string? BoughtBy { get; set; }
}
