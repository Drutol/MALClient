using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ExpressionBuilder;
using MALClient.UWP.Shared.Managers;
using MALClient.XShared.Comm.MagicalRawQueries.Clubs;
using MALClient.XShared.ViewModels;
using WinRTXamlToolkit.Controls.Extensions;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.UWP.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClubIndexPage : Page
    {
        private BlurHelper _blur;
        private static int lastPivotIndex = 0;

        public ClubIndexPage()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {

            _blur = new BlurHelper(LoadingOverlay, false);
            ClubsPivot.SelectedIndex = lastPivotIndex;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModelLocator.ClubIndex.NavigatedTo();
            base.OnNavigatedTo(e);
        }


        private void ClubClick(object sender, ItemClickEventArgs e)
        {
            ViewModelLocator.ClubIndex.NavigateDetailsCommand.Execute(e.ClickedItem);
        }

        private void ClubsPivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModelLocator.ClubIndex.QueryType = ClubsPivot.SelectedIndex == 0
                ? MalClubQueries.QueryType.My
                : MalClubQueries.QueryType.All;
            lastPivotIndex = ClubsPivot.SelectedIndex;
        }

        private void SearchComboBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            SearchComboBox.ItemsSource = Enum.GetValues(typeof(MalClubQueries.SearchCategory));
            SearchComboBox.SelectedItem = ViewModelLocator.ClubIndex.SearchCategory;
        }

        private void AutoSuggestBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ViewModelLocator.ClubIndex.SearchCommand.Execute(null);
        }
    }


}
