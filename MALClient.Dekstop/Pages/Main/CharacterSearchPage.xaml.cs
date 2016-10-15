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
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CharacterSearchPage : Page
    {
        private bool _initialized;

        public CharacterSearchPage()
        {
            this.InitializeComponent();
            Loaded+= OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            UpperNavBarPivot.SelectedIndex = 2;
            _initialized = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModelLocator.NavMgr.ResetMainBackNav();
            ViewModelLocator.CharacterSearch.Init(e.Parameter as SearchPageNavArgs);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModelLocator.CharacterSearch.OnNavigatedFrom();
            base.OnNavigatingFrom(e);
        }

        private void CharacterItemOnClick(object sender, ItemClickEventArgs e)
        {
            ViewModelLocator.CharacterSearch.NavigateCharacterDetailsCommand.Execute(e.ClickedItem);
        }

        private void UpperNavBarPivotOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_initialized)
                return;

            if (UpperNavBarPivot.SelectedIndex == 0)
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageSearch, new SearchPageNavigationArgs());
            else if (UpperNavBarPivot.SelectedIndex == 1)
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageMangaSearch, new SearchPageNavigationArgs { Anime = false });

            _initialized = false;
            UpperNavBarPivot.SelectedIndex = 2;
            _initialized = true;

        }
    }
}
