using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MALClient.XShared.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Shared.Items
{
    public sealed partial class AnimeCompactItem : UserControl
    {
        public static readonly DependencyProperty DisplayContextProperty =
            DependencyProperty.Register("DisplayContext", typeof(AnimeItemDisplayContext), typeof(AnimeCompactItem),
                new PropertyMetadata(AnimeItemDisplayContext.AirDay));

        public AnimeItemDisplayContext DisplayContext
        {
            get { return (AnimeItemDisplayContext) GetValue(DisplayContextProperty); }
            set { SetValue(DisplayContextProperty, value); }
        }

        public AnimeCompactItem()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (DataContext == null)
                return;
            ViewModel.AnimeItemDisplayContext = DisplayContext;
            Bindings.Update();
        }

        private AnimeItemViewModel ViewModel => DataContext as AnimeItemViewModel;

        private void SubmitWatchedEps(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ViewModel.ChangeWatchedEps();
                e.Handled = true;
            }
        }
    }
}