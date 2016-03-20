using System;
using System.Xml.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MALClient.Items;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{


    public sealed partial class AnimeDetailsPage : Page , IDetailsViewInteraction
    {
        private AnimeDetailsPageViewModel ViewModel => DataContext as AnimeDetailsPageViewModel;
        private static PivotItem _detailsPivotHoldingSpace;
        public AnimeDetailsPage()
        {
            this.InitializeComponent();
            ViewModel.View = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var param = e.Parameter as AnimeDetailsPageNavigationArgs;
            if (param == null)
                throw new Exception("No paramaters for this page");
            ViewModel.Init(param);
            if (!param.AnimeMode)
            {
                _detailsPivotHoldingSpace = (PivotItem)MainPivot.Items[1];
                MainPivot.Items.RemoveAt(1);
            }
            else if (_detailsPivotHoldingSpace != null)
            {
                MainPivot.Items.Insert(1,_detailsPivotHoldingSpace);
                _detailsPivotHoldingSpace = null;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataContext = null;
            base.OnNavigatedFrom(e);
            NavMgr.DeregisterBackNav();
        }      

        private void SubmitWatchedEps(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ViewModel.ChangeWatchedEps();
                e.Handled = true;
            }
        }

        public Flyout GetWatchedEpsFlyout()
        {
            return WatchedEpsFlyout;
        }

        private void Pivot_OnPivotItemLoading(Pivot sender, PivotItemEventArgs args)
        {
            switch (args.Item.Tag as string)
            {
                case "Details":
                    ViewModel.LoadDetails();
                    break;
                case "Reviews":
                    ViewModel.LoadReviews();
                    break;
                case "Recomm":
                    ViewModel.LoadRecommendations();
                    break;
                case "Related":
                    ViewModel.LoadRelatedAnime();
                    break;
            }            
        }
    }
}