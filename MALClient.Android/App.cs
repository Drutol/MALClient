using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Config;
using MALClient.XShared.Comm.CommUtils;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Managers;
using MALClient.XShared.ViewModels;

namespace MALClient.Android
{
    [Application]
    public class App : Application
    {
        public App(IntPtr handle, JniHandleOwnership ownerShip) : base(handle, ownerShip)
        {

        }

        public override void OnCreate()
        {
            ImageService.Instance.Initialize(new Configuration
            {
                FadeAnimationEnabled = true,
                FadeAnimationForCachedImages = true,
            });
            ViewModelLocator.RegisterBase();
            AndroidViewModelLocator.RegisterDependencies();
            Credentials.Init();
            HtmlClassMgr.Init();
            FavouritesManager.LoadData();
            ViewModelLocator.ForumsMain.LoadPinnedTopics();
            base.OnCreate();
        }
    }
}