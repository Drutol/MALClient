using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using MALClient.Items;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>



    public sealed partial class AnimeDetailsPage : Page
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public float Score { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Synopsis { get; set; }
        public int Episodes { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        private string _imgUrl;

        public int WatchedEps;
        public int MyStatus;
        public int MyScore;

        private bool _isOnAuthList = false;
        private AnimeItem _animeItemReference;

        private string _origin;

        public AnimeDetailsPage()
        {
            this.InitializeComponent();
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += CurrentViewOnBackRequested;

        }

        private async void ChangeStatus(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            var item = sender as MenuFlyoutItem;
            MyStatus = Utils.StatusToInt(item.Text);
            string response = await new AnimeUpdateQuery(Id,WatchedEps,MyStatus,MyScore).GetRequestResponse();
            if (response == "Updated")
            {
                BtnStatus.Content = item.Text;
            }
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private async void ChangeScore(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            var btn = sender as MenuFlyoutItem;
            MyScore = int.Parse(btn.Text.Split('-').First());
            string response = await new AnimeUpdateQuery(Id, WatchedEps, MyStatus, MyScore).GetRequestResponse();
            if (response == "Updated")
            {
                
                BtnScore.Content = Score > 0 ? $"{Score}/10" : "Unranked";
            }
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private async void ChangeWatched(object sender, RoutedEventArgs routedEventArgs)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            var btn = sender as MenuFlyoutItem;
            WatchedEps = int.Parse(btn.Text);
            string response = await new AnimeUpdateQuery(Id, WatchedEps, MyStatus, MyScore).GetRequestResponse();
            if (response == "Updated")
            {
                BtnWatched.Content = $"{WatchedEps}/{Episodes}";
            }
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private async void AddAnime(object sender, RoutedEventArgs e)
        {
            string response = await new AnimeAddQuery(Id.ToString()).GetRequestResponse();
            if(!response.Contains("Created"))
                return;
            BtnAddAnime.Visibility = Visibility.Collapsed;
            var animeItem = new AnimeItem(
                        true,
                        Title,
                        _imgUrl,
                        Id,
                        6,
                        0,
                        Episodes,
                        0);
            _isOnAuthList = true;
            MyScore = 0;
            MyStatus = 6;
            WatchedEps = 0;
            _animeItemReference = animeItem;
            Utils.GetMainPageInstance().AddAnimeEntry(Creditentials.UserName, animeItem);
            MyDetails.Visibility = Visibility.Visible;
            BtnScore.Content = $"{MyScore}";
            BtnStatus.Content = $"{Utils.StatusToString(MyStatus)}";
            for (int i = 0; i <= Episodes; i++)
            {
                var item = new MenuFlyoutItem();
                item.Text = i.ToString();
                item.Click += ChangeWatched;
                BtnWatchedMenuFlyout.Items.Add(item);
            }
            BtnWatched.Content = $"{WatchedEps}/{Episodes}";
        }

        private async void RemoveAnime(object sender, RoutedEventArgs e)
        {
            string response = await new AnimeRemoveQuery(Id.ToString()).GetRequestResponse();
            if(!response.Contains("Deleted"))
                return;
            BtnAddAnime.Visibility = Visibility.Visible;
            Utils.GetMainPageInstance().RemoveAnimeEntry(Creditentials.UserName,_animeItemReference);
            MyDetails.Visibility = Visibility.Collapsed;
        }


        private void CurrentViewOnBackRequested(object sender, BackRequestedEventArgs args)
        {
            args.Handled = true;
            var currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            currentView.BackRequested -= CurrentViewOnBackRequested;
            if (_origin == "Search")
                Utils.GetMainPageInstance().NavigateSearch(true);
            else
                Utils.GetMainPageInstance().NavigateList();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var param = e.Parameter as AnimeDetailsPageNavigationArgs;
            if (param == null)
                return;
            if (param.AnimeElement != null)
            {
                PopulateData(param.AnimeElement);
                _origin = "Search";
            }
            else
            {
                FetchData(param.Id.ToString(), param.Title);
                _origin = "List";
            }
        }

        private void PopulateData(XElement animeElement)
        {
            Id = int.Parse(animeElement.Element("id").Value);
            Score = float.Parse(animeElement.Element("score").Value);
            Episodes = int.Parse(animeElement.Element("episodes").Value);
            Title = animeElement.Element("title").Value;
            Type = animeElement.Element("type").Value;
            Status = animeElement.Element("status").Value;
            Synopsis = Regex.Replace(animeElement.Element("synopsis").Value, @"<[^>]+>|&nbsp;", "").Trim().Replace("[i]","").Replace("[/i]",""); 
            StartDate = animeElement.Element("start_date").Value;
            EndDate = animeElement.Element("end_date").Value;
            _imgUrl = animeElement.Element("image").Value;

            DetailScore.Text = Score.ToString();
            DetailEpisodes.Text = Episodes.ToString();

            DetailBroadcast.Text = StartDate;
            DetailStatus.Text = Status;
            DetailType.Text = Type;
            DetailSynopsis.Text = Synopsis;

            Utils.GetMainPageInstance().SetStatus(Title);

            DetailImage.Source = new BitmapImage(new Uri(_imgUrl));

            if (Utils.GetMainPageInstance().TryRetrieveListItem(Id, ref WatchedEps, ref MyStatus, ref MyScore, ref _animeItemReference))
            {
                _isOnAuthList = true;
                BtnAddAnime.Visibility = Visibility.Collapsed;
                MyDetails.Visibility = Visibility.Visible;
                BtnScore.Content = $"{MyScore}";
                BtnStatus.Content = $"{Utils.StatusToString(MyStatus)}";
                for (int i = 0; i <= Episodes; i++)
                {
                    var item = new MenuFlyoutItem();
                    item.Text = i.ToString();
                    item.Click += ChangeWatched;
                    BtnWatchedMenuFlyout.Items.Add(item);
                }
                BtnWatched.Content = $"{WatchedEps}/{Episodes}";
            }
        }



        private async void FetchData(string id,string title)
        {
            string data = await new AnimeSearchQuery(title.Replace(' ','+')).GetRequestResponse();
            data = WebUtility.HtmlDecode(data);
            data = data.Replace("&mdash", "").Replace("&rsquo","").Replace("&","");

            XDocument parsedData = XDocument.Parse(data);
            var elements = parsedData.Element("anime").Elements("entry");
            PopulateData(elements.First(element => element.Element("id").Value == id));
        }



    }


    public class AnimeDetailsPageNavigationArgs
    {
        public int Id;
        public string Title;
        public XElement AnimeElement;

        public AnimeDetailsPageNavigationArgs(int id, string title, XElement element)
        {
            Id = id;
            Title = title;
            AnimeElement = element;
        }
    }
}
