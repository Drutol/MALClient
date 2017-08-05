using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Windows.Input;
using Android.Views;
using Android.Widget;
using MALClient.Android.Adapters;
using MALClient.Android.BackgroundTasks;
using MALClient.Android.Fragments.SettingsFragments;
using MALClient.Models.Models.Misc;
using MALClient.XShared.BL;
using MALClient.XShared.ViewModels.Clubs;
using MALClient.XShared.ViewModels.Details;
using MALClient.XShared.ViewModels.Forums;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android
{
    // This class is never actually executed, but when Xamarin linking is enabled it does how to ensure types and properties
    // are preserved in the deployed app
    [global::Android.Runtime.Preserve(AllMembers = true)]
    public class LinkerPleaseInclude
    {
        public void Include(Button button)
        {
            button.Click += (s, e) => button.Text = button.Text + "";
        }

        public void Include(CheckBox checkBox)
        {
            checkBox.CheckedChange += (sender, args) => checkBox.Checked = !checkBox.Checked;
        }

        public void Include(View view)
        {
            view.Click += (s, e) => view.ContentDescription = view.ContentDescription + "";
        }

        public void Include(TextView text)
        {
            text.TextChanged += (sender, args) => text.Text = "" + text.Text;
            text.Hint = "" + text.Hint;
        }

        public void Include(CompoundButton cb)
        {
            cb.CheckedChange += (sender, args) => cb.Checked = !cb.Checked;
        }

        public void Include(SeekBar sb)
        {
            sb.ProgressChanged += (sender, args) => sb.Progress = sb.Progress + 1;
        }

        public void Include(ProgressBar sb)
        {
            var a = sb.Max;
            sb.Max = 10;
        }

        public void Include(Switch sw)
        {
            sw.CheckedChange += (sender, args) =>
            {

            };
        }

        public void Include(EditText et)
        {
            et.Text = "fdsf";
            et.SetText("aaa",TextView.BufferType.Normal);
            et.TextChanged += (sender, args) =>
            {

            };
            et.AfterTextChanged += (sender, args) =>
            {

            };
        }

        public void Include(INotifyCollectionChanged changed)
        {
            changed.CollectionChanged += (s, e) => { var test = string.Format("{0}{1}{2}{3}{4}", e.Action, e.NewItems, e.NewStartingIndex, e.OldItems, e.OldStartingIndex); };
        }

        public void Include(ICommand command)
        {
            command.CanExecuteChanged += (s, e) => { if (command.CanExecute(null)) command.Execute(null); };
        }

        public void Include(AesCryptoServiceProvider a)
        {
            System.Security.Cryptography.AesCryptoServiceProvider b = new System.Security.Cryptography.AesCryptoServiceProvider();
        }

        public void Include(RecommendationsViewModel vm)
        {
            vm.PopulateData();
            var vm1 = new RecommendationsViewModel();
        }

        public void Include(SearchPageViewModel vm) { var vm1 = new SearchPageViewModel(); }
        public void Include(HummingbirdProfilePageViewModel vm) { var vm1 = new HummingbirdProfilePageViewModel(); }
        public void Include(CalendarPageViewModel vm) { var vm1 = new CalendarPageViewModel(null); }
        public void Include(MalArticlesViewModel vm) { var vm1 = new MalArticlesViewModel(); }
        public void Include(MalMessagingViewModel vm) { var vm1 = new MalMessagingViewModel(); }
        public void Include(MalMessageDetailsViewModel vm) { var vm1 = new MalMessageDetailsViewModel(); }
        public void Include(AnimeDetailsPageViewModel vm) { var vm1 = new AnimeDetailsPageViewModel(null,null,null,null); }
        public void Include(AnimeListViewModel vm) { var vm1 = new AnimeListViewModel(null); }
        public void Include(ForumIndexViewModel vm) { var vm1 = new ForumIndexViewModel(); }
        public void Include(ForumsMainViewModel vm) { var vm1 = new ForumsMainViewModel(); }
        public void Include(ForumBoardViewModel vm) { var vm1 = new ForumBoardViewModel(); }
        public void Include(ForumTopicViewModel vm) { var vm1 = new ForumTopicViewModel(null); }
        public void Include(ForumsStarredMessagesViewModel vm) { var vm1 = new ForumsStarredMessagesViewModel(null); }
        public void Include(ForumNewTopicViewModel vm) { var vm1 = new ForumNewTopicViewModel(); }
        public void Include(HistoryViewModel vm) { var vm1 = new HistoryViewModel(null); }
        public void Include(CharacterDetailsViewModel vm) { var vm1 = new CharacterDetailsViewModel(); }
        public void Include(StaffDetailsViewModel vm) { var vm1 = new StaffDetailsViewModel(); }
        public void Include(CharacterSearchViewModel vm) { var vm1 = new CharacterSearchViewModel(); }
        public void Include(ProfilePageViewModel vm) { var vm1 = new ProfilePageViewModel(null); }
        public void Include(LogInViewModel vm) { var vm1 = new LogInViewModel(); }
        public void Include(WallpapersViewModel vm) { var vm1 = new WallpapersViewModel(); }
        public void Include(PopularVideosViewModel vm) { var vm1 = new PopularVideosViewModel(); }
        public void Include(FriendsFeedsViewModel vm) { var vm1 = new FriendsFeedsViewModel(); }
        public void Include(NotificationsHubViewModel vm) { var vm1 = new NotificationsHubViewModel(); }
        public void Include(ListComparisonViewModel vm) { var vm1 = new ListComparisonViewModel(null,null); }
        public void Include(FriendsPageViewModel vm) { var vm1 = new FriendsPageViewModel(); }
        public void Include(ClubDetailsViewModel vm) { var vm1 = new ClubDetailsViewModel(); }
        public void Include(ClubIndexViewModel vm) { var vm1 = new ClubIndexViewModel(null); }

        public void Include(AnimeLibraryDataStorage vm) { var vm1 = new AnimeLibraryDataStorage(); }
        public void Include(HandyDataStorage vm) { var vm1 = new HandyDataStorage(null,null); }

        public void Include(ClipboardProvider vm) { var vm1 = new ClipboardProvider(); }
        public void Include(SystemControlLauncherService vm) { var vm1 = new SystemControlLauncherService(); }
        public void Include(MessageDialogProvider vm) { var vm1 = new MessageDialogProvider(); }
        public void Include(ImageDownloaderService vm) { var vm1 = new ImageDownloaderService(); }
        public void Include(TelemetryProvider vm) { var vm1 = new TelemetryProvider(null,null); }
        public void Include(NotificationTaskManager vm) { var vm1 = new NotificationTaskManager(); }
        public void Include(ScheduledJobsManager vm) { var vm1 = new ScheduledJobsManager(); }
        public void Include(CssManager vm) { var vm1 = new CssManager(); }
        public void Include(ChangelogProvider vm) { var vm1 = new ChangelogProvider(); }
        public void Include(MalHttpContextProvider vm) { var vm1 = new MalHttpContextProvider(); }
        public void Include(SnackbarProvider vm) { var vm1 = new SnackbarProvider(); }
        public void Include(ConnectionInfoProvider vm) { var vm1 = new ConnectionInfoProvider(); }
        public void Include(DispatcherAdapter vm) { var vm1 = new DispatcherAdapter(); }
        public void Include(AiringNotificationsAdapter vm) { var vm1 = new AiringNotificationsAdapter(); }

        public void Include(SettingsPageFragment vm) { var vm1 = new SettingsPageFragment(); }

        //public void Include(RedditSearchRoot vm) { var vm1 = new RedditSearchRoot(); vm1.data = new Data(); }
        //public void Include(Models.Models.Misc.Data vm) { var vm1 = new Models.Models.Misc.Data(); }
        //public void Include(Models.Models.Misc.Child vm) { var vm1 = new Models.Models.Misc.Child(); }
        //public void Include(Models.Models.Misc.Data2 vm) { var vm1 = new Models.Models.Misc.Data2(); }
        //public void Include(Models.Models.Misc.Facets vm) { var vm1 = new Models.Models.Misc.Facets(); }
        //public void Include(Models.Models.Misc.Image vm) { var vm1 = new Models.Models.Misc.Image(); }
        //public void Include(Models.Models.Misc.Media vm) { var vm1 = new Models.Models.Misc.Media(); }
        //public void Include(Models.Models.Misc.Oembed vm) { var vm1 = new Models.Models.Misc.Oembed(); }
        //public void Include(Models.Models.Misc.Oembed2 vm) { var vm1 = new Models.Models.Misc.Oembed2(); }
        //public void Include(Models.Models.Misc.SecureMediaEmbed vm) { var vm1 = new Models.Models.Misc.SecureMediaEmbed(); }
        //public void Include(Models.Models.Misc.Resolution vm) { var vm1 = new Models.Models.Misc.Resolution(); }
        //public void Include(Models.Models.Misc.Source vm) { var vm1 = new Models.Models.Misc.Source(); }
        //public void Include(Models.Models.Misc.Preview vm) { var vm1 = new Models.Models.Misc.Preview(); }
        //public void Include(Models.Models.Misc.Variants vm) { var vm1 = new Models.Models.Misc.Variants(); }
        //public void Include(Models.Models.Misc.MediaEmbed vm) { var vm1 = new Models.Models.Misc.MediaEmbed(); }
        //public void Include(Models.Models.Misc.SecureMedia vm) { var vm1 = new Models.Models.Misc.SecureMedia(); }

    }
}