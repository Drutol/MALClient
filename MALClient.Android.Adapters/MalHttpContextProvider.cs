using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.Models.Enums;
using MALClient.XShared.BL;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using ModernHttpClient;
using Xamarin.Android.Net;

namespace MALClient.Android.Adapters
{
    [Preserve(AllMembers = true)]
    public class MalHttpContextProvider : MalHttpContextProviderBase
    {
        protected override async Task<CsrfHttpClient> ObtainContext()
        {
            return null;
        }

        public override HttpClientHandler GetHandler()
        {
            return new NativeMessageHandler(false, false, new NativeCookieHandler())
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                UseCookies = true,
            };
        }
    }
}