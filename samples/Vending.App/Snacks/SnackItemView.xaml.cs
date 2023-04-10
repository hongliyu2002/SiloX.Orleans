using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Media.Imaging;
using Fluxera.Utilities.Extensions;
using ReactiveUI;

namespace Vending.App.Snacks;

public partial class SnackItemView
{
    private const string DefaultUrl = "pack://application:,,,/Vending.App;component/Images/snack.png";
    private static readonly TextDecorationCollection NormalDecorations = new();
    private static readonly TextDecorationCollection DeletedDecorations = new() { new TextDecoration(TextDecorationLocation.Strikethrough, null, 0, TextDecorationUnit.FontRecommended, TextDecorationUnit.FontRecommended) };

    public SnackItemView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
                           {
                               this.OneWayBind(ViewModel, vm => vm.Name, v => v.NameRun.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.PictureUrl, v => v.PictureImage.Source, url => new BitmapImage(new Uri(url.IsNullOrEmpty() ? DefaultUrl : url))).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.MachineCount, v => v.MachineCountRun.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.TotalQuantity, v => v.TotalQuantityRun.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.TotalAmount, v => v.TotalAmountRun.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.BoughtCount, v => v.BoughtCountRun.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.BoughtAmount, v => v.BoughtAmountRun.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.NameRun.TextDecorations, deleted => deleted ? DeletedDecorations : NormalDecorations).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.MachineCountRun.TextDecorations, deleted => deleted ? DeletedDecorations : NormalDecorations).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.TotalQuantityRun.TextDecorations, deleted => deleted ? DeletedDecorations : NormalDecorations).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.TotalAmountRun.TextDecorations, deleted => deleted ? DeletedDecorations : NormalDecorations).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.BoughtCountRun.TextDecorations, deleted => deleted ? DeletedDecorations : NormalDecorations).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.IsDeleted, v => v.BoughtAmountRun.TextDecorations, deleted => deleted ? DeletedDecorations : NormalDecorations).DisposeWith(disposable);
                           });
    }
}