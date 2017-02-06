using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using MALClient.XShared.Utils;

namespace MALClient.UWP.Shared.XamlConverters
{
    /// <summary>
    ///     If anime status in grid view's selection menu is the same -> return accent color
    /// </summary>
    internal class MyStatusSelectionToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value.ToString() == parameter as string)
                return Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
            return Settings.SelectedTheme == (int)ApplicationTheme.Dark
                ? new SolidColorBrush(Colors.White)
                : new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}