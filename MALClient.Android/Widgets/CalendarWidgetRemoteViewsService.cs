using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using MALClient.Android.Activities;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Widgets
{
    [global::Android.Runtime.Preserve(AllMembers = true)]
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS")]
    public class CalendarWidgetRemoteViewsService : RemoteViewsService
    {
        [global::Android.Runtime.Preserve(AllMembers = true)]
        class CalendarViewFactory : Java.Lang.Object, IRemoteViewsFactory
        {
            private readonly Context _applicationContext;
            private List<AnimeItemViewModel> _items;

            public CalendarViewFactory(Context applicationContext)
            {
                _applicationContext = applicationContext;

            }

            public long GetItemId(int position)
            {
                return _items[position].Id;
            }

            public RemoteViews GetViewAt(int position)
            {
                var views = new RemoteViews(_applicationContext.PackageName, Resource.Layout.CalendarWidgetItem);
                var vm = _items[position];

                var intent = new Intent(_applicationContext, typeof(MainActivity));
                intent.PutExtra("launchArgs", $"https://myanimelist.net/anime/{vm.Id}");
                views.SetOnClickFillInIntent(Resource.Id.Image,intent);
                if (ResourceLocator.AiringInfoProvider.TryGetCurrentEpisode(vm.Id, out int ep, DateTime.Today))
                {
                    views.SetTextViewText(Resource.Id.EpisodeCount,$"ep. {ep}");
                    views.SetViewVisibility(Resource.Id.EpisodeCount, ViewStates.Visible);
                }
                else
                {
                   views.SetViewVisibility(Resource.Id.EpisodeCount,ViewStates.Gone);
                }
                Loadimage(vm, views);
      
                return views;
            }

            public void OnCreate()
            {


            }

            public void OnDataSetChanged()
            {
                try
                {
                    _items = ViewModelLocator.CalendarPage.CalendarData.First(
                            page => page.DayOfWeek == DateTime.Now.DayOfWeek).Items
                        .OrderByDescending(model => model.MyEpisodes)
                        .ThenByDescending(model => model.MyScore)
                        .Take(14)
                        .ToList();
                    Log.Debug("MalClient - Widget", $"Todays airing items count {_items.Count}");
                }
                catch (Exception e)
                {
                    Log.Debug("MalClient - Widget", e.ToString());
                    _items = new List<AnimeItemViewModel>();
                }
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
                try
                {
                    var bitmap = ImageService.Instance.LoadUrl(vm.ImgUrl).AsBitmapDrawableAsync().Result;
                    views.SetImageViewBitmap(Resource.Id.Image, bitmap.Bitmap);
                }
                catch (Exception)
                {
                    //probably no internet
                }

            }
        }

        public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
        {
            
            return new CalendarViewFactory(ApplicationContext);
        }
    }
}