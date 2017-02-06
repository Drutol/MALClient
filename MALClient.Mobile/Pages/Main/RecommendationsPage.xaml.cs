using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.UWP.Pages.Main
{


    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RecommendationsPage : Page
    {
        public RecommendationsPage()
        {
            InitializeComponent();
            ViewModelLocator.Recommendations.PropertyChanged += RecommendationsOnPropertyChanged;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            switch (ViewModelLocator.Recommendations.CurrentWorkMode)
            {
                case RecommendationsPageWorkMode.Anime:
                case RecommendationsPageWorkMode.Manga:
                    InnerPivot.Visibility = Visibility.Visible;
                    ItemsGrid.Visibility = Visibility.Collapsed;
                    break;
                case RecommendationsPageWorkMode.PersonalizedAnime:
                case RecommendationsPageWorkMode.PersonalizedManga:
                    InnerPivot.Visibility = Visibility.Collapsed;
                    ItemsGrid.Visibility = Visibility.Visible;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RecommendationsOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if(propertyChangedEventArgs.PropertyName == nameof(ViewModelLocator.Recommendations.CurrentWorkMode))
            {
                switch (ViewModelLocator.Recommendations.CurrentWorkMode)
                {
                    case RecommendationsPageWorkMode.Anime:
                    case RecommendationsPageWorkMode.Manga:
                        InnerPivot.Visibility = Visibility.Visible;
                        ItemsGrid.Visibility = Visibility.Collapsed;
                        InnerPivot.ItemsSource = ViewModelLocator.Recommendations.CurrentWorkMode ==
                                                 RecommendationsPageWorkMode.Anime
                            ? ViewModelLocator.Recommendations.RecommendationAnimeItems
                            : ViewModelLocator.Recommendations.RecommendationMangaItems;
                        break;
                    case RecommendationsPageWorkMode.PersonalizedAnime:
                    case RecommendationsPageWorkMode.PersonalizedManga:
                        InnerPivot.Visibility = Visibility.Collapsed;
                        ItemsGrid.Visibility = Visibility.Visible;
                        ItemsGrid.ItemsSource = ViewModelLocator.Recommendations.CurrentWorkMode ==
                                                 RecommendationsPageWorkMode.PersonalizedAnime
                            ? ViewModelLocator.Recommendations.PersonalizedAnimeItems
                            : ViewModelLocator.Recommendations.PersonalizedMangaItems;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (propertyChangedEventArgs.PropertyName == nameof(ViewModelLocator.Recommendations.PersonalizedAnimeItems))
            {
                ItemsGrid.ItemsSource = ViewModelLocator.Recommendations.PersonalizedAnimeItems;
            }
            else if (propertyChangedEventArgs.PropertyName == nameof(ViewModelLocator.Recommendations.PersonalizedMangaItems))
            {
                ItemsGrid.ItemsSource = ViewModelLocator.Recommendations.PersonalizedMangaItems;
            }
            else if (propertyChangedEventArgs.PropertyName == nameof(ViewModelLocator.Recommendations.RecommendationAnimeItems))
            {
                InnerPivot.ItemsSource = ViewModelLocator.Recommendations.RecommendationAnimeItems;
            }
            else if (propertyChangedEventArgs.PropertyName == nameof(ViewModelLocator.Recommendations.RecommendationMangaItems))
            {
                InnerPivot.ItemsSource = ViewModelLocator.Recommendations.RecommendationMangaItems;
            }
        }

        private void Pivot_OnPivotItemLoading(Pivot sender, PivotItemEventArgs args)
        {
            ((args.Item.Content as RecommendationsViewModel.XPivotItem).Content as RecommendationItemViewModel).PopulateData();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is RecommendationPageNavigationArgs)
                (DataContext as RecommendationsViewModel).PivotItemIndex =
                    (e.Parameter as RecommendationPageNavigationArgs).Index;
            ViewModelLocator.NavMgr.DeregisterBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
            ViewModelLocator.Recommendations.PopulateData();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataContext = null;
            ViewModelLocator.NavMgr.DeregisterBackNav();
        }

        private void ItemsGrid_OnItemClick(object sender, ItemClickEventArgs e)
        {
            (e.ClickedItem as AnimeItemViewModel).NavigateDetails(PageIndex.PageRecomendations, null);
        }
    }
}