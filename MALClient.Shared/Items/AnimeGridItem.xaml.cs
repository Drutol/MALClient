using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Models.Enums;
using MALClient.UWP.Shared.Managers;
using MALClient.XShared.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UWP.Shared.Items
{
    public sealed partial class AnimeGridItem : UserControl
    {
        private bool _handlerAdded;
        private Point _initialPoint;
        private static readonly TimeZoneInfo _jstTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
		private int _oldID;

        public AnimeGridItem()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        public static readonly DependencyProperty DisplayContextProperty =
            DependencyProperty.Register("DisplayContext", typeof(AnimeItemDisplayContext), typeof(AnimeGridItem),
                new PropertyMetadata(AnimeItemDisplayContext.AirDay,DisplayContextPropertyChangedCallback));

        private static void DisplayContextPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var item = dependencyObject as AnimeGridItem;
            if(item.ViewModel != null)
                item.ViewModel.AnimeItemDisplayContext = (AnimeItemDisplayContext)e.NewValue;
        }

        public AnimeItemDisplayContext DisplayContext
        {
            get { return (AnimeItemDisplayContext)GetValue(DisplayContextProperty); }
            set { SetValue(DisplayContextProperty, value); }
        }


        public static readonly DependencyProperty DisplayAirTillTimeProperty = DependencyProperty.Register(
            "DisplayAirTillTime", typeof(bool), typeof(AnimeGridItem), new PropertyMetadata(default(bool)));

        public bool DisplayAirTillTime
        {
            get { return (bool) GetValue(DisplayAirTillTimeProperty); }
            set { SetValue(DisplayAirTillTimeProperty, value); }
        }

        public bool AllowSwipeInGivenContext
        {
            set
            {
                if(!value)
                    ManipulationMode = ManipulationModes.System;
            }
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
			if (DataContext == null)
				return;
			if(_oldID != ViewModel.Id)
			{
				ImageLoading.Visibility = Visibility.Visible;
				Image.Source = null;
				_oldID = ViewModel.Id;
			}

            ViewModel.AnimeItemDisplayContext = DisplayContext;
            if (!_handlerAdded)
            {
                ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
                _handlerAdded = true;
            }
            if (DisplayAirTillTime)
            {
                var time = ViewModel.GetTimeTillNextAir(_jstTimeZone);
                if (!string.IsNullOrEmpty(time))
                {
                    TimeTillNextAirGrid.Visibility = Visibility.Visible;
                    TimeTillNextAir.Text = time;
                    TypeTextBlock.Text = ViewModel.PureType;
                }
            }
            //Bindings.Update();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.MyStatus))
            {
                var targetStatus = ViewModelLocator.AnimeList.GetDesiredStatus();
                if (targetStatus == AnimeStatus.AllOrAiring || ViewModel.MyStatus != targetStatus)
                    Opacity = .6;
                else
                    Opacity = 1;
            }
        }

        public AnimeItemViewModel ViewModel => DataContext as AnimeItemViewModel;

        private void BtnMoreClick(object sender, RoutedEventArgs e)
        {
            ItemFlyoutService.ShowAnimeGridItemFlyout(sender as FrameworkElement);
        }

        private void WatchedFlyoutButton_OnClick(object sender, RoutedEventArgs e)
        {
            ItemFlyoutService.ShowWatchedEpisodesFlyout(sender as FrameworkElement);
        }

        private static AnimeGridItem _manip; //currently manipulated item

        private void AnimeGridItem_OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (_manip != null)
                return;
            _initialPoint = e.Position;
            _manip = this;
			ViewModel.AllowDetailsNavigation = false;
			DecrementField.Visibility = IncrementField.Visibility = Visibility.Visible;
        }

		private bool? _incDecState = null;
		private void AnimeGridItem_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
		{
			if (_manip == this) {
				ViewModel.AllowDetailsNavigation = false;

				TranslateTransformSwipe.X = Math.Clamp(e.Cumulative.Translation.X, -100, 100);

				var delta = TranslateTransformSwipe.X;
				var absDelta = Math.Abs(delta);
				if(absDelta > 35)
				{
					if (delta < 0)
					{
						IncrementField.Background = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
						DecrementField.Background = new SolidColorBrush(Colors.Black);
						_incDecState = true; //inc

					}
					else if (delta > 0)
					{
						IncrementField.Background = new SolidColorBrush(Colors.Black);
						DecrementField.Background = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
						_incDecState = false; //dec
					}
				}
				else
				{
					IncrementField.Background = new SolidColorBrush(Colors.Black);
					DecrementField.Background = new SolidColorBrush(Colors.Black);
					_incDecState = null; //do nothing
				}
			}
		}

        private void AnimeGridItem_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            GoBackStoryboard.Begin();
            if (_incDecState != null)
                if (_incDecState.Value)
                    ViewModel.IncrementWatchedCommand.Execute(null);
                else
                    ViewModel.DecrementWatchedCommand.Execute(null);

            
            _incDecState = null;
            _manip = null;
            ViewModel.AllowDetailsNavigation = true;
        }

        private void Timeline_OnCompleted(object sender, object e)
        {
            DecrementField.Visibility = IncrementField.Visibility = Visibility.Collapsed;
        }

        private void Image_OnImageOpened(object sender, RoutedEventArgs e)
        {
            var img = sender as Image;
            var imgSource = img.Source as BitmapImage;
			if (imgSource == null)
				return;
            if(imgSource.PixelHeight == 0 || img.ActualHeight == 0)
                return;
            if (Math.Abs(ActualHeight / ActualWidth - imgSource.PixelHeight / (double)imgSource.PixelWidth) > .65)
            {
                img.Stretch = Stretch.Uniform;
            }
			ImageLoading.Visibility = Visibility.Collapsed;
        }

	}
}