using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using MALClient.Shared.Managers;
using MALClient.Models.Enums;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums;
using WinRTXamlToolkit.Controls.Extensions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages.Forums
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ForumTopicPage : Page , ForumTopicViewModel.IScrollInfoProvider
    {
        public ForumTopicViewModel ViewModel => ViewModelLocator.ForumsTopic;
        private int _webViewsToGo;
        private int _requestedIndex;
        private bool _addedCopyHandler;

        public ForumTopicPage()
        {
            this.InitializeComponent();
            ViewModel.ScrollInfoProvider = this;
            ViewModel.RequestScroll += ViewModelOnRequestScroll;
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;

        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.Messages))
            {
                _webViewsToGo = ViewModel.Messages.Count;
            }
        }

        private void ViewModelOnRequestScroll(object sender, int i)
        {
            if (_webViewsToGo > 0)
            {
                _requestedIndex = i;
                return;
            }
            ListView.ScrollIntoView(ViewModel.Messages[i], ScrollIntoViewAlignment.Leading);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (Settings.ForumsSearchOnCopy)
            {
                _addedCopyHandler = true;
                Clipboard.ContentChanged += ClipboardOnContentChanged;
            }
            ViewModel.Init(e.Parameter as ForumsTopicNavigationArgs);
            base.OnNavigatedTo(e);
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (_addedCopyHandler)
            {
                Clipboard.ContentChanged -= ClipboardOnContentChanged;
            }
            base.OnNavigatedFrom(e);
        }

        private async void ClipboardOnContentChanged(object sender, object o)
        {
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                string text = await dataPackageView.GetTextAsync();
                ViewModelLocator.GeneralMain.Navigate(PageIndex.PageSearch, new SearchPageNavigationArgs
                {
                    Anime = !ViewModel.IsMangaBoard,
                    Query = text.Trim(),
                    ForceQuery = true,
                });
            }
        }       

        

       

        private void TopicWebView_OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            try
            {
                if (args.Uri != null)
                {
                    var uri = args.Uri.AbsoluteUri;
                    args.Cancel = true;
                    var navArgs = MalLinkParser.GetNavigationParametersForUrl(uri);
                    if (navArgs != null)
                    {
                        if(!navArgs.Item1.GetAttribute<EnumUtilities.PageIndexEnumMember>().OffPage)
                            ViewModel.RegisterSelfBackNav();

                        ViewModelLocator.GeneralMain.Navigate(navArgs.Item1, navArgs.Item2);
                    }
                    else if (Settings.ArticlesLaunchExternalLinks)
                    {
                        ResourceLocator.SystemControlsLauncherService.LaunchUri(args.Uri);
                    }
                }
            }
            catch (Exception)
            {
                args.Cancel = true;
            }
        }

        private void GotoInputOnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                if (string.IsNullOrEmpty(GotoPageTextBox.Text))
                    return;
                int val;
                if (!int.TryParse(GotoPageTextBox.Text, out val))
                {
                    GotoPageFlyout.Hide();
                    return;
                }

                GotoPageFlyout.Hide();
                ViewModel.LoadPageCommand.Execute(val);
                e.Handled = true;
            }
        }

        private void GotoPageTextBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ViewModel.LoadGotoPageCommand.Execute(null);
        }

        private void WebView_OnScriptNotify(object sender, NotifyEventArgs e)
        {
            var val = int.Parse(e.Value);
            var view = sender as WebView;
            view.ScriptNotify -= WebView_OnScriptNotify;
            if (val > view.ActualHeight)
                view.Height = val + 60;
            _webViewsToGo--;

            if (_webViewsToGo == 0)
            {
                ScrollToIndex();
            }
        }

        public int GetFirstVisibleItemIndex()
        {
            return ListView.GetFirstVisibleIndex();
        }

        private void NewReplyButtonOnClick(object sender, RoutedEventArgs e)
        {
            ListView.ScrollToBottom();
        }

        private void ScrollToIndex()
        {
            ListView.ScrollIntoView(ViewModel.Messages[_requestedIndex], ScrollIntoViewAlignment.Leading);
            _requestedIndex = 0;
        }
    }
}
