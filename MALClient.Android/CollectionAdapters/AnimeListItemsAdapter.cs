using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Adapters;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.CollectionAdapters
{
    public class AnimeListItemsAdapter : ObservableCollectionAdapter<AnimeItemViewModel>
    {
        public AnimeListItemsAdapter(Activity context, int resource, ObservableCollection<AnimeItemViewModel> items)
            : base(context, resource, items)
        {

        }

        private readonly Dictionary<int,Dictionary<int,Binding>> _bindings = new Dictionary<int, Dictionary<int, Binding>>();

        protected override async void PrepareView(AnimeItemViewModel item, View view)
        {
            if(!_bindings.ContainsKey(item.Id))
                _bindings.Add(item.Id,new Dictionary<int, Binding>());

            if (!_bindings[item.Id].ContainsKey(Resource.Id.AnimeGridItemTitle))
            {
                var titleView = view.FindViewById<TextView>(Resource.Id.AnimeGridItemTitle);
                _bindings[item.Id].Add(Resource.Id.AnimeGridItemTitle,
                    new Binding<string, string>(item, () => item.Title, titleView, () => titleView.Text));
            }
            if (!_bindings[item.Id].ContainsKey(Resource.Id.AnimeGridItemImage))
            {
                _bindings[item.Id].Add(Resource.Id.AnimeGridItemImage,null);
                var img = view.FindViewById<ImageView>(Resource.Id.AnimeGridItemImage);
                img.SetImageBitmap(await GetImageBitmapFromUrl(item.ImgUrl));
            }


        }

        protected override long GetItemId(AnimeItemViewModel item, int position)
        {
            return item.Id;
        }

        private async Task<Bitmap> GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new HttpClient())
            {
                var response = await webClient.GetAsync(new Uri(url));
                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }
}