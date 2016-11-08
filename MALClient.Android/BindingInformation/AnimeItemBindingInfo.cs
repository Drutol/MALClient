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
using Com.Daimajia.Swipe;
using FFImageLoading;
using FFImageLoading.Extensions;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.CollectionAdapters;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.Android.Managers;
using MALClient.Models.Enums.Enums;
using MALClient.XShared.ViewModels;
using Org.Zakariya.Flyoutmenu;
using Debug = System.Diagnostics.Debug;

namespace MALClient.Android.BindingInformation
{
    public class AnimeItemBindingInfo : BindingInfo<AnimeItemViewModel>
    {
        private bool _bindingsInitialized;
        private bool _oneTimeBindingsInitialized;
        private bool _moreFlyoutMenuInitialized;
        private bool _swipeLayoutInitialized;

        private static FlyoutMenuView.IAdapter _moreFlyoutAdapter;
        private enum MoreFlyoutButtons
        {
            [EnumUtilities.Description("Copy link")]
            CopyLink,
            [EnumUtilities.Description("Open in browser")]
            OpenInBrowser
        }

        private SwipeLayoutListener _swipeListener;

        public AnimeItemBindingInfo(View container, AnimeItemViewModel viewModel) : base(container, viewModel) {}

        protected override void InitBindings()
        {
            if (_bindingsInitialized)
                return;

            _bindingsInitialized = true;

            ViewModel.AnimeItemDisplayContext = AnimeItemDisplayContext.AirDay;

            var typeView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemType);
            Bindings.Add(Resource.Id.AnimeGridItemType,new List<Binding>());
            Bindings[Resource.Id.AnimeGridItemType].Add(new Binding<string, string>(
                ViewModel,
                () => ViewModel.Type,
                typeView,
                () => typeView.Text));

            var topLeftView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemToLeftInfo);
            Bindings.Add(Resource.Id.AnimeGridItemToLeftInfo, new List<Binding>());
            Bindings[Resource.Id.AnimeGridItemToLeftInfo].Add(new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.TopLeftInfoBind,
                    topLeftView,
                    () => topLeftView.Text));
            Bindings[Resource.Id.AnimeGridItemToLeftInfo].Add(new Binding<string, ViewStates>(
                    ViewModel,
                    () => ViewModel.TopLeftInfoBind,
                    topLeftView,
                    () => topLeftView.Visibility).ConvertSourceToTarget(Converters.IsStringEmptyToVisibility));

            var statusView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemCurrentWatchingStatus);
            Bindings.Add(Resource.Id.AnimeGridItemCurrentWatchingStatus, new List<Binding>());
            Bindings[Resource.Id.AnimeGridItemCurrentWatchingStatus].Add(new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.MyStatusBindShort,
                    statusView,
                    () => statusView.Text));

            var watchedView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemWatchedStatus);
            Bindings.Add(Resource.Id.AnimeGridItemWatchedStatus, new List<Binding>());
            Bindings[Resource.Id.AnimeGridItemWatchedStatus].Add(new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.MyEpisodesBindShort,
                    watchedView,
                    () => watchedView.Text));

            var scoreView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemScore);
            Bindings.Add(Resource.Id.AnimeGridItemScore, new List<Binding>());
            Bindings[Resource.Id.AnimeGridItemScore].Add(new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.MyScoreBindShort,
                    scoreView,
                    () => scoreView.Text));
        }

        private void InitializeMoreFlyout()
        {
            if(_moreFlyoutMenuInitialized)
                return;
            _moreFlyoutMenuInitialized = true;

            if(AnimeGridItemMoreFlyout.Adapter != null)
                return;

            if (_moreFlyoutAdapter == null)
                _moreFlyoutAdapter =
                    new FlyoutMenuView.ArrayAdapter(
                        new[] {MoreFlyoutButtons.CopyLink, MoreFlyoutButtons.OpenInBrowser}.Select(
                            item => new TextFlyoutItem((int) item, item.GetDescription())).ToList());

            AnimeGridItemMoreFlyout.Layout = new FlyoutMenuView.GridLayout(1, FlyoutMenuView.GridLayout.Unspecified);
            AnimeGridItemMoreFlyout.Adapter = _moreFlyoutAdapter;

            AnimeGridItemMoreFlyout.Visibility = ViewStates.Visible;
        }

        #region Swipe


        private void InitializeSwipeLayout()
        {
            if (_swipeLayoutInitialized)
                return;
            _swipeLayoutInitialized = true;

            var swipe = (SwipeLayout)Container;

            //the view has been already set-up
            if (swipe.SwipeListener == null)
            {

                swipe.SetShowMode(SwipeLayout.ShowMode.LayDown);

                swipe.LeftSwipeEnabled = true;
                swipe.RightSwipeEnabled = true;
                swipe.ClickToClose = false;

                _swipeListener = new SwipeLayoutListener();
                swipe.SwipeListener = _swipeListener;

                swipe.AddDrag(SwipeLayout.DragEdge.Right,
                    Container.FindViewById(Resource.Id.AnimeGridItemBackSurfaceAdd));
                swipe.AddDrag(SwipeLayout.DragEdge.Left,
                    Container.FindViewById(Resource.Id.AnimeGridItemBackSurfaceSubtract));
            }
            else
            {
                _swipeListener = swipe.SwipeListener as SwipeLayoutListener;
            }

            _swipeListener.OnOpenAction = SwipeOnOpenEvent;
        }

        private bool _swipeCooldown;

        private async void SwipeOnOpenEvent(SwipeLayout sender)
        {
            Debug.WriteLine($"Attempting swipe for {ViewModel.Title}");
            if (_swipeCooldown)
                return;
            _swipeCooldown = true;
            sender.LeftSwipeEnabled = true;
            sender.RightSwipeEnabled = true;

            var edge = sender.GetDragEdge();
            if (edge == SwipeLayout.DragEdge.Right)
                ViewModel.IncrementWatchedCommand.Execute(null);
            else if (edge == SwipeLayout.DragEdge.Left)
                ViewModel.DecrementWatchedCommand.Execute(null);
            await Task.Delay(500);

            sender.Close();

            sender.LeftSwipeEnabled = true;
            sender.RightSwipeEnabled = true;
            _swipeCooldown = false;
        }
        #endregion


        protected override void InitOneTimeBindings()
        {
            if(_oneTimeBindingsInitialized)
                return;
            _oneTimeBindingsInitialized = true;


            var img = Container.FindViewById<ImageViewAsync>(Resource.Id.AnimeGridItemImage);
            ImageService.Instance.LoadUrl(ViewModel.ImgUrl, TimeSpan.FromDays(7)).FadeAnimation(true,true).Into(img);

            InitializeMoreFlyout();
            InitializeSwipeLayout();

            Container.FindViewById<TextView>(Resource.Id.AnimeGridItemTitle).Text = ViewModel.Title;
        }
        
        protected override void DetachInnerBindings()
        {
            _bindingsInitialized = false;
            _oneTimeBindingsInitialized = false;
            _swipeLayoutInitialized = false;
            _moreFlyoutMenuInitialized = false;
        }

        private ImageButton _animeGridItemMoreButton;
        private FlyoutMenuView _animeGridItemMoreFlyout;

        public ImageButton AnimeGridItemMoreButton => _animeGridItemMoreButton ?? (_animeGridItemMoreButton = Container.FindViewById<ImageButton>(Resource.Id.AnimeGridItemMoreButton));

        public FlyoutMenuView AnimeGridItemMoreFlyout => _animeGridItemMoreFlyout ?? (_animeGridItemMoreFlyout = Container.FindViewById<FlyoutMenuView>(Resource.Id.AnimeGridItemMoreFlyout));

    }
}