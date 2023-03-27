using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Entities;

[PublicAPI]
[Serializable]
public sealed class SnackPile
{
    public Guid SnackId { get; set; }

    public string SnackName { get; set; } = null!;

    public string? SnackPictureUrl { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }
    
    public decimal TotalPrice { get; set; }
}
