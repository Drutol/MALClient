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
using Android.Views;
using Android.Widget;
using Java.Lang;
using MALClient.Android.Adapters;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
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
            catch (Exception)
            {
                //may be already registered... voodoo I guess
            }

            foreach (var widgetId in allWidgetIds)
            {
                var view = new RemoteViews(PackageName, Resource.Layout.CalendarWidget);
                views.Add(new Tuple<RemoteViews, int>(view,widgetId)); 
                manager.UpdateAppWidget(widgetId,view);
            }

            Debug.WriteLine("Fetching");
            await ViewModelLocator.AnimeList.FetchData(true, AnimeListWorkModes.Anime);

            Debug.WriteLine("Loading calendar");
            await ViewModelLocator.CalendarPage.Init(true);

            var shows =
                ViewModelLocator.CalendarPage.CalendarData.FirstOrDefault(
                    page => page.DayOfWeek == DateTime.Now.DayOfWeek);

            Debug.WriteLine($"Loaded shows == null = {shows == null}");
            if (shows != null)
            {
                foreach (var view in views)
                {
                    view.Item1.SetViewVisibility(Resource.Id.LoadingSpinner, ViewStates.Gone);
                    view.Item1.SetViewVisibility(Resource.Id.EmptyNotice, ViewStates.Gone);
                    view.Item1.SetViewVisibility(Resource.Id.GridView, ViewStates.Visible);
                    view.Item1.SetRemoteAdapter(Resource.Id.GridView,new Intent(ApplicationContext,typeof(CalendarWidgetRemoteViewsService)));
                    manager.UpdateAppWidget(view.Item2, view.Item1);
                }
            }
            else
            {
                foreach (var view in views)
                {
                    view.Item1.SetViewVisibility(Resource.Id.LoadingSpinner,ViewStates.Gone);
                    view.Item1.SetViewVisibility(Resource.Id.EmptyNotice,ViewStates.Visible);
                    manager.UpdateAppWidget(view.Item2, view.Item1);
                }
            }
        }
    }
}