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
using MALClient.Items;

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
                items.Add(abstraction.AnimeItem);
            }
            Animes.ItemsSource = items;
        }
    }
}
