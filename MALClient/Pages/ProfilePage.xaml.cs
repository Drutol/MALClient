using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HtmlAgilityPack;
using MALClient.Comm;
using MALClient.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{


    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        private ProfileData _data;

        public ProfilePage()
        {
            InitializeComponent();
            SpinnerLoading.Background = new SolidColorBrush(Color.FromArgb(160, 230, 230, 230));
            Loaded += OnLoaded;
        }

        public ProfileData Data
        {
            get { return _data; }
            set
            {
                _data = value;
               
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (Data != null)
            {
                PopulateAnimeData();
                PopulateMangaData();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
                _data = e.Parameter as ProfileData;
            else
                PullData();

            Utils.GetMainPageInstance().CurrentStatus = $"{Creditentials.UserName} - Profile";

            NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            NavMgr.DeregisterBackNav();
        }


        private async void PullData()
        {
            SpinnerLoading.Visibility = Visibility.Visible;

            SpinnerLoading.Visibility = Visibility.Collapsed; //end of exec path
        }

        private void PopulateAnimeData()
        {
            ChartAnime.PopulateData(new List<int>
            {
                Data.AnimeWatching,
                Data.AnimeCompleted,
                Data.AnimeOnHold,
                Data.AnimeDropped,
                Data.AnimePlanned
            });

            TxtAnimeWatchingStats.Text = Data.AnimeWatching.ToString();
            TxtAnimeCompleted.Text = Data.AnimeCompleted.ToString();
            TxtAnimeOnHold.Text = Data.AnimeOnHold.ToString();
            TxtAnimeDropped.Text = Data.AnimeDropped.ToString();
            TxtAnimePlanned.Text = Data.AnimePlanned.ToString();
            TxtAnimeTotal.Text = Data.AnimeTotal.ToString();
            TxtAnimeRewatched.Text = Data.AnimeRewatched.ToString();
            TxtAnimeEpisodes.Text = Data.AnimeEpisodes.ToString();
            TxtAnimeDays.Text = $"Days :  {Data.AnimeDays}";
        }

        private void PopulateMangaData()
        {
            ChartManga.PopulateData(new List<int>
            {
                Data.MangaReading,
                Data.MangaCompleted,
                Data.MangaOnHold,
                Data.MangaDropped,
                Data.MangaPlanned
            });

            TxtMangaReadingStats.Text = Data.MangaReading.ToString();
            TxtMangaCompleted.Text = Data.MangaCompleted.ToString();
            TxtMangaOnHold.Text = Data.MangaOnHold.ToString();
            TxtMangaDropped.Text = Data.MangaDropped.ToString();
            TxtMangaPlanned.Text = Data.MangaPlanned.ToString();
            TxtMangaTotal.Text = Data.MangaTotal.ToString();
            TxtMangaReread.Text = Data.MangaReread.ToString();
            TxtMangaChapters.Text = Data.MangaChapters.ToString();
            TxtMangaVolumes.Text = Data.MangaVolumes.ToString();
            TxtMangaDays.Text = $"Days :  {Data.MangaDays}";
        }
    }
}