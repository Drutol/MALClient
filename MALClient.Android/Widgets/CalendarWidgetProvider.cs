using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.App.Job;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Debug = System.Diagnostics.Debug;

namespace MALClient.Android.Widgets
{
    [Preserve(AllMembers = true)]
    [BroadcastReceiver(Label = "Airing - Dark")]
    [IntentFilter(new string[] {"android.appwidget.action.APPWIDGET_UPDATE"})]
    [MetaData("android.appwidget.provider", Resource = "@xml/calendar_widget_info_dark")]
    public class CalendarWidgetProviderDark : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            ComponentName thisWidget = new ComponentName(context,
                Class.FromType(typeof(CalendarWidgetProviderDark)));
            int[] allWidgetIds = appWidgetManager.GetAppWidgetIds(thisWidget);

            // Update the widgets via the JobScheduler
            var bundle = new PersistableBundle();
            bundle.PutIntArray(AppWidgetManager.ExtraAppwidgetIds, allWidgetIds);
            bundle.PutInt("ResourceId", Resource.Layout.CalendarWidgetDark);
            var component = new ComponentName(context, Class.FromType(typeof(CalendarWidgetUpdateService)));
            var scheduler = (JobScheduler)context.GetSystemService(Context.JobSchedulerService);
            scheduler.Schedule(new JobInfo.Builder(2, component).SetRequiredNetworkType(NetworkType.Any)
                .SetExtras(bundle).Build());
        }

        public override void OnDeleted(Context context, int[] appWidgetIds)
        {
            var preferences = PreferenceManager.GetDefaultSharedPreferences(context);
            if (preferences.Contains("lastWidgetUpdate"))
            {
                var editor = preferences.Edit();
                editor.Remove("lastWidgetUpdate");
                editor.Commit();
            }
            base.OnDeleted(context, appWidgetIds);
        }
    }

    [Preserve(AllMembers = true)]
    [BroadcastReceiver(Label = "Airing - Light")]
    [IntentFilter(new string[] {"android.appwidget.action.APPWIDGET_UPDATE"})]
    [MetaData("android.appwidget.provider", Resource = "@xml/calendar_widget_info_light")]
    public class CalendarWidgetProviderLight : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            ComponentName thisWidget = new ComponentName(context,
                Class.FromType(typeof(CalendarWidgetProviderLight)));
            int[] allWidgetIds = appWidgetManager.GetAppWidgetIds(thisWidget);


            // Update the widgets via the JobScheduler
            var bundle = new PersistableBundle();
            bundle.PutIntArray(AppWidgetManager.ExtraAppwidgetIds, allWidgetIds);
            bundle.PutInt("ResourceId", Resource.Layout.CalendarWidgetLight);
            var component = new ComponentName(context, Class.FromType(typeof(CalendarWidgetUpdateService)));
            var scheduler = (JobScheduler) context.GetSystemService(Context.JobSchedulerService);
            scheduler.Schedule(new JobInfo.Builder(2, component).SetRequiredNetworkType(NetworkType.Any)
                .SetExtras(bundle).Build());
        }

        public override void OnDeleted(Context context, int[] appWidgetIds)
        {
            var preferences = PreferenceManager.GetDefaultSharedPreferences(context);
            if (preferences.Contains("lastWidgetUpdate"))
            {
                var editor = preferences.Edit();
                editor.Remove("lastWidgetUpdate");
                editor.Commit();
            }
            base.OnDeleted(context, appWidgetIds);
        }
    }
}