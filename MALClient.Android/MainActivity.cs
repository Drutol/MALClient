using System;
using System.Net;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.Net;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Interfaces;
using Uri = Android.Net.Uri;

namespace MALClient.Android
{
    [Activity(Label = "MALClient.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity , IDimensionsProvider
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            ViewModelLocator.RegisterBase();
            AndroidViewModelLocator.RegisterDependencies();
            Credentials.SetAuthStatus(true);
            Credentials.Update("MALClientTestAcc","MuchVerificatio",ApiType.Mal);            
            ViewModelLocator.AnimeList.Init(null);
            ViewModelLocator.AnimeList.Initialized += AnimeListOnInitialized;

        }

        private void AnimeListOnInitialized()
        {
            var gridview = FindViewById<GridView>(Resource.Id.gridview1);
            gridview.Adapter = new ImageAdapter(this);

            gridview.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args) {
                Toast.MakeText(this, args.Position.ToString(), ToastLength.Short).Show();
            };
        }

        public double ActualWidth => 800;
        public double ActualHeight => 1200;
    }

    public class ImageAdapter : BaseAdapter
    {
        Context context;

        public ImageAdapter(Context c)
        {
            context = c;
        }

        public override int Count
        {
            get { return ViewModelLocator.AnimeList.AnimeGridItems.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        // create a new ImageView for each item referenced by the Adapter
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageView imageView;

            if (convertView == null)
            {  // if it's not recycled, initialize some attributes
                imageView = new ImageView(context);
                imageView.LayoutParameters = new GridView.LayoutParams(200, 350);
                imageView.SetScaleType(ImageView.ScaleType.FitXy);
                imageView.SetPadding(8, 8, 8, 8);
            }
            else
            {
                imageView = (ImageView)convertView;
            }
            var imageBitmap = GetImageBitmapFromUrl(ViewModelLocator.AnimeList.AnimeGridItems[position].ImgUrl);
            imageView.SetImageBitmap(imageBitmap);
            return imageView;
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }

    }
}

