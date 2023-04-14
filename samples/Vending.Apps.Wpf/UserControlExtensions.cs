using System.Windows;
using System.Windows.Media;

namespace Vending.App.Wpf;

public static class UserControlExtensions
{
    public static Window? GetParentWindow(this DependencyObject control)
    {
        var parentControl = VisualTreeHelper.GetParent(control);
        while (parentControl != null && parentControl is not Window)
        {
            parentControl = VisualTreeHelper.GetParent(parentControl);
        }
        return parentControl as Window;
    }
}