using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels.Details;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Off
{
    public sealed partial class AnimeDetailsPage : Page
    {
        public AnimeDetailsPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            UpdatePivotHeaderSizes();
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            UpdatePivotHeaderSizes();
        }

        private void UpdatePivotHeaderSizes()
        {
            if (ActualWidth >= 535)
            {
                PivotHeaderGeneral.FontSize =
                    PivotHeaderDetails.FontSize =
                        PivotHeaderRecomm.FontSize = 
                            PivotHeaderReviews.FontSize = 
                                PivotHeaderRelated.FontSize = 24;
            }
            else
            {
                PivotHeaderGeneral.FontSize =
                    PivotHeaderDetails.FontSize =
                        PivotHeaderRecomm.FontSize =
                            PivotHeaderReviews.FontSize =
                                PivotHeaderRelated.FontSize = 20;
            }
        }

        private AnimeDetailsPageViewModel ViewModel => DataContext as AnimeDetailsPageViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var param = e.Parameter as AnimeDetailsPageNavigationArgs;
            if (param == null)
                throw new Exception("No paramaters for this page");
            ViewModel.Init(param);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataContext = null;
            base.OnNavigatedFrom(e);
        }

        private void SubmitWatchedEps(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ViewModel.ChangeWatchedCommand.Execute(null);
                WatchedEpsFlyout.Hide();
                e.Handled = true;
            }
        }

        private void SubmitReadVolumes(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ViewModel.ChangeVolumesCommand.Execute(null);
                ReadVolumesFlyout.Hide();
                e.Handled = true;
            }
        }

        private void Pivot_OnPivotItemLoading(Pivot sender, PivotItemEventArgs args)
        {
            if (!ViewModel.Initialized)
                return;
            switch (args.Item.Tag as string)
            {
                case "Details":
                    ViewModel.LoadDetails();
                    break;
                case "Reviews":
                    ViewModel.LoadReviews();
                    break;
                case "Recomm":
                    ViewModel.LoadRecommendations();
                    break;
                case "Related":
                    ViewModel.LoadRelatedAnime();
                    break;
            }
        }


        //if there was no date different than today's chosen by the user , we have to manually trigger the setter as binding won't do it
        private void StartDatePickerFlyout_OnDatePicked(DatePickerFlyout sender, DatePickedEventArgs args)
        {
            if (!ViewModel.StartDateValid)
                ViewModel.StartDateTimeOffset = ViewModel.StartDateTimeOffset;
        }

        private void EndDatePickerFlyout_OnDatePicked(DatePickerFlyout sender, DatePickedEventArgs args)
        {
            if (!ViewModel.EndDateValid)
                ViewModel.EndDateTimeOffset = ViewModel.EndDateTimeOffset;
        }

        private void StartDate_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (ViewModel.StartDateValid)
                ResetStartDateFlyout.ShowAt(sender as FrameworkElement);
        }

        private void EndDate_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (ViewModel.EndDateValid)
                ResetEndDateFlyout.ShowAt(sender as FrameworkElement);
        }

        private void UIElement_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ImageSaveFlyout.ShowAt(sender as FrameworkElement);
        }

        private void ReadVolumesButton_OnClick(object sender, RoutedEventArgs e)
        {
            ReadVolumesFlyout.Hide();
        }

        private void WatchedEpsButton_OnClick(object sender, RoutedEventArgs e)
        {
            WatchedEpsFlyout.Hide();
        }

        private TypeInfo _typeInfo;
        //why? beacuse MSFT Bugged this after anniversary update
        private void BuggedFlyoutContentAfterAnniversaryUpdateOnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var typeInfo = _typeInfo ?? (_typeInfo = typeof(FrameworkElement).GetTypeInfo());
                var prop = typeInfo.GetDeclaredProperty("AllowFocusOnInteraction");
                prop?.SetValue(sender, true);
            }
            catch (Exception)
            {
                //not AU
            }
        }

        private void RewatchedButtonOnRightClick(object sender, RightTappedRoutedEventArgs e)
        {
            var btn = sender as FrameworkElement;
            var flyout = FlyoutBase.GetAttachedFlyout(btn);
            flyout.ShowAt(btn);
        }

        private void RewatchedFlyoutOnItemSelected(object sender, ItemClickEventArgs e)
        {
            ViewModel.SetRewatchingCountCommand.Execute(e.ClickedItem);
            RewatchingFlyout.Hide();
        }

        private void VideosListViewOnItemCLick(object sender, ItemClickEventArgs e)
        {
            ViewModel.OpenVideoCommand.Execute(e.ClickedItem);
            PromotionalVideosFlyout.Hide();
        }

        private void WatchedButtonOnClick(object sender, RoutedEventArgs e)
        {
            var numbers = new List<int>();
            int i = ViewModel.MyEpisodes, j = ViewModel.MyEpisodes - 1, k = 0;
            for (; k < 10; i++, j--, k++)
            {
                if (ViewModel.AllEpisodes == 0 || i <= ViewModel.AllEpisodes)
                    numbers.Add(i);
                if (j >= 0)
                    numbers.Add(j);
            }
            QuickSelectionGrid.ItemsSource = numbers.OrderBy(i1 => i1).Select(i1 => i1.ToString());
            QuickSelectionGrid.SelectedItem = ViewModel.MyEpisodes.ToString();
            QuickSelectionGrid.ScrollIntoView(QuickSelectionGrid.SelectedItem);
        }

        private void QuickSelectionGrid_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.WatchedEpsInput = e.ClickedItem as string;
            ViewModel.ChangeWatchedCommand.Execute(null);
            WatchedEpsFlyout.Hide();
        }

        private static readonly Brush _b2 =
    new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Dark
        ? Color.FromArgb(200, 50, 50, 50)
        : Color.FromArgb(200, 190, 190, 190));

        private void ReviewItemOnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            (sender as Grid).Background = _b2;
        }

        private void ReviewItemOnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            (sender as Grid).Background = new SolidColorBrush(Colors.Transparent);
        }
    }
}