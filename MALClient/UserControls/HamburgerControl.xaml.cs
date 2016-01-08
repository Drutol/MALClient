using System;
using System.Linq;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using MALClient.Pages;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UserControls
{

    public enum HamburgerButtons
    {
        AnimeList,
        AnimeSearch,
        LogIn,
        Settings
    }

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
            Border2.SetValue(Grid.ColumnSpanProperty, 2);
            Border3.SetValue(Grid.ColumnSpanProperty, 2);
            Border4.SetValue(Grid.ColumnSpanProperty, 2);
        }

        internal void PaneClosed()
        {
            Border1.SetValue(Grid.ColumnSpanProperty, 1);
            Border2.SetValue(Grid.ColumnSpanProperty, 1);
            Border3.SetValue(Grid.ColumnSpanProperty, 1);
            Border4.SetValue(Grid.ColumnSpanProperty, 1);
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageSettings);
            SetActiveButton(HamburgerButtons.AnimeList);
        }

        private void BtnList_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageAnimeList);
            SetActiveButton(HamburgerButtons.AnimeList);
        }

        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageSearch);
            SetActiveButton(HamburgerButtons.AnimeSearch);
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            GetMainPageInstance().Navigate(PageIndex.PageLogIn);
            SetActiveButton(HamburgerButtons.LogIn);
        }

        public void SetActiveButton(HamburgerButtons val)
        {
            ResetActiveButton();
            switch (val)
            {
                case HamburgerButtons.AnimeList:
                    TxtList.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
                    break;
                case HamburgerButtons.AnimeSearch:
                    TxtSearch.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
                    break;
                case HamburgerButtons.LogIn:
                    TxtLogin.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
                    break;
                case HamburgerButtons.Settings:
                    TxtSettings.Foreground = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(val), val, null);
            }
        }

        private void ResetActiveButton()
        {
            //txtSettings.Foreground = new SolidColorBrush(Colors.Black);
            TxtSearch.Foreground = new SolidColorBrush(Colors.Black);
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
            return Utils.GetMainPageInstance();
        }
    }
}
