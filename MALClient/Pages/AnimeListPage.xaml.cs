using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Windows.Graphics.Printing;
using Windows.System;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using MALClient.Comm;
using MALClient.Items;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnimeListPage : Page
    {
        private enum SortOptions
        {
            SortNothing,
            SortTitle,
            SortScore,
            SortWatched,
        }

        private SortOptions _sortOption = SortOptions.SortNothing;
        private bool _sortDescending = true;
        private ObservableCollection<AnimeItem> _animeItems = new ObservableCollection<AnimeItem>();
        private List<AnimeItem> _allAnimeItems = new List<AnimeItem>();

        public AnimeListPage()
        {
            this.InitializeComponent();
            if (!string.IsNullOrWhiteSpace(Creditentials.UserName))
            {
                ListSource.Text = Creditentials.UserName;
                FetchData();
            }
            else
            {
                EmptyNotice.Visibility = Visibility.Visible;
                EmptyNotice.Text += "\nList source is not set.\nLog in or set it manually.";
                Utils.GetMainPageInstance()?.SetStatus("Anime List");
            }
        }

        private async void FetchData()
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            EmptyNotice.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(ListSource.Text))
            {
                EmptyNotice.Visibility = Visibility.Visible;
                EmptyNotice.Text += "\nList source is not set.\nLog in or set it manually.";
            }
            else
            {
                EmptyNotice.Text = "We have come up empty...";
            }

            _allAnimeItems.Clear();
            _animeItems.Clear();
            var possibleData = Utils.GetMainPageInstance()?.RetrieveAnimeEntries(ListSource.Text);
            if (possibleData != null)
                _allAnimeItems = possibleData;
            else
            {
                var args = new AnimeListParameters
                {
                    status = "all",
                    type = "anime",
                    user = ListSource.Text
                };
                var data = await new AnimeListQuery(args).GetRequestResponse();
                XDocument parsedData = XDocument.Parse(data);
                var anime = parsedData.Root.Elements("anime").ToList();
                foreach (var item in anime)
                {
                    _allAnimeItems.Add(new AnimeItem(
                        (Creditentials.Authenticated && ListSource.Text == Creditentials.UserName),
                        item.Element("series_title").Value,
                        item.Element("series_image").Value,
                        Convert.ToInt32(item.Element("series_animedb_id").Value),
                        Convert.ToInt32(item.Element("my_status").Value),
                        Convert.ToInt32(item.Element("my_watched_episodes").Value),
                        Convert.ToInt32(item.Element("series_episodes").Value),
                        Convert.ToInt32(item.Element("my_score").Value)));
                }
                Utils.GetMainPageInstance()?.SaveAnimeEntries(ListSource.Text,_allAnimeItems);
            }


            RefreshList();
            Animes.ItemsSource = _animeItems;
            SpinnerLoading.Visibility = Visibility.Collapsed;

        }

        private int GetDesiredStatus()
        {
            int value = StatusSelector.SelectedIndex;
            value++;
            return (value == 5 || value == 6) ? value + 1 : value;
        }

        public void RefreshList()
        {
            EmptyNotice.Visibility = Visibility.Collapsed;
            string query = Utils.GetMainPageInstance()?.GetSearchQuery();
            _animeItems.Clear();
            var items = _allAnimeItems.Where(item => !string.IsNullOrWhiteSpace(query) || GetDesiredStatus() == 7 || item.status == GetDesiredStatus());
            if (!string.IsNullOrWhiteSpace(query))
                items = items.Where(item => item.title.ToLower().Contains(query.ToLower()));
            if(_sortOption != SortOptions.SortNothing)
                switch (_sortOption)
                {
                    case SortOptions.SortTitle:
                        items = items.OrderBy(item => item.title);
                        break;
                    case SortOptions.SortScore:
                        items = items.OrderBy(item => item.Score);
                        break;
                    case SortOptions.SortWatched:
                        items = items.OrderBy(item => item.WatchedEpisodes);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_sortOption), _sortOption, null);
                }
            if (_sortDescending)
                items = items.Reverse();
            foreach (var item in items)
            {
                item.ItemLoaded();
                _animeItems.Add(item);
            }
            if(_animeItems.Count == 0)
                EmptyNotice.Visibility = Visibility.Visible;
            AlternateRowColors();
            Utils.GetMainPageInstance()?.SetStatus($"{ListSource.Text} - {Utils.StatusToString(GetDesiredStatus())}");
        }

        private void ChangeListStatus(object sender, SelectionChangedEventArgs e)
        {
            if(Animes != null)
                RefreshList();
        }

        private void AlternateRowColors()
        {
            for (int i = 0; i < _animeItems.Count; i++)
            {
                if ((i + 1)%2 == 0)
                    _animeItems[i].Setbackground(new SolidColorBrush(Color.FromArgb(170, 230, 230, 230)));
                else
                    _animeItems[i].Setbackground(new SolidColorBrush(Colors.Transparent));
            }
        }

        private void PinTileMal(object sender, RoutedEventArgs e)
        {
            foreach (var item in Animes.SelectedItems)
            {
                var anime = item as AnimeItem;
                if (SecondaryTile.Exists(anime.Id.ToString()))
                    continue;
                anime.PinTile($"http://www.myanimelist.net/anime/{anime.Id}");
            }
        }

        private void PinTileKiss(object sender, RoutedEventArgs e)
        {
            foreach (var item in Animes.SelectedItems)
            {
                var anime = item as AnimeItem;
                if (SecondaryTile.Exists(anime.Id.ToString()))
                    continue;

                anime.PinTile($"https://kissanime.to/M/Anime/{anime.Name.Replace(' ', '-')}");
            }
        }

        private void RefreshList(object sender, RoutedEventArgs e)
        {
            FetchData();
        }

        private void PinTileCustom(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SelectSortMode(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleMenuFlyoutItem;
            switch (btn.Text)
            {
                case "Title":
                    _sortOption = SortOptions.SortTitle;
                    break;
                case "Score":
                    _sortOption = SortOptions.SortScore;
                    break;
                case "Watched":
                    _sortOption = SortOptions.SortWatched;
                    break;
                default:
                    _sortOption = SortOptions.SortNothing;
                    break;
            }
            sort1.IsChecked = false;
            sort2.IsChecked = false;
            sort3.IsChecked = false;
            sort4.IsChecked = false;
            btn.IsChecked = true;
            RefreshList();

        }

        private void ChangeSortOrder(object sender, RoutedEventArgs e)
        {
            var chbox = sender as ToggleMenuFlyoutItem;
            _sortDescending = chbox.IsChecked;
            RefreshList();
        }

        private void ListSource_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                var txt = sender as TextBox;
                txt.IsEnabled = false; //reset input
                txt.IsEnabled = true;
                FetchData();
            }
        }
    }
}
