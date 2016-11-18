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
using Com.Orhanobut.Dialogplus;
using Com.Shehabic.Droppy;
using FFImageLoading;
using FFImageLoading.Extensions;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.Adapters.DialogAdapters;
using MALClient.Android.BindingConverters;
using MALClient.Android.CollectionAdapters;
using MALClient.Android.DIalogs;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.Android.Listeners.DialogListeners;
using MALClient.Android.Managers;
using MALClient.Models.Enums.Enums;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;
using Org.Zakariya.Flyoutmenu;
using Debug = System.Diagnostics.Debug;
using Object = Java.Lang.Object;

namespace MALClient.Android.BindingInformation
{
    public class AnimeItemBindingInfo : BindingInfo<AnimeItemViewModel>
    {
        private bool _bindingsInitialized;
        private bool _oneTimeBindingsInitialized;
        private bool _swipeLayoutInitialized;

        private DroppyMenuPopup _menu;
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

            AnimeGridItemMoreButton.Click += AnimeGridItemMoreButtonOnClick;
        }

        #region MoreFlyout

        private void AnimeGridItemMoreButtonOnClick(object sender, EventArgs eventArgs)
        {
            //var menu = new PopupMenu(MainActivity.CurrentContext,sender as View);
            //menu.MenuInflater.Inflate(Resource.Menu.griditem_more_menu,menu.Menu);
            //menu.MenuItemClick += MenuOnMenuItemClick;
            //menu.Show();
            _menu = DroppyFlyoutBuilder.BuildForAnimeGridItem(MainActivity.CurrentContext, AnimeGridItemMoreButton,
                ViewModel,
                MenuOnMenuItemClick);
            _menu.Show();
        }

        private void MenuOnMenuItemClick(AnimeGridItemMoreFlyoutButtons btn)
        {
            switch (btn)
            {
                case AnimeGridItemMoreFlyoutButtons.CopyLink:
                    break;
                case AnimeGridItemMoreFlyoutButtons.OpenInBrowser:
                    break;
                case AnimeGridItemMoreFlyoutButtons.SetStatus:
                    ShowStatusDialog();
                    break;
                case AnimeGridItemMoreFlyoutButtons.SetRating:
                    ShowRatingDialog();
                    break;
                case AnimeGridItemMoreFlyoutButtons.SetWatched:
                    ShowWatchedDialog();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(btn), btn, null);
            }
            _menu.Dismiss(true);
        }



        #endregion

        #region Dialogs

        private void ShowStatusDialog()
        {
            AnimeUpdateDialogBuilder.BuildStatusDialog(ViewModel,ViewModel.ParentAbstraction.RepresentsAnime);        
        }
        private void ShowWatchedDialog()
        {
            AnimeUpdateDialogBuilder.BuildWatchedDialog(ViewModel);
        }
        private void ShowRatingDialog()
        {
            AnimeUpdateDialogBuilder.BuildScoreDialog(ViewModel);
        }
        #endregion

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

            //InitializeMoreFlyout();
            InitializeSwipeLayout();

            Container.FindViewById<TextView>(Resource.Id.AnimeGridItemTitle).Text = ViewModel.Title;
        }
        
        protected override void DetachInnerBindings()
        {
            if(_animeGridItemMoreButton != null)
                AnimeGridItemMoreButton.Click -= AnimeGridItemMoreButtonOnClick;

            _bindingsInitialized = false;
            _oneTimeBindingsInitialized = false;
            _swipeLayoutInitialized = false;
        }

        private ImageButton _animeGridItemMoreButton;


        public ImageButton AnimeGridItemMoreButton => _animeGridItemMoreButton ?? (_animeGridItemMoreButton = Container.FindViewById<ImageButton>(Resource.Id.AnimeGridItemMoreButton));

        

    }
}