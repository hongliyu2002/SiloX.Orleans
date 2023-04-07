using Fluxera.Extensions.Hosting.Modules.Domain.Shared.Model;
using JetBrains.Annotations;

namespace Vending.Projection.Abstractions.Machines;

/// <summary>
///     Represents a machine that sells snacks.
/// </summary>
[PublicAPI]
[Serializable]
[GenerateSerializer]
public sealed class MachineInfo : IAuditedObject, ISoftDeleteObject
{
    /// <summary>
    ///     The ID of the machine.
    /// </summary>
    [Id(0)]
    public Guid Id { get; set; }

    /// <summary>
    ///     The version of the machine.(Concurrency token)
    /// </summary>
    [Id(1)]
    public int Version { get; set; }

    /// <summary>
    ///     The amount of money currently inside the machine.
    /// </summary>
    [Id(2)]
    public MoneyInfo MoneyInside { get; set; } = new();

    /// <summary>
    ///     The amount of money currently in the transaction.
    /// </summary>
    [Id(3)]
    public decimal AmountInTransaction { get; set; }

    /// <summary>
    ///     The list of slots in the machine.
    /// </summary>
    [Id(4)]
    public IList<MachineSlotInfo> Slots { get; set; } = new List<MachineSlotInfo>();

    /// <summary>
    ///     The date and time when the machine was created.
    /// </summary>
    [Id(5)]
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    ///     The name of the user who created the machine.
    /// </summary>
    [Id(6)]
    public string? CreatedBy { get; set; }

    /// <summary>
    ///     The date and time when the machine was last modified.
    /// </summary>
    [Id(7)]
    public DateTimeOffset? LastModifiedAt { get; set; }

    /// <summary>
    ///     The name of the user who last modified the machine.
    /// </summary>
    [Id(8)]
    public string? LastModifiedBy { get; set; }

    /// <summary>
    ///     The date and time when the machine was deleted.
    /// </summary>
    [Id(9)]
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    ///     The name of the user who deleted the machine.
    /// </summary>
    [Id(10)]
    public string? DeletedBy { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this machine has been deleted.
    /// </summary>
    [Id(11)]
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     The number of slots in the machine.
    /// </summary>
    [Id(12)]
    public int SlotsCount { get; set; }

    /// <summary>
    ///     The number of distinct snacks in the machine.
    /// </summary>
    [Id(13)]
    public int SnackCount { get; set; }

    /// <summary>
    ///     The total quantity of snacks in the machine.
    /// </summary>
    [Id(14)]
    public int SnackQuantity { get; set; }

    /// <summary>
    ///     The total amount of money from snacks in the machine.
    /// </summary>
    [Id(15)]
    public decimal SnackAmount { get; set; }

    /// <summary>
    ///     The number of times snacks have been bought from the machine.
    /// </summary>
    [Id(16)]
    public int BoughtCount { get; set; }

    /// <summary>
    ///     The total amount of money spent on snacks from the machine.
    /// </summary>
    [Id(17)]
    public decimal BoughtAmount { get; set; }
}
