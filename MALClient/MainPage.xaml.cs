using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Pages;
using MALClient.UserControls;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MALClient
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page , IMainViewInteractions
    {
        #pragma warning disable 4014
        public MainPage()
        {
            InitializeComponent();
            Utils.CheckTiles();
            ViewModelLocator.Main.View = this;          
        }
        #pragma warning restore 4014

        #region Search
        private void SearchInput_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if ((e == null || e.Key == VirtualKey.Enter) && SearchInput.Text.Length >= 2)
            {
                SearchInput.IsEnabled = false; //reset input
                SearchInput.IsEnabled = true;
                ViewModelLocator.Main.OnSearchInputSubmit();
            }
        }
        #endregion

        public void Navigate(Type page, object args = null)
        {
            MainContent.Navigate(page, args);
        }

        public object GetCurrentContent()
        {
            return MainContent.Content;
        }

        public void SearchInputFocus(FocusState state)
        {
            SearchInput.Focus(state);
        }

    }
}