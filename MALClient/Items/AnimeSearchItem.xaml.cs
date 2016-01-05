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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class AnimeSearchItem : UserControl
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
            Score = float.Parse(animeElement.Element("score").Value);
            Episodes = int.Parse(animeElement.Element("episodes").Value);
            Title = animeElement.Element("title").Value;
            Type = animeElement.Element("type").Value;
            Status = animeElement.Element("status").Value;

            TxtTitle.Text = Title;
            TxtScore.Text = Score.ToString();
            Img.Source = new BitmapImage(new Uri(animeElement.Element("image").Value));
            WatchedEps.Text = Episodes.ToString();

        }

        public int Id { get; set; }
        public float Score { get; set; }
        public int Episodes { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Status { get; private set; }

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
                    Utils.GetMainPageInstance().NavigateDetails(item);
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
