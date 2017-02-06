using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using MALClient.XShared.Utils;

namespace MALClient.UWP.Shared.XamlConverters
{
    public class BoolToAnimeItemManipulationModesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (Settings.EnableSwipeToIncDec)
            {
                if ((bool) value)
                {
                    return ManipulationModes.TranslateRailsX | ManipulationModes.TranslateX |
                           ManipulationModes.System | ManipulationModes.TranslateInertia;
                }
            } 
            return ManipulationModes.System;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
