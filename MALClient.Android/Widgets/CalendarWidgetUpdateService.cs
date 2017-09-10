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
    public class CalendarWidgetUpdateService : IntentService
    {
        private List<Tuple<RemoteViews, int>> _views;

        protected override async void OnHandleIntent(Intent intent)
        {
            var manager = AppWidgetManager.GetInstance(this);

            int[] allWidgetIds = intent
                .GetIntArrayExtra(AppWidgetManager.ExtraAppwidgetIds);
            var layoutId = intent.GetIntExtra("ResourceId", Resource.Layout.CalendarWidgetLight);
            _views = new List<Tuple<RemoteViews,int>>();
           
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
                _views.Add(new Tuple<RemoteViews, int>(view,widgetId));

                view.SetViewVisibility(Resource.Id.LoadingSpinner, ViewStates.Visible);
                view.SetViewVisibility(Resource.Id.EmptyNotice, ViewStates.Gone);
                //view.SetViewVisibility(Resource.Id.RefreshButton, ViewStates.Gone);
                view.SetViewVisibility(Resource.Id.GridView, ViewStates.Gone);

                manager.UpdateAppWidget(widgetId,view);
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

                    await ViewModelLocator.CalendarPage.Init(true);

                    shows =
                        ViewModelLocator.CalendarPage.CalendarData.FirstOrDefault(
                            page => page.DayOfWeek == DateTime.Now.DayOfWeek);

                }
            }
            catch (Exception)
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
                    view.Item1.SetRemoteAdapter(Resource.Id.GridView,new Intent(ApplicationContext,typeof(CalendarWidgetRemoteViewsService)));           

                    var intentTemplate = new Intent(ApplicationContext, typeof(MainActivity));
                    view.Item1.SetPendingIntentTemplate(Resource.Id.GridView, PendingIntent.GetActivity(ApplicationContext, 0, intentTemplate, 0));

                    var refreshIntent = new Intent(ApplicationContext, typeof(CalendarWidgetUpdateService));
                    refreshIntent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, new[] { view.Item2 });
                    refreshIntent.PutExtra("ResourceId", layoutId);
                    view.Item1.SetOnClickPendingIntent(Resource.Id.RefreshButton, PendingIntent.GetService(ApplicationContext, 0, refreshIntent, 0));

                    manager.UpdateAppWidget(view.Item2, view.Item1);
                }
            }
            else
            {
                foreach (var view in _views)
                {
                    view.Item1.SetViewVisibility(Resource.Id.LoadingSpinner,ViewStates.Gone);
                    view.Item1.SetViewVisibility(Resource.Id.EmptyNotice,ViewStates.Visible);
                    view.Item1.SetViewVisibility(Resource.Id.GridView,ViewStates.Gone);
                    

                    var refreshIntent = new Intent(ApplicationContext, typeof(CalendarWidgetUpdateService));
                    refreshIntent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, new[] {view.Item2});
                    refreshIntent.PutExtra("ResourceId", layoutId);
                    view.Item1.SetOnClickPendingIntent(Resource.Id.RefreshButton, PendingIntent.GetService(ApplicationContext, 0, refreshIntent, 0));


                    manager.UpdateAppWidget(view.Item2, view.Item1);
                }
            }

            await Task.Delay(6000); //let the widget update in peace...

        }

    }

    
}