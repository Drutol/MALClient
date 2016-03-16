using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private IEnumerable<AnimeItemAbstraction> _content;
        private bool _loaded = false;
        public AnimePagePivotContent(IEnumerable<AnimeItemAbstraction> content)
        {
            this.InitializeComponent();
            _content = content;
            Loaded += (sender, args) =>
            {
                var scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(Animes, 0), 0) as ScrollViewer;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            };

        }

        public void LoadContent()
        {
            if(_loaded)
                return;
            _loaded = true;

            var items = new ObservableCollection<AnimeItem>();
            foreach (var abstraction in _content)
            {
                 CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High,
                    () => { items.Add(abstraction.AnimeItem); });               
            }
            Animes.ItemsSource = items;
        }

        private void Animes_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count == 0)
                return;           
            ViewModelLocator.AnimeList.CurrentlySelectedAnimeItem = e.AddedItems.First() as AnimeItem;
        }

        public void ResetSelection()
        {
            Animes.SelectedItem = null;
        }
    }
}
