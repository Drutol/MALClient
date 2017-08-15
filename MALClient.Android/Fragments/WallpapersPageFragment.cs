using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Com.Mikepenz.Materialdrawer;
using Com.Mikepenz.Materialdrawer.Model;
using Com.Mikepenz.Materialdrawer.Model.Interfaces;
using Com.Shehabic.Droppy;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Android.UserControls;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Items;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public class WallpapersPageFragment : MalFragmentBase
    {
        private WallpapersViewModel ViewModel;
        private readonly SmartObservableCollection<WallpaperItemViewModel> _wallpapers = new SmartObservableCollection<WallpaperItemViewModel>();
        private DroppyMenuPopup _menu;
        private WallpaperItemViewModel _menuContext;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModelLocator.NavMgr.DeregisterBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
            ViewModel = ViewModelLocator.Wallpapers;
            ViewModel.Init(null);
        }

        protected override void InitBindings()
        {
            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingWallpapersVisibility,
                    () => WallpapersPageProgressSpinner.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));

            WallpapersPageList.InjectFlingAdapter(_wallpapers,DataTemplateFull,DataTemplateFling,ContainerTemplate,DataTemplateBasic  );
            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
            {
                WallpapersPageActionButtonLoadMore.Show();
            }
            else
            {
                WallpapersPageActionButtonLoadMore.Visibility = ViewStates.Gone;
                WallpapersPageList.SetOnScrollChangeListener(new ScrollListener(i =>
                {
                    if (ViewModel.LoadingWallpapersVisibility)
                        return;
                    if (WallpapersPageList.Adapter.Count - WallpapersPageList.FirstVisiblePosition <= 2)
                        ViewModel.GoForwardCommand.Execute(null);
                }));
            }


            Bindings.Add(this.SetBinding(() => ViewModel.Wallpapers)
                .WhenSourceChanges(() =>
                {
                    if (ViewModel.Wallpapers != null)
                    {
                        if (ViewModel.Wallpapers.Count == 0)
                            _wallpapers.Clear();
                        else
                            _wallpapers.AddRange(ViewModel.Wallpapers);
                    }
                }));

            (RootView as ScrollableSwipeToRefreshLayout).ScrollingView = WallpapersPageList;
            (RootView as ScrollableSwipeToRefreshLayout).Refresh += OnRefresh;

            InitDrawer();
            WallpapersPageActionButtonFilter.Click += (sender, args) => OpenFiltersDrawer();
            WallpapersPageActionButtonLoadMore.Click += (sender, args) =>
            {
                if (!ViewModel.LoadingWallpapersVisibility)
                    ViewModel.GoForwardCommand.Execute(null);
            };
        }

        private void OnRefresh(object o, EventArgs eventArgs)
        {
            _wallpapers.Clear();
            (RootView as ScrollableSwipeToRefreshLayout).Refreshing = false;
            ViewModel.RefreshCommand.Execute(null);
        }

        private void DataTemplateBasic(View view, int i2, WallpaperItemViewModel arg3)
        {
            view.FindViewById<TextView>(Resource.Id.WallpapersPageItemDate).Text = arg3.Created;
            view.FindViewById<TextView>(Resource.Id.WallpapersPageItemSubreddit).Text = $"/r/{arg3.Data.Source}";
            view.FindViewById<TextView>(Resource.Id.WallpapersPageItemTitle).Text = arg3.Data.Title;
            view.FindViewById<TextView>(Resource.Id.WallpapersPageItemUpvotes).Text = arg3.Data.Upvotes.ToString();
            view.FindViewById(Resource.Id.WallpapersPageItemRootContainer).Tag = arg3.Wrap();
        }

        private View ContainerTemplate(int i2)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.WallpapersPageItem, null);
            view.FindViewById(Resource.Id.WallpapersPageItemRootContainer).SetOnLongClickListener(new OnLongClickListener(OnLongClickAction));
            view.FindViewById(Resource.Id.WallpapersPageItemRootContainer).SetOnClickListener(new OnClickListener(OnClick));
            view.FindViewById(Resource.Id.WallpapersPageItemResolution).Visibility = ViewStates.Gone;
            return view;
        }



        private void DataTemplateFling(View view, int i2, WallpaperItemViewModel arg3)
        {
            var img = view.FindViewById<ImageViewAsync>(Resource.Id.WallpapersPageItemImage);        
            if (!img.IntoIfLoaded(arg3.Data.FileUrl, arg3.IsBlurred ? new BlurredTransformation(40) : null))
                img.Visibility = ViewStates.Gone;
        }

        private void DataTemplateFull(View view, int i2, WallpaperItemViewModel arg3)
        {
            var img = view.FindViewById<ImageViewAsync>(Resource.Id.WallpapersPageItemImage);
            if (img.Tag == null || (string) img.Tag != arg3.Data.FileUrl)
            {
                img.Into(arg3.Data.Thumb, arg3.IsBlurred ? new BlurredTransformation(40) : null,null, 350);
            }
        }

        //private void OnCompleted(ImageViewAsync imageViewAsync)
        //{
        //    Activity?.RunOnUiThread(() =>
        //    {
        //        try
        //        {
        //            var parent = (imageViewAsync.Parent as View);
        //            var vm = parent.Tag.Unwrap<WallpaperItemViewModel>();
        //            if (!string.IsNullOrEmpty(vm.Resolution))
        //                return;

        //            var bounds = imageViewAsync.Drawable.Bounds;
        //            vm.Resolution = $"{bounds.Right}x{bounds.Bottom}";
        //            parent.FindViewById<TextView>(Resource.Id.WallpapersPageItemResolution).Text = vm.Resolution;

        //            imageViewAsync.Visibility = ViewStates.Visible;
        //        }
        //        catch (Exception)
        //        {
        //            // user navigated out and image has loaded in background
        //        }
                
        //    });
        //}

        private void OnLongClickAction(View view)
        {
            _menuContext = view.Tag.Unwrap<WallpaperItemViewModel>();
            _menu = FlyoutMenuBuilder.BuildGenericFlyout(Context, view, new List<string>
            {
                "Save",
                "Copy link & open waifu2x",
                "Open on reddit",
                "Copy link",
            }, OnMenuSelected);
            _menu.Show();
        }

        private void OnMenuSelected(int i)
        {
            if(_menuContext == null)
                return;
            switch (i)
            {
                case 0:
                    _menuContext.SaveCommand.Execute(null);
                    break;
                case 1:
                    _menuContext.CopyAndWaifuCommand.Execute(null);
                    break;
                case 2:
                    _menuContext.OpenRedditCommand.Execute(null);
                    break;
                case 3:
                    _menuContext.CopyLinkCommand.Execute(null);
                    break;
            }
            _menu?.Dismiss(true);
            _menu = null;
            _menuContext = null;
        }

        private void OnClick(View view)
        {
            var vm = view.Tag.Unwrap<WallpaperItemViewModel>();
            if (vm.IsBlurred)
            {
                vm.IsBlurred = false;
                view.FindViewById<ImageViewAsync>(Resource.Id.WallpapersPageItemImage).Into(vm.Data.FileUrl);
            }
        }

        public override int LayoutResourceId => Resource.Layout.WallpapersPage;

        #region Hamburger
        private Drawer _rightDrawer;

        private void InitDrawer()
        {
            if (_rightDrawer != null)
                return;

            var builder = new DrawerBuilder().WithActivity(Activity);
            builder.WithSliderBackgroundColorRes(ResourceExtension.BrushHamburgerBackgroundRes);
            builder.WithStickyFooterShadow(true);
            builder.WithDisplayBelowStatusBar(true);
            builder.WithDrawerGravity((int)GravityFlags.Right);

            builder.WithStickyHeaderShadow(true);
            builder.WithStickyHeader(Resource.Layout.AnimeListPageDrawerHeader);

            _rightDrawer = builder.Build();

            _rightDrawer.DrawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            _rightDrawer.StickyHeader.SetBackgroundColor(new Color(ResourceExtension.BrushAppBars));
            _rightDrawer.DrawerLayout.AddDrawerListener(new DrawerListener(() =>
            {
                ViewModelLocator.NavMgr.ResetOneTimeOverride();
                OnCloseDrawer();
            }, null));
        }

        private HashSet<WallpaperSources> _startingSet;
        private HashSet<WallpaperSources> _workingSet;
        private void OpenFiltersDrawer()
        {
            var items = new List<IDrawerItem>();
            _startingSet = new HashSet<WallpaperSources>(Settings.EnabledWallpaperSources);
            _workingSet = new HashSet<WallpaperSources>(Settings.EnabledWallpaperSources);
            var listener = new DrawerCheckedChangeListener((drawerItem, b) =>
            {
                if (b)
                    _workingSet.Add((WallpaperSources)drawerItem.Identifier);
                else
                    _workingSet.Remove((WallpaperSources)drawerItem.Identifier);
            });
            foreach (WallpaperSources source in Enum.GetValues(typeof(WallpaperSources)))
            {
                var item = new SwitchDrawerItem();
                item.WithTextColorRes(ResourceExtension.BrushTextRes);
                item.WithChecked(_startingSet.Contains(source));
                item.WithName($"/r/{source}");
                item.WithIdentifier((int)source);
                item.WithOnCheckedChangeListener(listener);

                items.Add(item);
            }         

            _rightDrawer.SetItems(items);

            _rightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Sources";
            _rightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_double_check);


            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(() => _rightDrawer.CloseDrawer()));

            _rightDrawer.OpenDrawer();
        }



        private void OnCloseDrawer()
        {
            if(_workingSet.SetEquals(_startingSet))
                return;

            Settings.EnabledWallpaperSources = _workingSet.ToList();
            ViewModel.RefreshCommand.Execute(null);
        }

        #endregion

        #region Views

        private ListView _wallpapersPageList;
        private FloatingActionButton _wallpapersPageActionButtonFilter;
        private FloatingActionButton _wallpapersPageActionButtonLoadMore;
        private ProgressBar _wallpapersPageProgressSpinner;


        public ListView WallpapersPageList => _wallpapersPageList ?? (_wallpapersPageList = FindViewById<ListView>(Resource.Id.WallpapersPageList));

        public FloatingActionButton WallpapersPageActionButtonFilter => _wallpapersPageActionButtonFilter ?? (_wallpapersPageActionButtonFilter = FindViewById<FloatingActionButton>(Resource.Id.WallpapersPageActionButtonFilter));

        public FloatingActionButton WallpapersPageActionButtonLoadMore => _wallpapersPageActionButtonLoadMore ?? (_wallpapersPageActionButtonLoadMore = FindViewById<FloatingActionButton>(Resource.Id.WallpapersPageActionButtonLoadMore));

        public ProgressBar WallpapersPageProgressSpinner => _wallpapersPageProgressSpinner ?? (_wallpapersPageProgressSpinner = FindViewById<ProgressBar>(Resource.Id.WallpapersPageProgressSpinner));



        #endregion
    }
}