using System;
using System.Collections.Generic;
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
using MalClient.Shared.NavArgs;
using MalClient.Shared.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Off
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CharacterDetailsPage : Page
    {
        private CharacterDetailsNavigationArgs _lastArgs;

        public CharacterDetailsPage()
        {
            this.InitializeComponent();
            Loaded += (sender, args) => ViewModelLocator.CharacterDetails.Init(_lastArgs);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _lastArgs = e.Parameter as CharacterDetailsNavigationArgs;
            base.OnNavigatedTo(e);
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModelLocator.CharacterDetails.NavigateAnimeDetailsCommand.Execute(e.ClickedItem);
        }

        private void ListViewBase_OnItemClickManga(object sender, ItemClickEventArgs e)
        {
            ViewModelLocator.CharacterDetails.NavigateMangaDetailsCommand.Execute(e.ClickedItem);
        }
    }
}
