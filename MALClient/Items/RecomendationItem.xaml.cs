using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class RecomendationItem : UserControl
    {
        private RecomendationData _data; 

        public RecomendationItem(RecomendationData data)
        {
            this.InitializeComponent();
            _data = data;
            PopulateData();
        }

        private async void PopulateData()
        {
            DepImg.Source = new BitmapImage(new Uri(_data.DependentImgUrl));
            RecImg.Source = new BitmapImage(new Uri(_data.RecommendationImgUrl));
        }
    }
}
