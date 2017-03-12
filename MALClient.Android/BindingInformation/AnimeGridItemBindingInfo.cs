using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Views;
using Android.Widget;
using Com.Daimajia.Swipe;
using Com.Shehabic.Droppy;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.DIalogs;
using MALClient.Android.Flyouts;
using MALClient.Android.Listeners;
using MALClient.XShared.ViewModels;
using Debug = System.Diagnostics.Debug;

namespace MALClient.Android.BindingInformation
{
    public class AnimeGridItemBindingInfo : BindingInfo<AnimeItemViewModel>
    {
        private bool _bindingsInitialized;
        private bool _oneTimeBindingsInitialized;
        private bool _swipeLayoutInitialized;
        private bool _clickHandlerAdded;

        private DroppyMenuPopup _menu;
        private DroppyMenuPopup _tagsMenu;
        private SwipeLayoutListener _swipeListener;

        public bool AllowSwipeInGivenContext { get; set; }

        public Action<AnimeItemViewModel> OnItemClickAction { get; set; }

        public AnimeGridItemBindingInfo(View container, AnimeItemViewModel viewModel,bool fling, bool allowSwipe = true)
            : base(container, viewModel,fling)
        {
            AllowSwipeInGivenContext = allowSwipe;
            PrepareContainer();
        }

        protected override void InitBindings()
        {
            if (_bindingsInitialized || Fling)
                return;

            _bindingsInitialized = true;

            

            var typeView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemType);
            
            Bindings.Add(new Binding<string, string>(
                ViewModel,
                () => ViewModel.Type,
                typeView,
                () => typeView.Text));
            Bindings.Add(new Binding<string, ViewStates>(
                ViewModel,
                () => ViewModel.Type,
                typeView,
                () => typeView.Visibility).ConvertSourceToTarget(s => string.IsNullOrEmpty(s) ? ViewStates.Gone : ViewStates.Visible));

            var topLeftView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemToLeftInfo);
            
            Bindings.Add(new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.TopLeftInfoBind,
                    topLeftView,
                    () => topLeftView.Text));
            Bindings.Add(new Binding<string, ViewStates>(
                    ViewModel,
                    () => ViewModel.TopLeftInfoBind,
                    topLeftView,
                    () => topLeftView.Visibility).ConvertSourceToTarget(Converters.IsStringEmptyToVisibility));

            var statusView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemCurrentWatchingStatus);
            
            Bindings.Add(new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.MyStatusBindShort,
                    statusView,
                    () => statusView.Text));

            var watchedView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemWatchedStatus);
            
            Bindings.Add(new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.MyEpisodesBindShort,
                    watchedView,
                    () => watchedView.Text));
            Container.FindViewById(Resource.Id.AnimeGridItemWatchedStatusButton).SetOnClickListener(new OnClickListener(view => ShowWatchedDialog()));

            var scoreView = Container.FindViewById<TextView>(Resource.Id.AnimeGridItemScore);
            
            Bindings.Add(new Binding<string, string>(
                    ViewModel,
                    () => ViewModel.MyScoreBindShort,
                    scoreView,
                    () => scoreView.Text));

            var topRight = Container.FindViewById<LinearLayout>(Resource.Id.AnimeGridItemTopRightInfo);
            
            Bindings.Add(new Binding<bool, ViewStates>(
                ViewModel,
                () => ViewModel.Auth,
                topRight,
                () => topRight.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            var addButon = Container.FindViewById<FrameLayout>(Resource.Id.AnimeGridItemAddToListButton);
            
            Bindings.Add(new Binding<bool, ViewStates>(
                ViewModel,
                () => ViewModel.AddToListVisibility,
                addButon,
                () => addButon.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
            addButon.SetOnClickListener(new OnClickListener(view => ViewModel.AddAnimeCommand.Execute(null)));

            var tagButton = Container.FindViewById<FrameLayout>(Resource.Id.AnimeGridItemTagsButton);
            
            Bindings.Add(new Binding<bool, ViewStates>(
                ViewModel,
                () => ViewModel.TagsControlVisibility,
                tagButton,
                () => tagButton.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
            tagButton.SetOnClickListener(new OnClickListener(OnTagsButtonClick));
        }

        private void OnTagsButtonClick(View view)
        {
            _tagsMenu = AnimeItemFlyoutBuilder.BuildForAnimeItemTags(Container.Context, view, ViewModel,
                () => _tagsMenu.Dismiss(true));
            _tagsMenu.Show();
        }

        private void ContainerOnClick()
        {
            if(_swipeListener?.IsSwiping ?? false)
                return;
            if (OnItemClickAction != null)
                OnItemClickAction.Invoke(ViewModel);
            else
                ViewModel.NavigateDetailsCommand.Execute(null);
        }

        #region MoreFlyout

        private void AnimeGridItemMoreButtonOnClick()
        {
            _menu = AnimeItemFlyoutBuilder.BuildForAnimeItem(MainActivity.CurrentContext, AnimeGridItemMoreButton,
                ViewModel,
                MenuOnMenuItemClick);
            _menu.Show();
        }

        private void MenuOnMenuItemClick(AnimeGridItemMoreFlyoutButtons btn)
        {
            switch (btn)
            {
                case AnimeGridItemMoreFlyoutButtons.CopyLink:
                    ViewModel.CopyLinkToClipboardCommand.Execute(null);
                    break;
                case AnimeGridItemMoreFlyoutButtons.OpenInBrowser:
                    ViewModel.OpenInMALCommand.Execute(null);
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
            swipe.SwipeEnabled = true;
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

        private void DisableSwipe()
        {
            var swipe = (SwipeLayout)Container;
            swipe.SwipeEnabled = false;
            swipe.LeftSwipeEnabled = false;
            swipe.RightSwipeEnabled = false;
        }

        private bool _swipeCooldown;

        private async void SwipeOnOpenEvent(SwipeLayout sender)
        {
            Debug.WriteLine($"Attempting swipe for {ViewModel.Title}");
            if (_swipeCooldown)
                return;
            _swipeCooldown = true;

            var edge = sender.GetDragEdge();
            if (edge == SwipeLayout.DragEdge.Right)
                ViewModel.IncrementWatchedCommand.Execute(null);
            else if (edge == SwipeLayout.DragEdge.Left)
                ViewModel.DecrementWatchedCommand.Execute(null);
            await Task.Delay(500);

            sender.Close();

            _swipeCooldown = false;
        }
        #endregion


        protected override void InitOneTimeBindings()
        {
            if(_oneTimeBindingsInitialized)
                return;
            _oneTimeBindingsInitialized = true;
          
            if (!Fling && (int)Container.Tag != ViewModel.Id)
            {
                ViewModel.AnimeItemDisplayContext = ViewModelLocator.AnimeList.AnimeItemsDisplayContext;


                var img = Container.FindViewById<ImageViewAsync>(Resource.Id.AnimeGridItemImage);
                img.AnimeInto(ViewModel.ImgUrl);
                Container.FindViewById(Resource.Id.AnimeGridItemImgPlaceholder).Visibility = ViewStates.Gone;


                Container.FindViewById(Resource.Id.AnimeGridItemFavouriteIndicator).Visibility =
                    ViewModel.IsFavouriteVisibility ? ViewStates.Visible : ViewStates.Gone;

                if (AllowSwipeInGivenContext && ViewModel.Auth)
                    InitializeSwipeLayout();
                else
                    DisableSwipe();

                Container.SetOnClickListener(new OnClickListener(view => ContainerOnClick()));
                AnimeGridItemMoreButton.SetOnClickListener(new OnClickListener(view => AnimeGridItemMoreButtonOnClick()));

                Container.Tag = ViewModel.Id;
            }
            else if(Fling)
            {
                Container.FindViewById(Resource.Id.AnimeGridItemImage).Visibility = ViewStates.Invisible;
                Container.FindViewById(Resource.Id.AnimeGridItemImgPlaceholder).Visibility = ViewStates.Visible;
            }
          
            Container.FindViewById<TextView>(Resource.Id.AnimeGridItemTitle).Text = ViewModel.Title;
        }
        
        protected override void DetachInnerBindings()
        {
            if (Bindings.Any())
            {
                AnimeGridItemMoreButton.SetOnClickListener(null);
                Container.SetOnClickListener(null);
            }

            _bindingsInitialized = false;
            _oneTimeBindingsInitialized = false;
            _swipeLayoutInitialized = false;
        }



        public ImageButton AnimeGridItemMoreButton =>  Container.FindViewById<ImageButton>(Resource.Id.AnimeGridItemMoreButton);
    }
}