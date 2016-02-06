using System;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Pages;

namespace MALClient.Items
{
    public sealed partial class AnimeSearchItem : UserControl, IAnimeData
    {
        private readonly XElement item;


        private Point _initialPoint;

        public AnimeSearchItem()
        {
            InitializeComponent();
        }

        public AnimeSearchItem(XElement animeElement)
        {
            InitializeComponent();
            item = animeElement;
            Id = int.Parse(animeElement.Element("id").Value);
            GlobalScore = float.Parse(animeElement.Element("score").Value);
            AllEpisodes = int.Parse(animeElement.Element("episodes").Value);
            Title = animeElement.Element("title").Value;
            Type = animeElement.Element("type").Value;
            Status = animeElement.Element("status").Value;

            TxtTitle.Text = Title;
            TxtGlobalScore.Text = GlobalScore.ToString();
            if (Status == "Currently Airing")
                SymbolAiring.Visibility = Visibility.Visible;
            Img.Source = new BitmapImage(new Uri(animeElement.Element("image").Value));
            WatchedEps.Text = $"Episodes : {AllEpisodes}";
        }

        private string Type { get; set; }
        private string Status { get; }

        public int Id { get; set; }
        public float GlobalScore { get; set; }
        public int AllEpisodes { get; set; }

        public string Title { get; set; }

        //They must be here because reasons (interface reasons)
        public int MyEpisodes { get; set; }
        public int MyScore { get; set; }
        public int MyStatus { get; set; }

        private void ManipStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            _initialPoint = e.Position;
        }

        private async void ManipDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (!e.IsInertial || !(e.Position.X - _initialPoint.X >= 70)) return;
            if (!(e.Position.X - _initialPoint.X >= 70)) return;
            await
                Utils.GetMainPageInstance()
                    .Navigate(PageIndex.PageAnimeDetails, new AnimeDetailsPageNavigationArgs(0, "", item, this));
            e.Complete();
        }

        public void Setbackground(SolidColorBrush brush) //Used to alternate rows
        {
            Root.Background = brush;
        }

        private async void NavigateDetails(object sender, RoutedEventArgs e)
        {
            await
                Utils.GetMainPageInstance()
                    .Navigate(PageIndex.PageAnimeDetails, new AnimeDetailsPageNavigationArgs(0, "", item, this));
        }
    }
}