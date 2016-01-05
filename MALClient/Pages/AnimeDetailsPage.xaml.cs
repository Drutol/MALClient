using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;

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

        private string _origin;

        public AnimeDetailsPage()
        {
            this.InitializeComponent();
            var currentView = SystemNavigationManager.GetForCurrentView();

            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            currentView.BackRequested += (sender, args) =>
            {
                args.Handled = true;
                currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                if(_origin == "Search")
                    Utils.GetMainPageInstance().NavigateSearch(true);
                else
                    Utils.GetMainPageInstance().NavigateList();
            };
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
            Synopsis = Regex.Replace(animeElement.Element("synopsis").Value, @"<[^>]+>|&nbsp;", "").Trim(); 
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
