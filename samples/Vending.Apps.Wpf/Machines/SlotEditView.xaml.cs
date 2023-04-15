using System.Globalization;
using System.Reactive.Disposables;
using ReactiveUI;

namespace Vending.Apps.Wpf.Machines;

public partial class SlotEditView
{
    public SlotEditView()
    {
        InitializeComponent();
        this.WhenActivated(disposable =>
                           {
                               this.Bind(ViewModel, vm => vm.Position, v => v.PositionTextBox.Text, IntToTextConverter, TextToIntConverter).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.Quantity, v => v.QuantityTextBox.Text, NullableIntToTextConverter, TextToNullableIntConverter).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.Price, v => v.PriceTextBox.Text, NullableDecimalToTextConverter, TextToNullableDecimalConverter).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.Amount, v => v.AmountText.Text).DisposeWith(disposable);
                               this.OneWayBind(ViewModel, vm => vm.Snacks, v => v.SnackItemsComboBox.ItemsSource).DisposeWith(disposable);
                               this.Bind(ViewModel, vm => vm.CurrentSnack, v => v.SnackItemsComboBox.SelectedItem).DisposeWith(disposable);
                           });
    }

    private string IntToTextConverter(int number)
    {
        return number.ToString();
    }

    private int TextToIntConverter(string text)
    {
        return int.TryParse(text, out var number) ? number : 0;
    }

    private string NullableIntToTextConverter(int? number)
    {
        return number.HasValue ? number.Value.ToString() : string.Empty;
    }

    private int? TextToNullableIntConverter(string text)
    {
        if (int.TryParse(text, out var number))
        {
            return number;
        }
        return null;
    }

    private string NullableDecimalToTextConverter(decimal? amount)
    {
        return amount.HasValue ? amount.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
    }

    private decimal? TextToNullableDecimalConverter(string text)
    {
        if (decimal.TryParse(text, out var amount))
        {
            return amount;
        }
        return null;
    }
}