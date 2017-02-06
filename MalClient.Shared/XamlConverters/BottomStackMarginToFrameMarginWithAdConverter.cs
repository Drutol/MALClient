using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using MALClient.XShared.ViewModels;

namespace MALClient.UWP.Shared.XamlConverters
{
    public class BottomStackMarginToFrameMarginWithAdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var val = (Thickness) value;
            if(val.Bottom == 48 && ViewModelLocator.GeneralMain.AdsContainerVisibility)
                return parameter == null ? new Thickness(0,-48,0,0) : new Thickness(0,0,0,-48);
            return new Thickness(0);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
