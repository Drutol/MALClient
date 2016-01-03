using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using MALClient.Items;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MALClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<AnimeItem> _animeItems = new ObservableCollection<AnimeItem>(); 
        private List<AnimeItem> _allAnimeItems = new List<AnimeItem>();

        public MainPage()
        {
            this.InitializeComponent();
            Utils.Utils.CheckTiles();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(e.Parameter.ToString()))
                LaunchUri(e.Parameter.ToString());
        }

        private async void LaunchUri(string url)
        {
            await Launcher.LaunchUriAsync(new Uri(url));
        }

        private async void FetchData(object sender, RoutedEventArgs e)
        {
            _allAnimeItems.Clear();
            _animeItems.Clear();
            var args = new AnimeListParameters
            {
                status = "all",
                type = "anime",
                user = User.Text
            };
            var data = await new AnimeListQuery(args).GetRequestResponse();
            var anime = data.Root.Elements("anime").ToList();
            foreach (var item in anime)
            {
                _allAnimeItems.Add(new AnimeItem(item.Element("series_title").Value, item.Element("series_image").Value, Convert.ToInt32(item.Element("series_animedb_id").Value),Convert.ToInt32(item.Element("my_status").Value)));              
            }

            RefreshList();
            Animes.ItemsSource = _animeItems;

        }

        private int GetDesiredStatus()
        {
            int value = StatusSelector.SelectedIndex;
            value++;
            return (value == 5 || value == 6) ? value + 1 : value;
        }

        private void RefreshList()
        {
            _animeItems.Clear();
            foreach (var item in _allAnimeItems.Where(item => GetDesiredStatus() == 7 || item.status == GetDesiredStatus()))
                _animeItems.Add(item);
            AlternateRowColors();
        }

        private void ChangeListStatus(object sender, SelectionChangedEventArgs e)
        {
            RefreshList();
        }

        private void AlternateRowColors()
        {
            for (int i = 0; i < _animeItems.Count; i++)
            {
                if((i+1)%2 == 0)
                    _animeItems[i].Setbackground(new SolidColorBrush(Color.FromArgb(170,230,230,230)));
                else             
                    _animeItems[i].Setbackground(new SolidColorBrush(Colors.Transparent));           
            }
        }

        private async void PinTileMal(object sender, RoutedEventArgs e)
        {
            foreach (var item in Animes.SelectedItems)
            {
                var anime = item as AnimeItem;
                if (SecondaryTile.Exists(anime.Id.ToString()))
                    continue;
                    anime.PinTile($"http://www.myanimelist.net/anime/{anime.Id}");                 
            }
        }
        private async void PinTileKiss(object sender, RoutedEventArgs e)
        {
            foreach (var item in Animes.SelectedItems)
            {
                var anime = item as AnimeItem;
                if (SecondaryTile.Exists(anime.Id.ToString()))
                    continue;

                anime.PinTile($"https://kissanime.to/M/Anime/{anime.Name.Replace(' ', '-')}");
            }
        }
    }
}
