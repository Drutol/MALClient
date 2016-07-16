using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MalClient.Shared.Models.MalSpecific;
using MalClient.Shared.ViewModels.Main;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Messages
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MalMessagingPage : Page
    {
        public MalMessagingPage()
        {
            InitializeComponent();
            Loaded += (sender, args) => ViewModel.Init();
        }

        private MalMessagingViewModel ViewModel => DataContext as MalMessagingViewModel;

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var msg = e.AddedItems.First() as MalMessageModel;
                msg.IsRead = true;
            }
        }

        private async void ButtonDisclaimer_OnClick(object sender, RoutedEventArgs e)
        {
            var msg = new MessageDialog("These messages are obatained directly from html, I cannot build sophisticated system on top of this foundation... that's why it's that simplistic.","Disclaimer");
            await msg.ShowAsync();
        }
    }
}