using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Interfaces;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Managers;
using MALClient.XShared.ViewModels;

namespace MALClient.XShared.BL
{
    /// <summary>
    /// Platfrom independent init chain
    /// </summary>
    public static class InitializationRoutines
    {
        public static TaskCompletionSource<bool> AwaitableCompletion { get; } = new TaskCompletionSource<bool>();

        public static async Task InitApp()
        {
            ResourceLocator.ConnectionInfoProvider.Init();
            Credentials.Init();
            FavouritesManager.LoadData();
            AnimeImageQuery.Init();
            ViewModelLocator.ForumsMain.LoadPinnedTopics();
            if (Credentials.Authenticated)
            {
                // preload access token
                await ResourceLocator.MalHttpContextProvider.GetApiHttpContextAsync();
            }

            await Task.WhenAll(
                ResourceLocator.AiringInfoProvider.Init(false),
                ResourceLocator.EnglishTitlesProvider.Init());

            if (Settings.NotificationCheckInRuntime && Credentials.Authenticated)
                ResourceLocator.SchdeuledJobsManger.StartJob(ScheduledJob.FetchNotifications, 5,
                    () => { ResourceLocator.NotificationsTaskManager.CallTask(BgTasks.Notifications); });
            ResourceLocator.HandyDataStorage.Init();
            AwaitableCompletion.SetResult(true);

            try
            {
                using var client = new HttpClient();
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var result = await client.GetAsync("https://myanimelist.net", cts.Token);

                result.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                ResourceLocator.DispatcherAdapter.Run(() =>
                {
                    ResourceLocator.MessageDialogProvider.ShowMessageDialog(
                        "Failed to connect to MyAnimeList.net. Website is probably down, try checking on the browser. If it's down try again later.",
                        "MAL is down");
                });
            }


        }

        public static void InitPostUpdate()
        {
            var previousVersion = Settings.AppVersion;
            var currentVersion = ResourceLocator.ChangelogProvider.CurrentVersion;
            var isNewVersion = false;
            if (previousVersion != null)
            {
                if (previousVersion.Substring(0, previousVersion.LastIndexOf('.')) !=
                    currentVersion.Substring(0, currentVersion.LastIndexOf('.')))
                    isNewVersion = true;
            }

            ResourceLocator.ChangelogProvider.NewChangelog = isNewVersion;
            Settings.AppVersion = ResourceLocator.ChangelogProvider.CurrentVersion;
        }
    }
}
