using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using MALClient.Adapters;
using MALClient.UWP.Shared;
using MALClient.XShared.Comm;
using MALClient.XShared.Utils;

namespace MALClient.UWP.Adapters
{
    public class ClipboardProvider : IClipboardProvider
    {
        public void SetText(string text)
        {
            var dp = new DataPackage();
            dp.SetText(text);
            Clipboard.SetContent(dp);
            UWPUtilities.GiveStatusBarFeedback("Copied to clipboard...");
        }
    }
}
