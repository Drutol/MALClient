using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
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
        private ObservableCollection<ListViewItem> _detailItems = new ObservableCollection<ListViewItem>(); 
        public int Index { get; private set; }
        private bool _dataLoaded;
        public RecomendationItem(RecomendationData data)
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
            _data = data;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                var scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(DetailsListView, 0), 0) as ScrollViewer;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
            catch (Exception)
            {
                //
            }
        }


        public async Task PopulateData()
        {
            if (_dataLoaded)
                return;
            SpinnerLoading.Visibility = Visibility.Visible;
            await _data.FetchData();
            DepImg.Source = new BitmapImage(new Uri(_data.DependentImgUrl));
            RecImg.Source = new BitmapImage(new Uri(_data.RecommendationImgUrl));
            TxtDepTitle.Text = _data.DependentTitle;
            TxtRecTitle.Text = _data.RecommendationTitle;
            TxtRecommendation.Text = _data.Description;
            _detailItems.Add(BuildListViewItem("Episodes", _data.DependentEpisodes, _data.RecommendationEpisodes));
            _detailItems.Add(BuildListViewItem("Score", _data.DependentGlobalScore.ToString(), _data.RecommendationGlobalScore.ToString()));
            _detailItems.Add(BuildListViewItem("Type", _data.DependentType, _data.RecommendationType));
            _detailItems.Add(BuildListViewItem("Status", _data.DependentStatus, _data.RecommendationStatus));
            _detailItems.Add(BuildListViewItem("Start:", _data.DependentStartDate, _data.RecommendationStartDate));
            _detailItems.Add(BuildListViewItem("End:", _data.DependentEndDate, _data.RecommendationStartDate));
            DetailsListView.ItemsSource = _detailItems;
            _dataLoaded = true;
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }

        private ListViewItem BuildListViewItem(string label, string val1, string val2)
        {
            return new ListViewItem
            {
                Content = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition {Width = new GridLength(0.24,GridUnitType.Star)},
                        new ColumnDefinition {Width = new GridLength(0.38,GridUnitType.Star)},
                        new ColumnDefinition {Width = new GridLength(0.38,GridUnitType.Star)}
                    },
                    Children =
                    {
                        BuildTextBlock(label,FontWeights.SemiBold,0),
                        BuildTextBlock(val1,FontWeights.SemiLight,1),
                        BuildTextBlock(val2,FontWeights.SemiLight,2)
                    },                   
                },
                Background = new SolidColorBrush((_detailItems.Count + 1) % 2 == 0 ? Color.FromArgb(170, 230, 230, 230) : Colors.Transparent)
            };
        }

        private TextBlock BuildTextBlock(string value,FontWeight weight,int column)
        {
            var txt = new TextBlock
            {
                Text = value,
                FontWeight = weight,
                TextAlignment = !weight.Equals(FontWeights.SemiBold) ? TextAlignment.Center : TextAlignment.Left
            };
            txt.SetValue(Grid.ColumnProperty,column);
            return txt;
        }

        private void ButtonRecomDetails_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonDependentDetails_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
