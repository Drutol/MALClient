using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public class AnimeDetailsPageNavigationArgs
    {
        public int Id;
        public XElement AnimeElement;

        public AnimeDetailsPageNavigationArgs(int id,XElement element)
        {
            Id = id;
            AnimeElement = element;
        }
    }

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


        public AnimeDetailsPage(int id)
        {
            this.InitializeComponent();
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var param = e.Parameter as AnimeDetailsPageNavigationArgs;
            if(param.AnimeElement != null)
                PopulateData(param.AnimeElement);
            else
                FetchData(param.Id.ToString());
        }

        private void PopulateData(XElement animeElement)
        {
            Id = int.Parse(animeElement.Element("id").Value);
            Score = float.Parse(animeElement.Element("score").Value);
            Episodes = int.Parse(animeElement.Element("episodes").Value);
            Title = animeElement.Element("title").Value;
            Type = animeElement.Element("type").Value;
            Status = animeElement.Element("status").Value;
            Synopsis = animeElement.Element("synopsis").Value;
            StartDate = animeElement.Element("start_date").Value;
            EndDate = animeElement.Element("end_date").Value;
            _imgUrl = animeElement.Element("image").Value;

            DetailScore.Text = Score.ToString();
            DetailEpisodes.Text = Episodes.ToString();

            DetailBroadcast.Text = StartDate;
            DetailStatus.Text = Status;
            DetailType.Text = Type;
            DetailTitle.Text = Title;
            DetailSynopsis.Text = Title;

            DetailImage.Source = new BitmapImage(new Uri(_imgUrl));
        }

        private async void FetchData(string id)
        {
            string data = await new AnimeSearchQuery(id).GetRequestResponse();
            XDocument parsedData = XDocument.Parse(data);
            var elements = parsedData.Elements("entry");
            PopulateData(elements.First(element => element.Element("id").Value == id));
        }
    }
}
