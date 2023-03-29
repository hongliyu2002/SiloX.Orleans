using Fluxera.Guards;
using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Entities;

/// <summary>
///     Represents a pile of snacks with the same type and price in the vending machine.
/// </summary>
[PublicAPI]
[Serializable]
public sealed class SnackPile
{
    /// <summary>
    ///     Gets or sets the unique identifier of the snack.
    /// </summary>
    public Guid SnackId { get; set; }

    /// <summary>
    ///     Gets or sets the name of the snack. Cannot be null.
    /// </summary>
    public string SnackName { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the URL of the picture of the snack, if available.
    /// </summary>
    public string? SnackPictureUrl { get; set; }

    /// <summary>
    ///     Gets or sets the quantity of snacks in the pile.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    ///     Gets or sets the price of one snack in the pile.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    ///     Gets or sets the total price of all snacks in the pile.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    ///     Updates the name and picture URL of the snack.
    /// </summary>
    /// <param name="getNamePicture">A function that returns the name and picture URL of the snack. </param>
    public async Task UpdateSnackNameAndPictureUrlAsync(Func<Guid, Task<(string, string?)>> getNamePicture)
    {
        Guard.Against.Null(getNamePicture, nameof(getNamePicture));
        var (name, pictureUrl) = await getNamePicture(SnackId);
        SnackName = name;
        SnackPictureUrl = pictureUrl;
    }
}
