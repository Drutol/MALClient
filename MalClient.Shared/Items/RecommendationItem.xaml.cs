using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UWP.Shared.Items
{
    public sealed partial class RecommendationItem : UserControl
    {
        private RecommendationData _data;

        RecommendationItemViewModel ViewModel => DataContext as RecommendationItemViewModel;

        private bool _dataLoaded;
        private bool _wide;

        public RecommendationItem()
        {
            InitializeComponent();
            SizeChanged += OnSizeChanged;
            Loaded += OnLoaded;
        }


        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (_wide && sizeChangedEventArgs.NewSize.Width < 1000)
            {
                _wide = false;
                VisualStateManager.GoToState(this, "Narrow",false);
            }
            else if(!_wide && sizeChangedEventArgs.NewSize.Width > 1000)
            {
                _wide = true;
                VisualStateManager.GoToState(this, "Wide", false);
            }
        }

        public int Index => ViewModel.Index;

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                _wide = ActualWidth > 1000;
                VisualStateManager.GoToState(this, _wide ? "Wide" : "Narrow", false);
            }
            catch (Exception)
            {
                //
            }
        }
     
        private void ButtonRecomDetails_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateRecDetails.Execute(null);
        }

        private void ButtonDependentDetails_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateDepDetails.Execute(null);
        }
    }
}