using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
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

namespace MALClient.Android.Widgets
{
    [Service(Exported = true)]
    [IntentFilter(new[] { "android.intent.action.DATE_CHANGED" })]
    public class CalendarWidgetUpdateService : IntentService
    {
        protected override async void OnHandleIntent(Intent intent)
        {
            AppWidgetManager manager = AppWidgetManager.GetInstance(this);

            int[] allWidgetIds = intent
                .GetIntArrayExtra(AppWidgetManager.ExtraAppwidgetIds);
            var layoutId = intent.GetIntExtra("ResourceId", Resource.Layout.CalendarWidgetLight);
            var views = new List<Tuple<RemoteViews,int>>();
           
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
                var view = new RemoteViews(PackageName, layoutId);
                views.Add(new Tuple<RemoteViews, int>(view,widgetId)); 

                manager.UpdateAppWidget(widgetId,view);
            }

            CalendarPivotPage shows = null;
            if (Credentials.Authenticated)
            {

                if (!running)
                {
                   
                    ViewModelLocator.AnimeList.ListSource = Credentials.UserName;
                    await ViewModelLocator.AnimeList.FetchData(true, AnimeListWorkModes.Anime);
                }

                await ViewModelLocator.CalendarPage.Init(true);

                shows =
                    ViewModelLocator.CalendarPage.CalendarData.FirstOrDefault(
                        page => page.DayOfWeek == DateTime.Now.DayOfWeek);

            }

            if (shows != null && shows.Items.Any())
            {

                foreach (var view in views)
                {
                    view.Item1.SetViewVisibility(Resource.Id.LoadingSpinner, ViewStates.Gone);
                    view.Item1.SetViewVisibility(Resource.Id.EmptyNotice, ViewStates.Gone);
                    view.Item1.SetViewVisibility(Resource.Id.GridView, ViewStates.Visible);
                    view.Item1.SetRemoteAdapter(Resource.Id.GridView,new Intent(ApplicationContext,typeof(CalendarWidgetRemoteViewsService)));

                    var intentTemplate = new Intent(ApplicationContext, typeof(MainActivity));
                    view.Item1.SetPendingIntentTemplate(Resource.Id.GridView, PendingIntent.GetActivity(ApplicationContext, 0, intentTemplate, 0));

                    manager.UpdateAppWidget(view.Item2, view.Item1);
                }
            }
            else
            {
                foreach (var view in views)
                {
                    view.Item1.SetViewVisibility(Resource.Id.LoadingSpinner,ViewStates.Gone);
                    view.Item1.SetViewVisibility(Resource.Id.EmptyNotice,ViewStates.Visible);
                    view.Item1.SetViewVisibility(Resource.Id.GridView,ViewStates.Gone);
                    manager.UpdateAppWidget(view.Item2, view.Item1);
                }
            }
        }
    }
}