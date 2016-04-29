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
using MALClient.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UserControls
{
    public sealed partial class GridItemFlyout : MenuFlyoutPresenter
    {
        public GridItemFlyout()
        {
            this.InitializeComponent();
        }

        public void ShowAt(FrameworkElement target)
        {
            Flyout.ShowAt(target);
        }

        private void FlyoutButtonPressed(object sender, RoutedEventArgs e)
        {
            Flyout.Hide();
        }
    }
}
