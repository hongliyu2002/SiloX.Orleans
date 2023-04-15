﻿using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Media.Imaging;
using Fluxera.Utilities.Extensions;
using ReactiveUI;

namespace Vending.Apps.Wpf.Snacks;

public partial class SnackEditView
{
    private const string DefaultUrl = "pack://application:,,,/Vending.Apps.Wpf;component/Images/snack.png";

    public SnackEditView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
                           {
                               this.OneWayBind(ViewModel, vm => vm.ErrorInfo, v => v.ErrorLabel.Visibility, StringToVisibilityConverter).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.ErrorInfo, v => v.ErrorLabel.Content).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.Id, v => v.IdTextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.Name, v => v.NameTextBox.Text).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.PictureUrl, v => v.PictureTextBox.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.PictureUrl, v => v.PictureImage.Source, url => new BitmapImage(new Uri(url.IsNullOrEmpty() ? DefaultUrl : url))).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.IsDeletedCheckBox.IsChecked).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.NameTextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.PictureTextBox.IsEnabled, deleted => !deleted).DisposeWith(disposable);
                               this.BindCommand(ViewModel, vm => vm.SaveSnackCommand, v => v.SaveButton).DisposeWith(disposable);
                           });
    }

    private Visibility StringToVisibilityConverter(string value)
    {
        return value.IsNotNullOrEmpty() ? Visibility.Visible : Visibility.Collapsed;
    }
}