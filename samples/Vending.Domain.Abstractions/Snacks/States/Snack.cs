using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Fluxera.Extensions.Hosting.Modules.Domain.Shared.Model;
using ReactiveUI;

namespace Vending.Domain.Abstractions.Snacks;

/// <summary>
///     Represents a snack.
/// </summary>
[Serializable]
[GenerateSerializer]
public sealed class Snack : ReactiveObject, IAuditedObject, ISoftDeleteObject
{
    /// <inheritdoc />
    public Snack()
    {
        this.WhenAnyValue(vm => vm.CreatedAt).ObserveOn(Scheduler.CurrentThread).Subscribe(createdAt => IsCreated = createdAt != null);
        this.WhenAnyValue(vm => vm.DeletedAt).ObserveOn(Scheduler.CurrentThread).Subscribe(deletedAt => IsDeleted = deletedAt != null);
    }

    [Id(0)]
    private Guid _id;
    /// <summary>
    ///     The unique identifier of the snack.
    /// </summary>
    public Guid Id { get => _id; set => this.RaiseAndSetIfChanged(ref _id, value); }

    [Id(1)]
    private DateTimeOffset? _createdAt;
    /// <summary>
    ///     The date and time when the snack was created.
    /// </summary>
    public DateTimeOffset? CreatedAt { get => _createdAt; set => this.RaiseAndSetIfChanged(ref _createdAt, value); }

    [Id(2)]
    private string? _createdBy;
    /// <summary>
    ///     The user who created the snack.
    /// </summary>
    public string? CreatedBy { get => _createdBy; set => this.RaiseAndSetIfChanged(ref _createdBy, value); }

    [Id(3)]
    private bool _isCreated;
    /// <summary>
    ///     Indicates whether the snack has been created.
    /// </summary>
    public bool IsCreated { get => _isCreated; set => this.RaiseAndSetIfChanged(ref _isCreated, value); }

    [Id(4)]
    private DateTimeOffset? _lastModifiedAt;
    /// <summary>
    ///     The date and time when the snack was last modified.
    /// </summary>
    public DateTimeOffset? LastModifiedAt { get => _lastModifiedAt; set => this.RaiseAndSetIfChanged(ref _lastModifiedAt, value); }

    [Id(5)]
    private string? _lastModifiedBy;
    /// <summary>
    ///     The user who last modified the snack.
    /// </summary>
    public string? LastModifiedBy { get => _lastModifiedBy; set => this.RaiseAndSetIfChanged(ref _lastModifiedBy, value); }

    [Id(6)]
    private DateTimeOffset? _deletedAt;
    /// <summary>
    ///     The date and time when the snack was deleted.
    /// </summary>
    public DateTimeOffset? DeletedAt { get => _deletedAt; set => this.RaiseAndSetIfChanged(ref _deletedAt, value); }

    [Id(7)]
    private string? _deletedBy;
    /// <summary>
    ///     The user who deleted the snack.
    /// </summary>
    public string? DeletedBy { get => _deletedBy; set => this.RaiseAndSetIfChanged(ref _deletedBy, value); }

    [Id(8)]
    private bool _isDeleted;
    /// <summary>
    ///     Indicates whether the snack has been deleted.
    /// </summary>
    public bool IsDeleted { get => _isDeleted; set => this.RaiseAndSetIfChanged(ref _isDeleted, value); }

    [Id(9)]
    private string _name = null!;
    /// <summary>
    ///     The name of the snack.
    /// </summary>
    public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }

    [Id(10)]
    private string? _pictureUrl;
    /// <summary>
    ///     The URL of the picture of the snack.
    /// </summary>
    public string? PictureUrl { get => _pictureUrl; set => this.RaiseAndSetIfChanged(ref _pictureUrl, value); }

    public override string ToString()
    {
        return $"Snack with Id:'{Id}' Name:'{Name}' PictureUrl:'{PictureUrl}'";
    }

    #region Apply

    public void Apply(SnackInitializeCommand command)
    {
        Id = command.SnackId;
        Name = command.Name;
        PictureUrl = command.PictureUrl;
        CreatedAt = command.OperatedAt;
        CreatedBy = command.OperatedBy;
    }

    public void Apply(SnackRemoveCommand command)
    {
        DeletedAt = command.OperatedAt;
        DeletedBy = command.OperatedBy;
    }

    public void Apply(SnackUpdateCommand command)
    {
        Name = command.Name;
        PictureUrl = command.PictureUrl;
        LastModifiedAt = command.OperatedAt;
        LastModifiedBy = command.OperatedBy;
    }

    #endregion

}
