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
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;


namespace MALClient.UWP.Pages.Main
{
    public sealed partial class ListComparisonPage : Page
    {
        private ListComparisonViewModel _viewModel = ViewModelLocator.Comparison;

        public ListComparisonPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _viewModel.NavigatedTo(e.Parameter as ListComparisonPageNavigationArgs);
            base.OnNavigatedTo(e);
        }

        private void OnStatusFilterSelected(object sender, ItemClickEventArgs e)
        {
            _viewModel.StatusFilter = (AnimeStatus) (e.ClickedItem as FrameworkElement).Tag;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.StatusFilterTarget = (ComparisonStatusFilterTarget) FilterStatusComboBox.SelectedIndex;
        }

        private void OnComparisonStatusFilterSelected(object sender, ItemClickEventArgs e)
        {
            _viewModel.ComparisonFilter = (ComparisonFilter)(e.ClickedItem as FrameworkElement).Tag;
        }

        private void OnSortingSelected(object sender, ItemClickEventArgs e)
        {
            _viewModel.ComparisonSorting = (ComparisonSorting)(e.ClickedItem as FrameworkElement).Tag;
        }
    }
}
