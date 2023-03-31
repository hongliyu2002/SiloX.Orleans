using System.Windows;
using Fluxera.Utilities.Extensions;

namespace Vending.App.Controls;

public partial class CreateSnackWindow : Window
{
    public string SnackName { get; set; } = string.Empty;

    public string? SnackPictureUrl { get; set; }

    public CreateSnackWindow()
    {
        InitializeComponent();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(NameTextBox.Text))
        {
            MessageBox.Show("Please enter a name for the snack.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        SnackName = NameTextBox.Text.Trim();
        SnackPictureUrl = PictureUrlTextBox.Text.IsNullOrWhiteSpace() ? null : PictureUrlTextBox.Text.Trim();
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
