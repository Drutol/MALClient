

using MALClient.Android.Resources;
using MALClient.XShared.Utils;

namespace MALClient.Android
{
    public class CssManager : CssManagerBase
    {
        protected override string AccentColour => '#'+ResourceExtension.AccentColourHex.Substring(3);
        protected override string AccentColourLight => '#'+ ResourceExtension.AccentColourLightHex.Substring(3);
        protected override string AccentColourDark => '#'+ResourceExtension.AccentColourDarkHex.Substring(3);
        protected override string NotifyFunction => "android.OnData";
        protected override string ShadowsDefinition => "0px 0px 34px 4px";
    }
}
