using System;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using MALClient.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    public sealed partial class AnimeDetailsPage : Page, IDetailsViewInteraction
    {
        public AnimeDetailsPage()
        {
            InitializeComponent();
            ViewModel.View = this;
            var disp = DisplayInformation.GetForCurrentView();
            ProcessOrientation(disp.CurrentOrientation);
            disp.OrientationChanged += OnOrientationChanged;           
        }

        /// <summary>
        /// Very very dirty hack , don't look at this...
        /// But I want to have landscape orientation and I don't know enough XAML magic to do this there.
        /// </summary>
        /// <param name="orientation"></param>
        private void ProcessOrientation(DisplayOrientations orientation)
        {
            if (ScrollingContainer.Content == null && (orientation == DisplayOrientations.Landscape ||
                orientation == DisplayOrientations.LandscapeFlipped))
            {
                var grid = RootGrid.Children[1];
                RootGrid.Children.RemoveAt(1);
                ScrollingContainer.Content = grid;

            }
            else if(ScrollingContainer.Content != null)
            {
                var grid = ScrollingContainer.Content;
                ScrollingContainer.Content = null;
                RootGrid.Children.Insert(1, grid as FrameworkElement);
            }
        }

        private void OnOrientationChanged(DisplayInformation sender, object args)
        {
            ProcessOrientation(sender.CurrentOrientation);
        }

        private AnimeDetailsPageViewModel ViewModel => DataContext as AnimeDetailsPageViewModel;

        public Flyout GetWatchedEpsFlyout()
        {
            return WatchedEpsFlyout;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var param = e.Parameter as AnimeDetailsPageNavigationArgs;
            if (param == null)
                throw new Exception("No paramaters for this page");
            if (!param.AnimeMode)
                MainPivot.Items.RemoveAt(1);
            ViewModel.Init(param);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataContext = null;
            base.OnNavigatedFrom(e);
            NavMgr.DeregisterBackNav();
            DisplayInformation.GetForCurrentView().OrientationChanged -= OnOrientationChanged;
        }

        private void SubmitWatchedEps(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ViewModel.ChangeWatchedEps();
                e.Handled = true;
            }
        }

        private void Pivot_OnPivotItemLoading(Pivot sender, PivotItemEventArgs args)
        {
            if (!ViewModel._initialized)
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

        private void Image_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.ImageOverlayVisibility = Visibility.Visible;
        }

        private void ScrollViewer_OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            (sender as ScrollViewer).ZoomToFactor(Settings.SelectedApiType == ApiType.Mal ? 1 : 0.5f);
        }

        private void ScrollViewerAlternate_OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            (sender as ScrollViewer).ZoomToFactor((float).5);
        }

        private void AlternateImage_OnImageOpened(object sender, RoutedEventArgs e)
        {
            AlternateImgScrollViewer.ZoomToFactor((float).5);
            CurrentImgDimesnions.Text = $"{ViewModel.HummingbirdImage.PixelWidth}x{ViewModel.HummingbirdImage.PixelHeight}";

        }

        private void MalImage_OnImageOpened(object sender, RoutedEventArgs e)
        {
            StockImgScrollViewer.ZoomToFactor(Settings.SelectedApiType == ApiType.Mal ? 1 : 0.5f);
            CurrentImgDimesnions.Text = $"{ViewModel.DetailImage.PixelWidth}x{ViewModel.DetailImage.PixelHeight}";
        }

        private void ImagePivot_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ((sender as FlipView).SelectedIndex == 0 && ViewModel?.DetailImage != null)
                    CurrentImgDimesnions.Text =
                        $"{ViewModel.DetailImage.PixelWidth}x{ViewModel.DetailImage.PixelHeight}";
                else if (ViewModel?.HummingbirdImage != null)
                    CurrentImgDimesnions.Text =
                        $"{ViewModel.HummingbirdImage.PixelWidth}x{ViewModel.HummingbirdImage.PixelHeight}";
            }
            catch (Exception)
            {
                //voodoo
            }

        }

        private void StartDate_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if(ViewModel.StartDateValid)
                ResetStartDateFlyout.ShowAt(sender as FrameworkElement);
        }

        private void EndDate_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (ViewModel.EndDateValid)
                ResetEndDateFlyout.ShowAt(sender as FrameworkElement);
        }
    }
}