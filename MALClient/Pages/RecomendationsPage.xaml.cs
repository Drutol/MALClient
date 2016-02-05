using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using MALClient.Items;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecomendationsPage : Page
    {
        private ObservableCollection<PivotItem> _recomendationItems = new ObservableCollection<PivotItem>(); 

        public RecomendationsPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var data = await new AnimeRecomendationsQuery().GetRecomendationsData();
            await data[0].FetchData();
            await data[1].FetchData();
            _recomendationItems.Add(new PivotItem
            {
                Header = "lol",
                Content = new RecomendationItem(data[0])
            });
            _recomendationItems.Add(new PivotItem
            {
                Header = "gfh",
                Content = new RecomendationItem(data[1])
            });
            Pivot.ItemsSource = _recomendationItems;
        }
    }
}
