using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.BillingClient.Api;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MALClient.Android.Activities;
using MALClient.Android.Adapters;
using MALClient.Android.DIalogs;
using MALClient.Android.Listeners;
using MALClient.Android.ViewModels;
using MALClient.XShared.ViewModels;
using Org.Json;
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
            BillingAdapter adapter = null;
            try
            {
                adapter = new BillingAdapter();
                await adapter.Initialize(Activity);

                //first try to redeem all existing ones
                var ownedItems = adapter.BillingClient.QueryPurchases("inapp");
                if (ownedItems.ResponseCode != 0)
                    throw new Exception();

                // Get the list of purchased items
                foreach (var purchaseData in ownedItems.PurchasesList)
                {
                    await adapter.BillingClient.ConsumeAsync(ConsumeParams.NewBuilder().SetPurchaseToken(purchaseData.PurchaseToken).Build());
                }

                var sku = await adapter.BillingClient.QuerySkuDetailsAsync(SkuDetailsParams.NewBuilder()
                    .SetSkusList(new List<string> { GetProductSku() }).SetType("inapp").Build());

                if(sku.Result.ResponseCode != BillingResponseCode.Ok)
                    throw new Exception();

                var buyIntentBundle = adapter.BillingClient.LaunchBillingFlow(Activity,
                    BillingFlowParams.NewBuilder().SetSkuDetails(sku.SkuDetails[0]).Build());
            }
            catch (Exception e)
            {
                ResourceLocator.MessageDialogProvider.ShowMessageDialog(
                    "Something went wrong, you can always try later ^^", "Something went wrong™");
            }
            finally
            {
                adapter?.BillingClient?.EndConnection();
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