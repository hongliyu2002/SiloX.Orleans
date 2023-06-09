﻿using Fluxera.Guards;
using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Machines;

/// <summary>
///     Represents a pile of snacks with the same type and price in the machine.
/// </summary>
[PublicAPI]
[Serializable]
[GenerateSerializer]
public sealed class SnackPileInfo
{
    /// <summary>
    ///     The unique identifier of the snack.
    /// </summary>
    [Id(0)]
    public Guid SnackId { get; set; }

    /// <summary>
    ///     The name of the snack. Cannot be null.
    /// </summary>
    [Id(1)]
    public string SnackName { get; set; } = null!;

    /// <summary>
    ///     The URL of the picture of the snack, if available.
    /// </summary>
    [Id(2)]
    public string? SnackPictureUrl { get; set; }

    /// <summary>
    ///     The quantity of snacks in the pile.
    /// </summary>
    [Id(3)]
    public int Quantity { get; set; }

    /// <summary>
    ///     The price of one snack in the pile.
    /// </summary>
    [Id(4)]
    public decimal Price { get; set; }

    /// <summary>
    ///     The total price of all snacks in the pile.
    /// </summary>
    [Id(5)]
    public decimal Amount { get; set; }

    #region Update

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

    #endregion

}
