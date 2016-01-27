using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using MALClient.Items;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{

    public class AnimeDetailsPageNavigationArgs
    {
        public int Id;
        public string Title;
        public XElement AnimeElement;
        public AnimeListPageNavigationArgs PrevListSetup;
        public IAnimeData AnimeItem;

        public AnimeDetailsPageNavigationArgs(int id, string title, XElement element,IAnimeData animeReference, AnimeListPageNavigationArgs args = null)
        {
            Id = id;
            Title = title;
            AnimeElement = element;
            PrevListSetup = args;
            AnimeItem = animeReference;
        }
    }

    public sealed partial class AnimeDetailsPage : Page
    {
        public int Id => _animeItemReference.Id;
        public string Title => _animeItemReference.Title;
        public int AllEpisodes => _animeItemReference.AllEpisodes;
        private float _globalScore;
        public float GlobalScore
        {
            get { return _globalScore; }
            set
            {
                _animeItemReference.GlobalScore = value;
                _globalScore = value;
            }
        }

        public string Type { get; set; }
        public string Status { get; set; }
        public string Synopsis { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
        private string _imgUrl;

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

        private string _origin;
        private IAnimeData _animeItemReference;
        private AnimeListPageNavigationArgs _previousPageSetup;

        public AnimeDetailsPage()
        {
            this.InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var param = e.Parameter as AnimeDetailsPageNavigationArgs;
            if (param == null)
                throw new Exception("No paramaters for this page");

            _animeItemReference = param.AnimeItem;
            if (_animeItemReference is AnimeSearchItem) //if we are from search let's look for abstraction
            {
                if (!Utils.GetMainPageInstance()
                    .TryRetieveAuthenticatedAnimeItem(_animeItemReference.Id, ref _animeItemReference))
                    // else we don't have this item
                {
                    //we may only prepare for its creation
                    BtnAddAnime.Visibility = Visibility.Visible;
                    MyDetails.Visibility = Visibility.Collapsed;
                }
            } // else we already have it

            if(_animeItemReference is AnimeItem && (_animeItemReference as AnimeItem).Auth)
            {
                //we have item on the list , so there's valid data here
                MyDetails.Visibility = Visibility.Visible;
                BtnAddAnime.Visibility = Visibility.Collapsed;
                BtnWatchedEps.Content = $"{MyEpisodes}/{(AllEpisodes == 0 ? "?" : AllEpisodes.ToString())}";
                BtnStatus.Content = Utils.StatusToString(MyStatus);
                BtnScore.Content = MyScore == 0 ? "Unranked" : $"{MyScore}/10";
            }

            if (param.AnimeElement != null)
            {
                PopulateData(param.AnimeElement);
                _origin = "Search";
            }
            else
            {
                FetchData(param.Id.ToString(), param.Title);
                _previousPageSetup = param.PrevListSetup;
                _origin = "List";
            }

            if (_origin == "Search")
                Utils.RegisterBackNav(PageIndex.PageSearch, true);
            else
                Utils.RegisterBackNav(PageIndex.PageAnimeList, _previousPageSetup);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Utils.DeregisterBackNav();
        }



        #region ChangeStuff
        private async void ChangeStatus(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            var item = sender as MenuFlyoutItem;
            int prevStatus = MyStatus;
            MyStatus = Utils.StatusToInt(item.Text);
            string response = await new AnimeUpdateQuery(Id, MyEpisodes, MyStatus, MyScore).GetRequestResponse();
            if (response != "Updated")
                MyStatus = prevStatus;

            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private async void ChangeScore(object sender, RoutedEventArgs e)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            var btn = sender as MenuFlyoutItem;
            int prevScore = MyScore;
            MyScore = int.Parse(btn.Text.Split('-').First());
            string response = await new AnimeUpdateQuery(Id, MyEpisodes, MyStatus, MyScore).GetRequestResponse();
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
                return;
            }
            string response = await new AnimeUpdateQuery(Id, eps, MyStatus, MyScore).GetRequestResponse();
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
            ChangeWatchedEps(null,null);
        }
        #endregion

        #region Add/Remove
        private async void AddAnime(object sender, RoutedEventArgs e)
        {
            string response = await new AnimeAddQuery(Id.ToString()).GetRequestResponse();
            if(!response.Contains("Created"))
                return;
            BtnAddAnime.Visibility = Visibility.Collapsed;
            var animeItem = new AnimeItemAbstraction(true,Title,_imgUrl,Id, 6,0, AllEpisodes,0);
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
            bool uSure = false;
            var msg = new MessageDialog("Are you sure about deleting this entry from your list?");
            msg.Commands.Add(new UICommand("I'm sure", command => uSure = true));
            msg.Commands.Add(new UICommand("Cancel", command => uSure = false));
            await msg.ShowAsync();
            if(!uSure)
                return;

            string response = await new AnimeRemoveQuery(Id.ToString()).GetRequestResponse();
            if(!response.Contains("Deleted"))
                return;
            
            Utils.GetMainPageInstance().RemoveAnimeEntry(Creditentials.UserName, (_animeItemReference as AnimeItem)._parentAbstraction);

            (_animeItemReference as AnimeItem).SetAuthStatus(false,true);
            BtnAddAnime.Visibility = Visibility.Visible;
            MyDetails.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region FetachAndPopulate
        private void PopulateData(XElement animeElement)
        {
            GlobalScore = float.Parse(animeElement.Element("score").Value);
            Type = animeElement.Element("type").Value;
            Status = animeElement.Element("status").Value;
            if (Status == "Currently Airing" && _animeItemReference is AnimeItem)
                ((AnimeItem) _animeItemReference).Airing = true;
            Synopsis = Regex.Replace(animeElement.Element("synopsis").Value, @"<[^>]+>|&nbsp;", "").Trim().Replace("[i]", "").Replace("[/i]", "");
            StartDate = animeElement.Element("start_date").Value;
            EndDate = animeElement.Element("end_date").Value;
            _imgUrl = animeElement.Element("image").Value;

            DetailScore.Text = GlobalScore.ToString();
            DetailEpisodes.Text = AllEpisodes.ToString();

            DetailBroadcast.Text = StartDate + "\n" + EndDate;
            DetailStatus.Text = Status;
            DetailType.Text = Type;
            DetailSynopsis.Text = Synopsis;

            Utils.GetMainPageInstance().SetStatus(Title);

            DetailImage.Source = new BitmapImage(new Uri(_imgUrl));
            
        }



        private async void FetchData(string id, string title)
        {
            string data = await new AnimeSearchQuery(title.Replace(' ', '+')).GetRequestResponse();
            data = WebUtility.HtmlDecode(data);
            data = data.Replace("&mdash", "").Replace("&rsquo", "").Replace("&", "");

            XDocument parsedData = XDocument.Parse(data);
            var elements = parsedData.Element("anime").Elements("entry");
            PopulateData(elements.First(element => element.Element("id").Value == id));
        }
        #endregion



        private async void OpenMalPage(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri($"http://myanimelist.net/anime/{Id}"));
        }


    }



}
