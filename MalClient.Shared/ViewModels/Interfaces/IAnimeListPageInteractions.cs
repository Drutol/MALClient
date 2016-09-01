using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace MalClient.Shared.ViewModels
{
    public interface IAnimeListViewInteractions
    {
        Flyout FlyoutFilters { get; }
        MenuFlyout FlyoutSorting { get; }
        ScrollViewer IndefiniteScrollViewer { set; }
        Flyout FlyoutViews { get; }
        Task<ScrollViewer> GetIndefiniteScrollViewer();
        void FlyoutSeasonSelectionHide();
    }
}
