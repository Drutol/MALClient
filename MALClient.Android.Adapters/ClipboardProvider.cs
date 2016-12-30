using Android.App;
using Android.Content;
using MALClient.Adapters;

namespace MALClient.Android.Adapters
{
    public class ClipboardProvider : IClipboardProvider
    {
        public void SetText(string text)
        {
            ClipboardManager clipboard = (ClipboardManager)Application.Context.GetSystemService(Context.ClipboardService);
            ClipData clip = ClipData.NewPlainText("", text);
            clipboard.PrimaryClip = clip;
        }
    }
}
