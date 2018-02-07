using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.App.Job;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MALClient.Android.Activities;
using MALClient.Android.Adapters;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using Debug = System.Diagnostics.Debug;
using Exception = System.Exception;
using Void = Java.Lang.Void;

namespace MALClient.Android.Widgets
{
    [global::Android.Runtime.Preserve(AllMembers = true)]
    [Service(Exported = true)]
    public class CalendarWidgetUpdateStarterService : IntentService
    {
        protected override void OnHandleIntent(Intent intent)
        {
            var preferences = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            if (preferences.Contains("lastWidgetUpdate"))
            {
                var editor = preferences.Edit();
                editor.Remove("lastWidgetUpdate");
                editor.Commit();
            }


            var bundle = new PersistableBundle();
            bundle.PutIntArray(AppWidgetManager.ExtraAppwidgetIds, intent.GetIntArrayExtra(AppWidgetManager.ExtraAppwidgetIds));
            bundle.PutInt("ResourceId", intent.GetIntExtra("ResourceId",0));
            var component = new ComponentName(this, Class.FromType(typeof(CalendarWidgetUpdateService)));
            var scheduler = (JobScheduler)this.GetSystemService(Context.JobSchedulerService);
            scheduler.Schedule(new JobInfo.Builder(2, component).SetRequiredNetworkType(NetworkType.Any)
                .SetExtras(bundle).Build());
        }
    }

    [global::Android.Runtime.Preserve(AllMembers = true)]
    [Service(Exported = true, Permission = "android.permission.BIND_JOB_SERVICE")]
    public class CalendarWidgetUpdateService : JobService
    {
        public override bool OnStartJob(JobParameters @params)
        {
            var updater = new CalendarTask(this,@params);
            updater.Execute();
            return true;
        }

        public override bool OnStopJob(JobParameters @params)
        {
            return false;
        }

        [global::Android.Runtime.Preserve(AllMembers = true)]
        class CalendarTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, Java.Lang.Void>
        {
            private readonly CalendarWidgetUpdateService _parent;
            private readonly JobParameters _info;

            private List<Tuple<RemoteViews, int>> _views;

            public CalendarTask(CalendarWidgetUpdateService parent, JobParameters info)
            {
                _parent = parent;
                _info = info;
            }

            protected override Void RunInBackground(params Void[] @params)
            {
                Log.Debug("MalClient - Widget", "Starting update in background.");
                RunUpdate().Wait();
                return default(Void);
            }

            private async Task RunUpdate()
            {
                var manager = AppWidgetManager.GetInstance(_parent);

                int[] allWidgetIds = _info.Extras.GetIntArray(AppWidgetManager.ExtraAppwidgetIds);
                var layoutId = _info.Extras.GetInt("ResourceId", Resource.Layout.CalendarWidgetLight);
                _views = new List<Tuple<RemoteViews, int>>();
                var preferences = PreferenceManager.GetDefaultSharedPreferences(_parent.ApplicationContext);

                if (preferences.Contains("lastWidgetUpdate"))
                {
                    long lastUpdateBinary = preferences.GetLong("lastWidgetUpdate", 0);
                    var date = DateTime.FromBinary(lastUpdateBinary);
                    if (date.DayOfYear == DateTime.Today.DayOfYear)
                    {
                        Log.Debug("MalClient - Widget", "There was an update today, skipping.");
                        _parent.JobFinished(_info, false);
                        return;
                    }
                }

                var editor = preferences.Edit();
                editor.PutLong("lastWidgetUpdate", DateTime.Today.ToBinary());
                editor.Commit();

                try
                {
                    ResourceLocator.RegisterBase();
                    ResourceLocator.RegisterAppDataServiceAdapter(new ApplicationDataServiceService());
                    ResourceLocator.RegisterPasswordVaultAdapter(new PasswordVaultProvider());
                    ResourceLocator.RegisterMessageDialogAdapter(new MessageDialogProvider());
                    ResourceLocator.RegisterDataCacheAdapter(new Adapters.DataCache(null));

                    Credentials.Init();
                }
                catch (Exception e)
                {

                }
                bool running = ResourceLocator.AnimeLibraryDataStorage.AllLoadedAuthAnimeItems?.Any() ?? false;
                foreach (var widgetId in allWidgetIds)
                {
                    var view = new RemoteViews(_parent.PackageName, layoutId);
                    _views.Add(new Tuple<RemoteViews, int>(view, widgetId));

                    view.SetViewVisibility(Resource.Id.LoadingSpinner, ViewStates.Visible);
                    view.SetViewVisibility(Resource.Id.EmptyNotice, ViewStates.Gone);
                    //view.SetViewVisibility(Resource.Id.RefreshButton, ViewStates.Gone);
                    view.SetViewVisibility(Resource.Id.GridView, ViewStates.Gone);

                    manager.UpdateAppWidget(widgetId, view);
                }

                CalendarPivotPage shows = null;

                try
                {
                    if (Credentials.Authenticated)
                    {

                        if (!running)
                        {
                            await ResourceLocator.AiringInfoProvider.Init(false);

                            ViewModelLocator.AnimeList.ListSource = Credentials.UserName;
                            await ViewModelLocator.AnimeList.FetchData(true, AnimeListWorkModes.Anime);

                        }
                        if (ResourceLocator.AiringInfoProvider.InitializationSuccess && (ResourceLocator.AnimeLibraryDataStorage.AllLoadedAuthAnimeItems?.Any() ?? false))
                        {
                            await ViewModelLocator.CalendarPage.Init(true);

                            shows =
                                ViewModelLocator.CalendarPage.CalendarData.FirstOrDefault(
                                    page => page.DayOfWeek == DateTime.Now.DayOfWeek);
                        }

                    }
                }
                catch (Exception e)
                {
                    //we have failed very very badly
                }

                await Task.Delay(1000); // give visual feedback


                if (shows != null && shows.Items.Any())
                {
                    foreach (var view in _views)
                    {
                        view.Item1.SetViewVisibility(Resource.Id.LoadingSpinner, ViewStates.Gone);
                        view.Item1.SetViewVisibility(Resource.Id.EmptyNotice, ViewStates.Gone);
                        view.Item1.SetViewVisibility(Resource.Id.GridView, ViewStates.Visible);
                        view.Item1.SetRemoteAdapter(Resource.Id.GridView, new Intent(_parent.ApplicationContext, typeof(CalendarWidgetRemoteViewsService)));

                        var intentTemplate = new Intent(_parent.ApplicationContext, typeof(MainActivity));
                        view.Item1.SetPendingIntentTemplate(Resource.Id.GridView, PendingIntent.GetActivity(_parent.ApplicationContext, 0, intentTemplate, 0));

                        var refreshIntent = new Intent(_parent.ApplicationContext, typeof(CalendarWidgetUpdateStarterService));
                        refreshIntent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, new[] { view.Item2 });
                        refreshIntent.PutExtra("ResourceId", layoutId);
                        view.Item1.SetOnClickPendingIntent(Resource.Id.RefreshButton, PendingIntent.GetService(_parent.ApplicationContext, 0, refreshIntent, 0));

                        var ids = preferences.GetStringSet("lastWidgetItems", new List<string>()).Select(int.Parse).ToList();
                        if (!ids.OrderBy(i => i).SequenceEqual(shows.Items.Select(model => model.Id).OrderBy(i => i)))
                        {
                            Log.Debug("MalClient - Widget", "New items detected. Refreshing data source.");
                            manager.NotifyAppWidgetViewDataChanged(view.Item2, Resource.Id.GridView);

                            var edit = preferences.Edit();
                            edit.PutStringSet("lastWidgetItems", shows.Items.Select(model => model.Id.ToString()).ToList());
                            edit.Commit();
                        }



                        manager.UpdateAppWidget(view.Item2, view.Item1);
                    }
                }
                else
                {
                    foreach (var view in _views)
                    {
                        view.Item1.SetViewVisibility(Resource.Id.LoadingSpinner, ViewStates.Gone);
                        view.Item1.SetViewVisibility(Resource.Id.EmptyNotice, ViewStates.Visible);
                        view.Item1.SetViewVisibility(Resource.Id.GridView, ViewStates.Gone);


                        var refreshIntent = new Intent(_parent.ApplicationContext, typeof(CalendarWidgetUpdateStarterService));
                        refreshIntent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, new[] { view.Item2 });
                        refreshIntent.PutExtra("ResourceId", layoutId);
                        view.Item1.SetOnClickPendingIntent(Resource.Id.RefreshButton, PendingIntent.GetService(_parent.ApplicationContext, 0, refreshIntent, 0));


                        manager.UpdateAppWidget(view.Item2, view.Item1);
                    }
                }

                await Task.Delay(6000); //let the widget update in peace...

                _parent.JobFinished(_info,false);
            }
        }

    }

    
}