using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Config;
using FFImageLoading.Helpers;
using FFImageLoading.Work;
using MALClient.XShared.BL;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Managers;
using MALClient.XShared.ViewModels;
using ModernHttpClient;

namespace MALClient.Android
{
    [Application]
    public class App : Application
    {
        public static App Current { get; private set; }

        public App(IntPtr handle, JniHandleOwnership ownerShip) : base(handle, ownerShip)
        {
            Current = this;
        }

        public override void OnCreate()
        {

            //ImageService.Instance.Initialize(new Configuration
            //{
            //    BitmapOptimizations = true,
            //    VerbosePerformanceLogging = true,
            //    Logger = new MiniLogger()
            //});
            //var sp = new Stopwatch();
            //sp.Start();
            ImageService.Instance.Initialize(new Configuration
            {
                HttpClient = new HttpClient(new NativeMessageHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate}),
                ExecuteCallbacksOnUIThread = true,
                AnimateGifs = false,
            });
            //System.Diagnostics.Debug.WriteLine($"ImgSer init {sp.ElapsedMilliseconds}");
            //sp.Restart();
            ViewModelLocator.RegisterBase();
            //System.Diagnostics.Debug.WriteLine($"RegBase {sp.ElapsedMilliseconds}");
            //sp.Restart();
            AndroidViewModelLocator.RegisterDependencies();
            //System.Diagnostics.Debug.WriteLine($"RegDep {sp.ElapsedMilliseconds}");
            //sp.Restart();
            InitializationRoutines.InitApp();
            //System.Diagnostics.Debug.WriteLine($"App init {sp.ElapsedMilliseconds}");
            //sp.Restart();
            ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) =>
            {
                if(certificate.Subject == "CN=*.myanimelist.net" || certificate.Subject == "CN=*.google.com, O=Google Inc, L=Mountain View, S=California, C=US")
                    return true;
                return false;
            };
            
            base.OnCreate();
        }

       
    }
}