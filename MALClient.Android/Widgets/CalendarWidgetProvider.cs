using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Debug = System.Diagnostics.Debug;

namespace MALClient.Android.Widgets
{
    [BroadcastReceiver(Label = "Anime Calendar Widget - Dark")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/calendar_widget_info_dark")]
    public class CalendarWidgetProviderDark : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            Debug.WriteLine("Updating widget");
            ComponentName thisWidget = new ComponentName(context,
                Class.FromType(typeof(CalendarWidgetProviderDark)));
            int[] allWidgetIds = appWidgetManager.GetAppWidgetIds(thisWidget);

            // Build the intent to call the service
            Intent intent = new Intent(context.ApplicationContext, typeof(CalendarWidgetUpdateService));
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, allWidgetIds);
            intent.PutExtra("ResourceId", Resource.Layout.CalendarWidgetDark);
            // Update the widgets via the service
            context.StartService(intent);
        }
    }

    [BroadcastReceiver(Label = "Anime Calendar Widget - Light")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/calendar_widget_info_light")]
    public class CalendarWidgetProviderLight : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            ComponentName thisWidget = new ComponentName(context,
                Class.FromType(typeof(CalendarWidgetProviderLight)));
            int[] allWidgetIds = appWidgetManager.GetAppWidgetIds(thisWidget);

            // Build the intent to call the service
            Intent intent = new Intent(context.ApplicationContext, typeof(CalendarWidgetUpdateService));
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, allWidgetIds);
            intent.PutExtra("ResourceId", Resource.Layout.CalendarWidgetLight);

            // Update the widgets via the service
            context.StartService(intent);
        }
    }
}