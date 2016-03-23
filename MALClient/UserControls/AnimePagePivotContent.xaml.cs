using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using MALClient.Items;
using MALClient.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UserControls
{
    public sealed partial class AnimePagePivotContent : UserControl
    {
        private readonly IEnumerable<AnimeItemAbstraction> _content;
        private bool _loaded;

        public AnimePagePivotContent(IEnumerable<AnimeItemAbstraction> content)
        {
            InitializeComponent();
            _content = content;
            Loaded += (sender, args) =>
            {
                var scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(Animes, 0), 0) as ScrollViewer;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            };
        }

        public async void LoadContent()
        {
            if (_loaded)
                return;
            _loaded = true;

            var items = new ObservableCollection<AnimeItem>();
            foreach (var abstraction in _content)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High,
                    () => { items.Add(abstraction.AnimeItem); });
            }
            Animes.ItemsSource = items;
        }

        private async void Animes_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
            //var item = e.AddedItems.First() as AnimeItem;
            await Task.Delay(1);
            (e.AddedItems.First() as AnimeItem).ViewModel.NavigateDetails();
            //ViewModelLocator.AnimeList.CurrentlySelectedAnimeItem = item;

            //item.ViewModel.NavigateDetails();
        }

        public void ResetSelection()
        {
            Animes.SelectedItem = null;
        }
    }
}