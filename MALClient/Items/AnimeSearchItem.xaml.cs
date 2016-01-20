using System;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Pages;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class AnimeSearchItem : UserControl , IAnimeData
    {
        private XElement item;

        public AnimeSearchItem()
        {
            this.InitializeComponent();
        }

        public AnimeSearchItem(XElement animeElement)
        {
            this.InitializeComponent();
            this.item = animeElement;
            Id = int.Parse(animeElement.Element("id").Value);
            GlobalScore = float.Parse(animeElement.Element("score").Value);
            AllEpisodes = int.Parse(animeElement.Element("episodes").Value);
            Title = animeElement.Element("title").Value;
            Type = animeElement.Element("type").Value;
            Status = animeElement.Element("status").Value;

            TxtTitle.Text = Title;
            TxtScore.Text = GlobalScore.ToString();
            Img.Source = new BitmapImage(new Uri(animeElement.Element("image").Value));
            WatchedEps.Text = AllEpisodes.ToString();

        }

        public int Id { get; set; }
        public float GlobalScore { get; set; }
        public int AllEpisodes { get; set; }

        public string Title { get; set; }
        public string Type { get; set; }
        public string Status { get; private set; }

        //They must be here because reasons (interface reasons)
        public int MyEpisodes { get; set; }
        public int MyScore { get; set; }
        public int MyStatus { get; set; }


        private Point initialpoint;

        private void ManipStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            initialpoint = e.Position;
        }

        private void ManipDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial)
            {
                Point currentpoint = e.Position;
                if (currentpoint.X - initialpoint.X >= 70) //left
                {
                    Utils.GetMainPageInstance().Navigate(PageIndex.PageAnimeDetails,new AnimeDetailsPageNavigationArgs(0,"",item,this));
                    e.Complete();
                }
            }
        }


        public void Setbackground(SolidColorBrush brush)
        {
            Root.Background = brush;
        }
    }
}
