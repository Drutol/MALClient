using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using MALClient.Pages.Main;
using MALClient.Shared.Managers;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Forums
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ForumNewTopicPage : Page
    {
        private readonly ForumNewTopicViewModel ViewModel = ViewModelLocator.ForumsNewTopic;

        public ForumNewTopicPage()
        {
            this.InitializeComponent();
            ViewModel.UpdatePreview += ViewModelOnUpdatePreview;
            Loaded += OnLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.Init(e.Parameter as ForumsNewTopicNavigationArgs);
            base.OnNavigatedTo(e);
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            UpdateSizing();
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            UpdateSizing();
        }

        private void UpdateSizing()
        {
            if (ActualWidth > 900)
            {
                InputSection.SetValue(Grid.RowSpanProperty,2);
                InputSection.SetValue(Grid.ColumnSpanProperty,1);
                PreviewSection.SetValue(Grid.RowSpanProperty,2);
                PreviewSection.SetValue(Grid.ColumnSpanProperty,1);
                PreviewSection.SetValue(Grid.ColumnProperty,1);
                PreviewSection.SetValue(Grid.RowProperty,0);
                InputScroll.IsVerticalRailEnabled = true;
                GlobalScroll.IsVerticalRailEnabled = false;
                PreviewHeader.Visibility = Visibility.Collapsed;
            }
            else
            {
                InputSection.SetValue(Grid.RowSpanProperty, 1);
                InputSection.SetValue(Grid.ColumnSpanProperty, 2);
                PreviewSection.SetValue(Grid.RowSpanProperty, 1);
                PreviewSection.SetValue(Grid.ColumnSpanProperty,2);
                PreviewSection.SetValue(Grid.ColumnProperty, 0);
                PreviewSection.SetValue(Grid.RowProperty, 1);
                InputScroll.IsVerticalRailEnabled = false;
                GlobalScroll.IsVerticalRailEnabled = true;
                PreviewHeader.Visibility = Visibility.Visible;
            }
        }

        private void ViewModelOnUpdatePreview(object sender, string s)
        {
            PreviewWebView.NavigateToString(CssManager.WrapWithCss(s));
        }
    }
}
