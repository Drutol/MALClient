using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using MALClient.Items;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    public class AnimeDetailsPageNavigationArgs
    {
        public readonly XElement AnimeElement;
        public readonly IAnimeData AnimeItem;
        public readonly int Id;
        public readonly object PrevPageSetup;
        public readonly string Title;
        public PageIndex Source;

        public AnimeDetailsPageNavigationArgs(int id, string title, XElement element, IAnimeData animeReference,
            object args = null)
        {
            Id = id;
            Title = title;
            AnimeElement = element;
            PrevPageSetup = args;
            AnimeItem = animeReference;
        }
    }

    public sealed partial class AnimeDetailsPage : Page
    {
        private IAnimeData _animeItemReference;
        private float _globalScore;
        private string _imgUrl;
        private ObservableCollection<ListViewItem> _loadedItems1 = new ObservableCollection<ListViewItem>();
        private ObservableCollection<ListViewItem> _loadedItems2 = new ObservableCollection<ListViewItem>();

        public AnimeDetailsPage()
        {
            InitializeComponent();
        }

        private int _allEpisodes;


        private int Id { get; set; }
        private string Title { get; set; }

        private int AllEpisodes
        {
            get
            {
                return _animeItemReference?.AllEpisodes ?? _allEpisodes;
            }
            set { _allEpisodes = value; }
        } 
        private string Type { get; set; }
        private string Status { get; set; }
        private string Synopsis { get; set; }
        private string StartDate { get; set; }
        private string EndDate { get; set; }

        private float GlobalScore
        {
            get { return _globalScore; }
            set
            {
                if(_animeItemReference != null)
                _animeItemReference.GlobalScore = value;
                _globalScore = value;
            }
        }

        private int MyEpisodes
        {
            get { return _animeItemReference.MyEpisodes; }
            set
            {
                _animeItemReference.MyEpisodes = value;
                BtnWatchedEps.Content = $"{value}/{(AllEpisodes == 0 ? "?" : AllEpisodes.ToString())}";
            }
        }

        private int MyStatus
        {
            get { return _animeItemReference.MyStatus; }
            set
            {
                _animeItemReference.MyStatus = value;
                BtnStatus.Content = Utils.StatusToString(value);
            }
        }

        private int MyScore
        {
            get { return _animeItemReference.MyScore; }
            set
            {
                _animeItemReference.MyScore = value;
                BtnScore.Content = $"{value}/10";
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var param = e.Parameter as AnimeDetailsPageNavigationArgs;
            if (param == null)
                throw new Exception("No paramaters for this page");

            Id = param.Id;
            Title = param.Title;
            _animeItemReference = param.AnimeItem;
            if (_animeItemReference == null || _animeItemReference is AnimeSearchItem || !(_animeItemReference as AnimeItem).Auth)
                //if we are from search or from unauthenticated item let's look for proper abstraction
            {
                if (!Utils.GetMainPageInstance()
                    .TryRetrieveAuthenticatedAnimeItem(param.Id, ref _animeItemReference))
                    // else we don't have this item
                {
                    //we may only prepare for its creation
                    BtnAddAnime.Visibility = Visibility.Visible;
                    MyDetails.Visibility = Visibility.Collapsed;
                }
            } // else we already have it

            if (_animeItemReference is AnimeItem && (_animeItemReference as AnimeItem).Auth)
            {
                //we have item on the list , so there's valid data here
                MyDetails.Visibility = Visibility.Visible;
                BtnAddAnime.Visibility = Visibility.Collapsed;
                BtnWatchedEps.Content = $"{MyEpisodes}/{(AllEpisodes == 0 ? "?" : AllEpisodes.ToString())}";
                BtnStatus.Content = Utils.StatusToString(MyStatus);
                BtnScore.Content = MyScore == 0 ? "Unranked" : $"{MyScore}/10";
            }

            switch (param.Source)
            {
                case PageIndex.PageSearch:
                    ExtractData(param.AnimeElement);
                    Utils.RegisterBackNav(param.Source, true);
                    break;
                case PageIndex.PageAnimeList:
                    FetchData(param.Id.ToString(), param.Title);
                    Utils.RegisterBackNav(param.Source, param.PrevPageSetup);
                    break;
                case PageIndex.PageRecomendations:
                    ExtractData(param.AnimeElement);
                    Utils.RegisterBackNav(param.Source,param.PrevPageSetup);
                    break;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Utils.DeregisterBackNav();
        }

        private async void OpenMalPage(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri($"http://myanimelist.net/anime/{Id}"));
        }

        #region ChangeStuff

        private async void ChangeStatus(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            var item = sender as MenuFlyoutItem;
            var prevStatus = MyStatus;
            MyStatus = Utils.StatusToInt(item.Text);
            var response = await new AnimeUpdateQuery(Id, MyEpisodes, MyStatus, MyScore).GetRequestResponse();
            if (response != "Updated")
                MyStatus = prevStatus;

            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private async void ChangeScore(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            var btn = sender as MenuFlyoutItem;
            var prevScore = MyScore;
            MyScore = int.Parse(btn.Text.Split('-').First());
            var response = await new AnimeUpdateQuery(Id, MyEpisodes, MyStatus, MyScore).GetRequestResponse();
            if (response != "Updated")
                MyScore = prevScore;
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private async void ChangeWatchedEps(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            int eps;
            if (!int.TryParse(TxtBoxWatchedEps.Text, out eps))
            {
                TxtWatchedInvalidInputNotice.Visibility = Visibility.Visible;
                SpinnerLoading.Visibility = Visibility.Collapsed;
                return;
            }
            var response = await new AnimeUpdateQuery(Id, eps, MyStatus, MyScore).GetRequestResponse();
            if (response == "Updated")
            {
                MyEpisodes = eps;
                WatchedEpsFlyout.Hide();
                TxtWatchedInvalidInputNotice.Visibility = Visibility.Collapsed;
            }
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private void SubmitWatchedEps(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                ChangeWatchedEps(null, null);
        }

        #endregion

        #region Add/Remove

        private async void AddAnime(object sender, RoutedEventArgs e)
        {
            var response = await new AnimeAddQuery(Id.ToString()).GetRequestResponse();
            if (!response.Contains("Created"))
                return;
            BtnAddAnime.Visibility = Visibility.Collapsed;
            var animeItem = new AnimeItemAbstraction(true, Title, _imgUrl, Id, 6, 0, AllEpisodes, 0);
            _animeItemReference = animeItem.AnimeItem;
            MyScore = 0;
            MyStatus = 6;
            MyEpisodes = 0;
            GlobalScore = GlobalScore; //trigger setter of anime item
            if (Status == "Currently Airing")
                (_animeItemReference as AnimeItem).Airing = true;
            Utils.GetMainPageInstance().AddAnimeEntry(Creditentials.UserName, animeItem);
            MyDetails.Visibility = Visibility.Visible;
        }

        private async void RemoveAnime(object sender, RoutedEventArgs e)
        {
            var uSure = false;
            var msg = new MessageDialog("Are you sure about deleting this entry from your list?");
            msg.Commands.Add(new UICommand("I'm sure", command => uSure = true));
            msg.Commands.Add(new UICommand("Cancel", command => uSure = false));
            await msg.ShowAsync();
            if (!uSure)
                return;

            var response = await new AnimeRemoveQuery(Id.ToString()).GetRequestResponse();
            if (!response.Contains("Deleted"))
                return;

            Utils.GetMainPageInstance()
                .RemoveAnimeEntry(Creditentials.UserName, (_animeItemReference as AnimeItem)._parentAbstraction);

            (_animeItemReference as AnimeItem).SetAuthStatus(false, true);
            BtnAddAnime.Visibility = Visibility.Visible;
            MyDetails.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region FetchAndPopulate

        private void PopulateData()
        {
            if (_animeItemReference is AnimeItem)
            {
                var day = Status == "Currently Airing" ? (int)DateTime.Parse(StartDate).DayOfWeek + 1 : -1;
                DataCache.RegisterVolatileData(Id, new VolatileDataCache
                {
                    DayOfAiring = day,
                    GlobalScore = GlobalScore
                });
                ((AnimeItem)_animeItemReference).Airing = day != -1;
                DataCache.SaveVolatileData();
            }
            _loadedItems1.Add(BuildListViewItem("Episodes", AllEpisodes.ToString(),true));
            _loadedItems1.Add(BuildListViewItem("Score", GlobalScore.ToString()));
            _loadedItems1.Add(BuildListViewItem("Start", StartDate,true));
            _loadedItems2.Add(BuildListViewItem("Type", Type,true, 0.3f, 0.7f));
            _loadedItems2.Add(BuildListViewItem("Status", Status,false,0.3f,0.7f));      
            _loadedItems2.Add(BuildListViewItem("End", EndDate,true, 0.3f, 0.7f));

            DetailSynopsis.Text = Synopsis;

            Utils.GetMainPageInstance().CurrentStatus = Title;

            DetailImage.Source = new BitmapImage(new Uri(_imgUrl));
            DetailsListViewP1.ItemsSource = _loadedItems1;
            DetailsListViewP2.ItemsSource = _loadedItems2;
        }

        private ListViewItem BuildListViewItem(string label, string val1,bool alternate = false,float left = 0.4f,float right = 0.6f)
        {
            return new ListViewItem
            {
                Content = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition {Width = new GridLength(left,GridUnitType.Star)},
                        new ColumnDefinition {Width = new GridLength(right,GridUnitType.Star)}
                    },
                    Children =
                    {
                        BuildTextBlock(label,FontWeights.SemiBold,0),
                        BuildTextBlock(val1,FontWeights.SemiLight,1),
                    },
                },
                Background = new SolidColorBrush(alternate ? Color.FromArgb(170, 230, 230, 230) : Colors.Transparent),
                Height = 20
            };
        }

        private TextBlock BuildTextBlock(string value, FontWeight weight, int column)
        {
            var txt = new TextBlock
            {
                Text = value,
                FontWeight = weight,
                FontSize = 13,
                Height = 20,
                TextAlignment = !weight.Equals(FontWeights.SemiBold) ? TextAlignment.Center : TextAlignment.Left              
            };
            txt.SetValue(Grid.ColumnProperty, column);
            return txt;
        }

        private void ExtractData(XElement animeElement)
        {
            GlobalScore = float.Parse(animeElement.Element("score").Value);
            Type = animeElement.Element("type").Value;
            Status = animeElement.Element("status").Value;
            Synopsis =
                Regex.Replace(animeElement.Element("synopsis").Value, @"<[^>]+>|&nbsp;", "")
                    .Trim()
                    .Replace("[i]", "")
                    .Replace("[/i]", "")
                    .Replace("#039;","'")
                    .Replace("quot;","\"")
                    .Replace("&mdash;","—");            
            StartDate = animeElement.Element("start_date").Value;
            EndDate = animeElement.Element("end_date").Value;
            _imgUrl = animeElement.Element("image").Value;
            if (_animeItemReference == null)
                AllEpisodes = Convert.ToInt32(animeElement.Element("episodes").Value);
            PopulateData();
        }

        private async void FetchData(string id, string title)
        {
            var data = await new AnimeSearchQuery(Utils.CleanAnimeTitle(title)).GetRequestResponse();
            data = WebUtility.HtmlDecode(data);
            data = data.Replace("&mdash", "").Replace("&rsquo", "").Replace("&", "");

            XDocument parsedData = XDocument.Parse(data);
            var elements = parsedData.Element("anime").Elements("entry");
            ExtractData(elements.First(element => element.Element("id").Value == id));
        }

        #endregion
    }
}