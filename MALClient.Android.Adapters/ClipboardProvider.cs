using MALClient.Adapters;

namespace MALClient.Android.Adapters
{
    public class ClipboardProvider : IClipboardProvider
    {
        public void SetText(string text)
        {
            //var dp = new DataPackage();
            //dp.SetText(text);
            //Clipboard.SetContent(dp);
            //UWPUtilities.GiveStatusBarFeedback("Copied to clipboard...");
        }
    }
}
