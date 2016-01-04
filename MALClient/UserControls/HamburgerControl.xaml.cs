using System;
using System.Linq;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UserControls
{
    public sealed partial class HamburgerControl : UserControl
    {
        public HamburgerControl()
        {
            InitializeComponent();
            TxtList.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
        }

        internal void PaneOpened()
        {
            Border1.SetValue(Grid.ColumnSpanProperty, 2);
            //Border2.SetValue(Grid.ColumnSpanProperty, 2);
            //Border3.SetValue(Grid.ColumnSpanProperty, 2);
            Border4.SetValue(Grid.ColumnSpanProperty, 2);
        }

        internal void PaneClosed()
        {
            Border1.SetValue(Grid.ColumnSpanProperty, 1);
            //Border2.SetValue(Grid.ColumnSpanProperty, 1);
            //Border3.SetValue(Grid.ColumnSpanProperty, 1);
            Border4.SetValue(Grid.ColumnSpanProperty, 1);
        }

        //private void BtnSettings_Click(object sender, RoutedEventArgs e)
        //{
        //    GetMainPageInstance().NavigateSettings();
        //    ResetActiveButton();
        //    txtSettings.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
        //}

        private void BtnList_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().NavigateList();
            ResetActiveButton();
            TxtList.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
        }

        //private void BtnHistory_Click(object sender, RoutedEventArgs e)
        //{
        //    GetMainPageInstance().NavigateHistory();
        //    ResetActiveButton();
        //    txtHistory.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
        //}

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().NavigateLogin();
            ResetActiveButton();
            TxtLogin.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
        }

        private void ResetActiveButton()
        {
            //txtSettings.Foreground = new SolidColorBrush(Colors.Black);
            //txtHistory.Foreground = new SolidColorBrush(Colors.Black);
            TxtList.Foreground = new SolidColorBrush(Colors.Black);
            TxtLogin.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Button btn = sender as Button;
            Grid grid = btn.Content as Grid;
            foreach (Border item in grid.Children.OfType<Border>())
            {
                item.Visibility = Visibility.Visible;
                break;
            }
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Button btn = sender as Button;
            Grid grid = btn.Content as Grid;
            foreach (Border item in grid.Children.OfType<Border>())
            {
                item.Visibility = Visibility.Collapsed;
                break;
            }
        }

        private MainPage GetMainPageInstance()
        {
            return MALClient.Utils.GetMainPageInstance();
        }


    }
}
