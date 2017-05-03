using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MALClient.Android.Activities;
using MALClient.Android.Aidl;
using MALClient.Android.DIalogs;
using MALClient.Android.Listeners;
using MALClient.Android.ViewModels;
using MALClient.XShared.ViewModels;
using Exception = System.Exception;

namespace MALClient.Android.Fragments.SettingsFragments
{
    public class SettingsAboutFragment : MalFragmentBase
    {
        private SettingsViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = AndroidViewModelLocator.Settings;
        }

        protected override void InitBindings()
        {
            AboutPageGetItOnWindows.SetOnClickListener(
                new OnClickListener(view => ResourceLocator.SystemControlsLauncherService.LaunchUri(
                    new Uri("https://www.microsoft.com/store/apps/9NBLGGH5F3BL?ocid=android"))));

            AboutPageViewSourceButton.SetOnClickListener(
                new OnClickListener(view => ResourceLocator.SystemControlsLauncherService.LaunchUri(
                    new Uri("https://github.com/Drutol/MALClient"))));

            AboutPageIssuesBoard.SetOnClickListener(
                new OnClickListener(view => ResourceLocator.SystemControlsLauncherService.LaunchUri(
                    new Uri("https://github.com/Drutol/MALClient/issues"))));

            var listener = new OnClickListener(AboutPageDonateButtonOnClick);
            AboutPageDonate1Button.SetOnClickListener(listener);
            AboutPageDonate2Button.SetOnClickListener(listener);
            AboutPageDonate3Button.SetOnClickListener(listener);
            AboutPageDonate4Button.SetOnClickListener(listener);

            AboutPageChangelogButton.SetOnClickListener(
                new OnClickListener(view => ChangelogDialog.BuildChangelogDialog(ResourceLocator.ChangelogProvider)));
        }

        private async void AboutPageDonateButtonOnClick(View view)
        {
            InAppBillingServiceConnection connection = null;
            try
            {
                connection = new InAppBillingServiceConnection(Activity);
                var sem = new SemaphoreSlim(0);
                connection.OnConnected += (sender, args) => sem.Release();
                connection.Connect();
                if (await sem.WaitAsync(TimeSpan.FromSeconds(5)))
                {
                    var buyIntentBundle = connection.Service.GetBuyIntent(3, Context.PackageName,
                        GetProductSku(), "inapp", "");
                    var pendingIntent = buyIntentBundle.GetParcelable("BUY_INTENT") as PendingIntent;
                    MainActivity.CurrentContext.StartIntentSenderForResult(pendingIntent.IntentSender, 1001, new Intent(), 0, 0, 0, Bundle.Empty);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                ResourceLocator.MessageDialogProvider.ShowMessageDialog("Something went wrong, you can always try later ^^", "Something went wrong™");
            }
            finally
            {
                connection.Disconnected();
            }

            string GetProductSku()
            {
                switch (view.Id)
                {
                    case Resource.Id.AboutPageDonate1Button:
                        return "donation1";
                    case Resource.Id.AboutPageDonate2Button:
                        return "donate2";
                    case Resource.Id.AboutPageDonate3Button:
                        return "donate3";
                    case Resource.Id.AboutPageDonate4Button:
                        return "donate4";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }

        public override int LayoutResourceId => Resource.Layout.SettingsPageAbout;

        #region Views

        private FrameLayout _aboutPageGetItOnWindows;
        private Button _aboutPageViewSourceButton;
        private Button _aboutPageIssuesBoard;
        private Button _aboutPageDonate1Button;
        private Button _aboutPageDonate2Button;
        private Button _aboutPageDonate3Button;
        private Button _aboutPageDonate4Button;
        private Button _aboutPageChangelogButton;

        public FrameLayout AboutPageGetItOnWindows => _aboutPageGetItOnWindows ?? (_aboutPageGetItOnWindows = FindViewById<FrameLayout>(Resource.Id.AboutPageGetItOnWindows));

        public Button AboutPageViewSourceButton => _aboutPageViewSourceButton ?? (_aboutPageViewSourceButton = FindViewById<Button>(Resource.Id.AboutPageViewSourceButton));

        public Button AboutPageIssuesBoard => _aboutPageIssuesBoard ?? (_aboutPageIssuesBoard = FindViewById<Button>(Resource.Id.AboutPageIssuesBoard));

        public Button AboutPageDonate1Button => _aboutPageDonate1Button ?? (_aboutPageDonate1Button = FindViewById<Button>(Resource.Id.AboutPageDonate1Button));

        public Button AboutPageDonate2Button => _aboutPageDonate2Button ?? (_aboutPageDonate2Button = FindViewById<Button>(Resource.Id.AboutPageDonate2Button));

        public Button AboutPageDonate3Button => _aboutPageDonate3Button ?? (_aboutPageDonate3Button = FindViewById<Button>(Resource.Id.AboutPageDonate3Button));

        public Button AboutPageDonate4Button => _aboutPageDonate4Button ?? (_aboutPageDonate4Button = FindViewById<Button>(Resource.Id.AboutPageDonate4Button));

        public Button AboutPageChangelogButton => _aboutPageChangelogButton ?? (_aboutPageChangelogButton = FindViewById<Button>(Resource.Id.AboutPageChangelogButton));

        #endregion
    }
}