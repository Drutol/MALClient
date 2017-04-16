using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
using Com.Mikepenz.Materialdrawer.Model.Interfaces;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.Models.Models.Notifications;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public class NotificationHubPageFragment : MalFragmentBase
    {
        private NotificationsHubViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.NotificationsHub;
            ViewModel.Init();
            ViewModelLocator.NavMgr.ResetMainBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
        }

        protected override void InitBindings()
        {
            Bindings.Add(this.SetBinding(() => ViewModel.Notifications)
                .WhenSourceChanges(() =>
                {
                    NotificationHubPageList.Adapter = ViewModel.Notifications.GetAdapter(GetTemplateDelegate);
                }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => NotificationHubPageLoadingSpinner.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));

            NotificationHubPageActionButton.Click += NotificationHubPageActionButtonOnClick;

            InitDrawer();
        }

        private void NotificationHubPageActionButtonOnClick(object sender, EventArgs eventArgs)
        {
            OpenFiltersDrawer();
        }

        private View GetTemplateDelegate(int i, MalNotification malNotification, View arg3)
        {
            var view = arg3;
            if (view == null)
            {
                view = Activity.LayoutInflater.Inflate(Resource.Layout.NotificationHubPageItem,null);
                view.Click += ViewOnClick;
                view.FindViewById(Resource.Id.NotificationHubPageItemMarkReadButton).SetOnClickListener(new OnClickListener(OnMarkReadAction));
                view.FindViewById<TextView>(Resource.Id.NotificationHubPageItemIcon).Typeface =
                    FontManager.GetTypeface(Activity, FontManager.TypefacePath);
            }

            view.FindViewById<TextView>(Resource.Id.NotificationHubPageItemIcon).SetText(Converters.MalNotificationTypeToIconConverter(malNotification.Type));
            view.FindViewById<TextView>(Resource.Id.NotificationHubPageItemContent).Text = malNotification.Content;
            view.FindViewById<TextView>(Resource.Id.NotificationHubPageItemDate).Text = malNotification.Date;

            view.Tag = malNotification.Wrap();
            view.FindViewById(Resource.Id.NotificationHubPageItemMarkReadButton).Tag = malNotification.Wrap();
            return view;
        }

        private void ViewOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.NavigateNotificationCommand.Execute((sender as View).Tag.Unwrap<MalNotification>());
        }

        private void OnMarkReadAction(View view)
        {
            ViewModel.MarkAsReadComand.Execute(view.Tag.Unwrap<MalNotification>());
        }


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
            _rightDrawer.DrawerLayout.AddDrawerListener(new DrawerListener(() => ViewModelLocator.NavMgr.ResetOneTimeOverride(),null));
        }

        private void OpenFiltersDrawer()
        {
            if(ViewModel.Loading)
                return;

            var items = new List<IDrawerItem>();
            IDrawerItem activeItem = null;
            foreach (var viewModelNotificationGroup in ViewModel.NotificationGroups)
            {
                var item = HamburgerUtilities.GetBaseSecondaryItem();
                item.WithName(viewModelNotificationGroup.NotificationType.GetDescription());
                item.WithBadge(viewModelNotificationGroup.NotificationsCount.ToString());
                item.WithIdentifier((int)viewModelNotificationGroup.NotificationType);

                if (ViewModel.CurrentNotificationType == viewModelNotificationGroup.NotificationType)
                    activeItem = item;



                items.Add(item);
            }

            var showAllItem = HamburgerUtilities.GetBaseSecondaryItem();
            showAllItem.WithName("All");
            showAllItem.WithIdentifier(3);

            items.Insert(0,showAllItem);

            if (activeItem == null)
                activeItem = showAllItem;



            _rightDrawer.SetItems(items);
            _rightDrawer.SetSelection(activeItem);


            _rightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Filters";
            _rightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_filter);
            _rightDrawer.OnDrawerItemClickListener = new HamburgerItemClickListener((view, i, arg3) =>
            {
                if (arg3.Identifier != 3)
                    ViewModel.CurrentNotificationType = (MalNotificationsTypes) arg3.Identifier;
                else
                    ViewModel.CurrentNotificationType = MalNotificationsTypes.Generic;

                _rightDrawer.OnDrawerItemClickListener = null;
                _rightDrawer.CloseDrawer();
            });


            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CloseDrawer));
            _rightDrawer.OpenDrawer();
        }

        private void CloseDrawer()
        {
            _rightDrawer.CloseDrawer();
        }

        #endregion

        public override int LayoutResourceId => Resource.Layout.NotificationHubPage;

        #region Views

        private ListView _notificationHubPageList;
        private ProgressBar _notificationHubPageLoadingSpinner;
        private FloatingActionButton _notificationHubPageActionButton;

        public ListView NotificationHubPageList => _notificationHubPageList ?? (_notificationHubPageList = FindViewById<ListView>(Resource.Id.NotificationHubPageList));

        public ProgressBar NotificationHubPageLoadingSpinner => _notificationHubPageLoadingSpinner ?? (_notificationHubPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.NotificationHubPageLoadingSpinner));

        public FloatingActionButton NotificationHubPageActionButton => _notificationHubPageActionButton ?? (_notificationHubPageActionButton = FindViewById<FloatingActionButton>(Resource.Id.NotificationHubPageActionButton));

        #endregion
    }
}