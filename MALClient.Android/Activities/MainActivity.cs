using System;
using System.Net;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Ioc;
using MALClient.Android.ViewModels;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Interfaces;
using Fragment = Android.Support.V4.App.Fragment;

namespace MALClient.Android.Activities
{
    [Activity(Label = "MALClient", MainLauncher = true, Icon = "@drawable/icon", LaunchMode = LaunchMode.SingleTop,Theme = "@style/Theme.AppCompat")]
    public partial class MainActivity : AppCompatActivity , IDimensionsProvider
    {
        private MainViewModel _viewModel;
        private bool _addedNavHandlers;

        private MainViewModel ViewModel => _viewModel ?? (_viewModel = SimpleIoc.Default.GetInstance<MainViewModel>());

        protected override void OnCreate(Bundle bundle)
        {
            SetContentView(Resource.Layout.MainPage);
            if (!_addedNavHandlers)
            {
                ViewModel.MainNavigationRequested += ViewModelOnMainNavigationRequested;
            }
            base.OnCreate(bundle);       
            NavView.NavigationItemSelected += NavViewOnNavigationItemSelected;
        }

        private void ViewModelOnMainNavigationRequested(Fragment fragment)
        {
            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.MainContentFrame, fragment)
                .Commit();
        }


        private void NavViewOnNavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            e.MenuItem.SetChecked(true);

            switch (e.MenuItem.ItemId)
            {
                case Resource.Id.MainHamburgerBtnLogIn:
                    ViewModelLocator.GeneralMain.Navigate(PageIndex.PageLogIn);
                    break;
            }

            DrawerLayout.CloseDrawers();
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
            //ImageView imageView;

            //if (convertView == null)
            //{  // if it's not recycled, initialize some attributes
            //    imageView = new ImageView(context);
            //    imageView.LayoutParameters = new GridView.LayoutParams(200, 350);
            //    imageView.SetScaleType(ImageView.ScaleType.FitXy);
            //    imageView.SetPadding(8, 8, 8, 8);
            //}
            //else
            //{
            //    imageView = (ImageView)convertView;
            //}
            //var imageBitmap = GetImageBitmapFromUrl(ViewModelLocator.AnimeList.AnimeGridItems[position].ImgUrl);
            //imageView.SetImageBitmap(imageBitmap);
            //return imageView;
            if (convertView == null)
            {
                var item = ViewModelLocator.AnimeList.AnimeGridItems[position];
                var contentView = LayoutInflater.From(context).Inflate(Resource.Layout.AnimeGridItem, null);


                var name = contentView.FindViewById<TextView>(Resource.Id.ShowTitle);
                name.Text = item.Title;

                var info = contentView.FindViewById<ImageView>(Resource.Id.ShowCoverImage);
                info.SetImageBitmap(GetImageBitmapFromUrl(item.ImgUrl));

                return contentView;
            }
            else
                return convertView;
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

