﻿using Fluxera.Guards;
using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Purchases;

/// <summary>
///     Represents a record of a snack that has been bought from a machine.
/// </summary>
[PublicAPI]
[Serializable]
[GenerateSerializer]
public sealed class PurchaseInfo
{
    /// <summary>
    ///     Gets or sets the ID of the purchase.
    /// </summary>
    [Id(0)]
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the version of the purchase.(Concurrency token)
    /// </summary>
    [Id(1)]
    public int Version { get; set; }

    /// <summary>
    ///     Gets or sets the ID of the machine where the snack was bought.
    /// </summary>
    [Id(2)]
    public Guid MachineId { get; set; }

    /// <summary>
    ///     Gets or sets the position of the machineSlot in the machine where the snack was bought.
    /// </summary>
    [Id(3)]
    public int Position { get; set; }

    /// <summary>
    ///     Gets or sets the ID of the snack that was bought.
    /// </summary>
    [Id(4)]
    public Guid SnackId { get; set; }

    /// <summary>
    ///     Gets or sets the name of the snack that was bought.
    /// </summary>
    [Id(5)]
    public string SnackName { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the URL of the picture for the snack that was bought.
    /// </summary>
    [Id(6)]
    public string? SnackPictureUrl { get; set; }

    /// <summary>
    ///     Gets or sets the price of the snack that was bought.
    /// </summary>
    [Id(7)]
    public decimal BoughtPrice { get; set; }

    /// <summary>
    ///     Gets or sets the date and time when the snack was bought.
    /// </summary>
    [Id(8)]
    public DateTimeOffset? BoughtAt { get; set; }

    /// <summary>
    ///     Gets or sets the name of the user who bought the snack.
    /// </summary>
    [Id(9)]
    public string? BoughtBy { get; set; }

    #region Update methods

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
