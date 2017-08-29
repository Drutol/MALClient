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
                try
                {
                    _items = ViewModelLocator.CalendarPage.CalendarData.First(
                            page => page.DayOfWeek == DateTime.Now.DayOfWeek).Items
                        .OrderByDescending(model => model.MyEpisodes)
                        .ThenByDescending(model => model.MyScore)
                        .Take(8)
                        .ToList();
                }
                catch (Exception )
                {
                    _items = new List<AnimeItemViewModel>();
                }

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