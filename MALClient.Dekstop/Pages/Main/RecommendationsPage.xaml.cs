using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MALClient.Shared.Items;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using Microsoft.Toolkit.Uwp.UI;
using WinRTXamlToolkit.Controls.Extensions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Main
{
    public class RecommendationPageNavigationArgs
    {
        public int Index;
    }

    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecommendationsPage : Page
    {
        public RecommendationsPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            ViewModelLocator.Recommendations.PropertyChanged += RecommendationsOnPropertyChanged;
        }

        private void RecommendationsOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(ViewModelLocator.Recommendations.CurrentWorkMode))
            {
                InnerPivot.SelectedIndex = (int) ViewModelLocator.Recommendations.CurrentWorkMode;
                if (ViewModelLocator.Recommendations.CurrentWorkMode == RecommendationsPageWorkMode.PersonalizedAnime ||
                    ViewModelLocator.Recommendations.CurrentWorkMode == RecommendationsPageWorkMode.PersonalizedManga)
                {
                    MenuButton.HorizontalAlignment = HorizontalAlignment.Stretch;
                    MenuButton.MaxWidth = 4000;
                }
                else
                {
                    MenuButton.HorizontalAlignment = HorizontalAlignment.Left;
                    MenuButton.MaxWidth = 40;
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            VisualStateGroup.CurrentStateChanged += VisualStateGroupOnCurrentStateChanged;

            if (ActualWidth >= 1120)
            {
                VisualStateManager.GoToState(this, "Wide", true);
                UpdatePivotHeaders(new Thickness(0, 0, 0, 0));
            }
            else
            {
                VisualStateManager.GoToState(this, "Narrow", true);
                UpdatePivotHeaders(new Thickness(40, 0, 0, 0));
            }
        }

        private void VisualStateGroupOnCurrentStateChanged(object sender,
            VisualStateChangedEventArgs visualStateChangedEventArgs)
        {
            UpdatePivotHeaders(visualStateChangedEventArgs.NewState.Name == Narrow.Name
                ? new Thickness(40, 0, 0, 0)
                : new Thickness(0, 0, 0, 0));
        }

        private void UpdatePivotHeaders(Thickness thickness)
        {
            var control = InnerPivotManga.FindDescendantByName("HeaderClipper");
            var control1 = InnerPivotAnime.FindDescendantByName("HeaderClipper");


            if (control1 != null)
                control1.Margin = thickness;
            if (control != null)
                control.Margin = thickness;
        }

        private void Pivot_OnPivotItemLoading(Pivot sender, PivotItemEventArgs args)
        {
            ((args.Item.Content as RecommendationsViewModel.XPivotItem).Content as RecommendationItemViewModel).PopulateData();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModelLocator.NavMgr.ResetMainBackNav();
            if (e.Parameter is RecommendationPageNavigationArgs)
                (DataContext as RecommendationsViewModel).PivotItemIndex =
                    (e.Parameter as RecommendationPageNavigationArgs).Index;
            ViewModelLocator.Recommendations.PopulateData();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataContext = null;
        }

        private void InnerPivot_OnLoaded(object sender, RoutedEventArgs e)
        {
            var sv = InnerPivot.GetFirstDescendantOfType<ItemsPresenter>();
            sv.RenderTransform = null;
        }

        private void InnerPivot_OnPivotItemLoading(Pivot sender, PivotItemEventArgs args)
        {
            ViewModelLocator.Recommendations.CurrentWorkMode = (RecommendationsPageWorkMode)args.Item.Tag;
        }

        private void ItemsGrid_OnItemClick(object sender, ItemClickEventArgs e)
        {
            (e.ClickedItem as AnimeItemViewModel).NavigateDetails(PageIndex.PageRecomendations,null);
        }
    }
}