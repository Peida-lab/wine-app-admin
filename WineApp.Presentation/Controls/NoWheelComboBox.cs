using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace WineApp.Presentation.Controls;

public class NoWheelComboBox : ComboBox
{
    protected override void OnPointerWheelChanged(
        PointerRoutedEventArgs e)
    {
        if (IsDropDownOpen)
        {
            base.OnPointerWheelChanged(e);
        }
    }
}