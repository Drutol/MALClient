using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MALClient.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Items
{
    public sealed partial class AnimeCompactItem : UserControl
    {
        public AnimeCompactItem(AnimeItemViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        public AnimeCompactItem()
        {
            InitializeComponent();
        }

        public AnimeItemViewModel ViewModel => DataContext as AnimeItemViewModel;

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