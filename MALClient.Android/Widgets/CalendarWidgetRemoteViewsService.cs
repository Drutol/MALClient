using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using MALClient.Android.Activities;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Widgets
{
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS")]
    public class CalendarWidgetRemoteViewsService : RemoteViewsService
    {
        
        class CalendarViewFactory : Java.Lang.Object, IRemoteViewsFactory
        {
            private List<AnimeItemViewModel> _items;

            public long GetItemId(int position)
            {
                return _items[position].Id;
            }

            public RemoteViews GetViewAt(int position)
            {
                var views = new RemoteViews(Application.Context.PackageName, Resource.Layout.CalendarWidgetItem);
                var vm = _items[position];

                var intent = new Intent(Application.Context, typeof(MainActivity));
                intent.SetAction(DateTime.Now.Ticks.ToString());
                intent.PutExtra("launchArgs", $"https://myanimelist.net/anime/{vm.Id}");

                views.SetTextViewText(Resource.Id.Title,vm.Title);
                views.SetOnClickPendingIntent(Resource.Id.Root, PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.UpdateCurrent));

                 Loadimage(vm, views);
      
                return views;
            }

            public void OnCreate()
            {
                _items = ViewModelLocator.CalendarPage.CalendarData.First(
                    page => page.DayOfWeek == DateTime.Now.DayOfWeek).Items;
            }

            public void OnDataSetChanged()
            {

            }

            public void OnDestroy()
            {

            }

            public int Count => _items.Count;
            public bool HasStableIds { get; } = false;
            public RemoteViews LoadingView { get; } = null;
            public int ViewTypeCount { get; } = 1;

            private void Loadimage(AnimeItemViewModel vm, RemoteViews views)
            {
                var bitmap = ImageService.Instance.LoadUrl(vm.ImgUrl).AsBitmapDrawableAsync().Result;
                views.SetImageViewBitmap(Resource.Id.Image,bitmap.Bitmap);
            }
        }

        public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
        {
            return new CalendarViewFactory();
        }
    }
}