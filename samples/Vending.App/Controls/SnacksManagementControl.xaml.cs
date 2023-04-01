using System;
using System.Threading.Tasks;
using System.Windows;
using Orleans;
using SiloX.Domain.Abstractions.Extensions;
using Vending.Domain.Abstractions.Snacks;

namespace Vending.App.Controls;

public partial class SnacksManagementControl
{
    private readonly IClusterClient _clusterClient;

    public SnacksManagementControl(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
        InitializeComponent();
        LoadSnacks();
    }

    private void LoadSnacks()
    {
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        LoadSnacks();
    }

    private async void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        var createSnackWindow = new CreateSnackWindow();
        if (createSnackWindow.ShowDialog() == true)
        {
            await InitializeAsync(createSnackWindow.SnackName, createSnackWindow.SnackPictureUrl);
            SnacksListView.Items.Refresh();
        }
    }

    private void RemoveButton_Click(object sender, RoutedEventArgs e)
    {
    }

    private void ChangeNameButton_Click(object sender, RoutedEventArgs e)
    {
    }

    private void ChangePictureUrlButton_Click(object sender, RoutedEventArgs e)
    {
    }

    private async Task InitializeAsync(string name, string? pictureUrl)
    {
        try
        {
            var snackId = Guid.NewGuid();
            var grain = _clusterClient.GetGrain<ISnackGrain>(snackId);
            var command = new SnackInitializeCommand(snackId, name, pictureUrl, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager");
            var result = await grain.InitializeAsync(command);
            if (result.IsSuccess)
            {
                MessageBox.Show("Snack has been created successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(result.Errors.ToReasonString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task RemoveAsync(Guid snackId)
    {
        try
        {
            var grain = _clusterClient.GetGrain<ISnackGrain>(snackId);
            var command = new SnackRemoveCommand(Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager");
            var result = await grain.RemoveAsync(command);
            if (result.IsSuccess)
            {
                MessageBox.Show("Snack has been removed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(result.Errors.ToReasonString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task ChangeNameAsync(Guid snackId, string name)
    {
        try
        {
            var grain = _clusterClient.GetGrain<ISnackGrain>(snackId);
            var command = new SnackChangeNameCommand(name, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager");
            var result = await grain.ChangeNameAsync(command);
            if (result.IsSuccess)
            {
                MessageBox.Show("Snack's name has been changed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(result.Errors.ToReasonString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task ChangePictureUrlAsync(Guid snackId, string? pictureUrl)
    {
        try
        {
            var grain = _clusterClient.GetGrain<ISnackGrain>(snackId);
            var command = new SnackChangePictureUrlCommand(pictureUrl, Guid.NewGuid(), DateTimeOffset.UtcNow, "Manager");
            var result = await grain.ChangePictureUrlAsync(command);
            if (result.IsSuccess)
            {
                MessageBox.Show("Snack's picture URL has been changed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(result.Errors.ToReasonString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
