using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.ViewModels
{
    public class MainViewModel : IMainViewModel
    {
        public void Navigate(PageIndex page, object args = null)
        {
            
        }

        public string CurrentStatus { get; set; }
        public AnimeListPageNavigationArgs GetCurrentListOrderParams()
        {
            return new AnimeListPageNavigationArgs(0,AnimeListWorkModes.Anime);
        }

        public void PopulateSearchFilters(HashSet<string> filters)
        {
            
        }

        public void OnSearchInputSubmit()
        {
            
        }

        public event OffContentPaneStateChanged OffContentPaneStateChanged;
        public ICommand HideOffContentCommand { get; }
        public string CurrentOffStatus { get; set; }
        public bool NavigateOffBackButtonVisibility { get; set; }
        public bool NavigateMainBackButtonVisibility { get; set; }
        public string CurrentSearchQuery { get; set; }
        public List<string> SearchHints { get; set; }
        public bool ScrollToTopButtonVisibility { get; set; }
        public string CurrentStatusSub { get; set; }
        public bool IsCurrentStatusSelectable { get; set; }
        public PageIndex? CurrentOffPage { get; set; }
        public bool OffContentVisibility { get; set; }
        public event SearchQuerySubmitted OnSearchQuerySubmitted;
        public event SearchDelayedQuerySubmitted OnSearchDelayedQuerySubmitted;
    }
}