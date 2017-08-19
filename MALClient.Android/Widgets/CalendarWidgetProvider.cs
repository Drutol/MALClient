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

namespace MALClient.Android.Widgets
{
    [BroadcastReceiver(Label = "Anime Calendar Widget")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/calendar_widget_info")]
    public class CalendarWidgetProvider : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            ComponentName thisWidget = new ComponentName(context,
                Class.FromType(typeof(CalendarWidgetProvider)));
            int[] allWidgetIds = appWidgetManager.GetAppWidgetIds(thisWidget);

            // Build the intent to call the service
            Intent intent = new Intent(context.ApplicationContext, typeof(CalendarWidgetUpdateService));
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, allWidgetIds);

            // Update the widgets via the service
            context.StartService(intent);
        }
    }
}