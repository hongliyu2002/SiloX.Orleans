using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows;
using Fluxera.Guards;
using Orleans.FluentResults;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions.Snacks;

namespace Vending.App.Snacks;

public class SnackEditViewModel : ReactiveObject
{
    private readonly ISnackRepoGrain _snackRepoGrain;


    public SnackEditViewModel(Snack snack, ISnackRepoGrain snackRepoGrain)
    {
        snack = Guard.Against.Null(snack, nameof(snack));
        snackRepoGrain = Guard.Against.Null(snackRepoGrain, nameof(snackRepoGrain));
        _snackRepoGrain = snackRepoGrain;
        Id = snack.Id;
        Name = snack.Name;
        PictureUrl = snack.PictureUrl;
        IsDeleted = snack.IsDeleted;
        SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
    }

    [Reactive]
    public Guid Id { get; set; }

    [Reactive]
    public string Name { get; set; }

    [Reactive]
    public string? PictureUrl { get; set; }

    [Reactive]
    public bool IsDeleted { get; set; }

    #region Commands

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    private async Task SaveAsync()
    {
        var result = await Result.Ok()
                                 .BindTryIfAsync(Id == Guid.Empty, () => _snackRepoGrain.CreateAsync(new SnackRepoCreateCommand(Name, PictureUrl, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
                                 .BindTryIfAsync<Snack>(Id != Guid.Empty, () => _snackRepoGrain.UpdateAsync(new SnackRepoUpdateCommand(Id, Name, PictureUrl, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager")))
                                 .TapTryAsync(snack =>
                                              {
                                                  Id = snack.Id;
                                                  Name = snack.Name;
                                                  PictureUrl = snack.PictureUrl;
                                                  IsDeleted = snack.IsDeleted;
                                              });
        if (result.IsFailed)
        {
            MessageBox.Show(result.Errors.ToMessage(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    #endregion

}