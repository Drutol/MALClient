using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using MALClient.XShared.Utils;

namespace MALClient.UWP.Adapters
{
    public class CssManager : CssManagerBase
    {    
        public CssManager()
        {
            var uiSettings = new UISettings();
            var color = uiSettings.GetColorValue(UIColorType.Accent);
            var color1 = uiSettings.GetColorValue(UIColorType.AccentDark2);
            var color2 = uiSettings.GetColorValue(UIColorType.AccentLight2);

            AccentColour = "#" + color.ToString().Substring(3);
            AccentColourLight = "#" + color2.ToString().Substring(3);
            AccentColourDark = "#" + color1.ToString().Substring(3);

        }

        protected override string AccentColour { get; }
        protected override string AccentColourLight { get; }
        protected override string AccentColourDark { get; }
        protected override string NotifyFunction => "window.external.notify";
    }
}
