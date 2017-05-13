using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Oguzdev.Circularfloatingactionmenu.Library;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Resources;
using MALClient.Android.UserControls;
using MALClient.Models.Enums;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;

namespace MALClient.Android.Fragments.MessagingFragments
{
    public class MessagingPageFragment : MalFragmentBase
    {
        private MalMessagingViewModel ViewModel;
        private FloatingActionMenu _actionMenu;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModelLocator.NavMgr.DeregisterBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
            ViewModel = ViewModelLocator.MalMessaging;
            ViewModel.Init();


        }

        private void CurrentContextOnHamburgerOpened(object sender, EventArgs eventArgs)
        {
            _actionMenu?.Close(true);
        }

        public override void OnResume()
        {
            MainActivity.CurrentContext.HamburgerOpened += CurrentContextOnHamburgerOpened;
            base.OnResume();
        }

        protected override void Cleanup()
        {
            MainActivity.CurrentContext.HamburgerOpened -= CurrentContextOnHamburgerOpened;
            _actionMenu.Close(true);
            base.Cleanup();
        }

        protected override void InitBindings()
        {
            _actionMenu?.Close(true);
            var padding = DimensionsHelper.DpToPx(10);
            var param = new ViewGroup.LayoutParams(DimensionsHelper.DpToPx(45), DimensionsHelper.DpToPx(45));
            var builder = new FloatingActionMenu.Builder(Activity)
                .AddSubActionView(BuildFabActionButton(param, Resource.Drawable.icon_message_new))
                .AddSubActionView(BuildFabActionButton(param, Resource.Drawable.icon_message_sent));
            builder.SetRadius(DimensionsHelper.DpToPx(75));
            _actionMenu = builder.AttachTo(MessagingPageActionButton).Build();

            Bindings.Add(this.SetBinding(() => ViewModel.DisplaySentMessages).WhenSourceChanges(() =>
            {
                MessagingPageList.Adapter = ViewModel.DisplaySentMessages
                    ? ViewModel.Outbox.GetAdapter(GetMessageTemplateDelegate)
                    : ViewModel.Inbox.GetAdapter(GetMessageTemplateDelegate);
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.LoadingVisibility).WhenSourceChanges(() =>
            {
                if (ViewModel.LoadingVisibility)
                {
                    MessagingPageList.Adapter = null;
                    return;
                }
                MessagingPageList.Adapter = ViewModel.DisplaySentMessages
                    ? ViewModel.Outbox.GetAdapter(GetMessageTemplateDelegate)
                    : ViewModel.Inbox.GetAdapter(GetMessageTemplateDelegate);
            }));




            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingVisibility,
                    () => MessagingPageProgressSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            var scrollToRefresh = RootView as ScrollableSwipeToRefreshLayout;
            scrollToRefresh.ScrollingView = MessagingPageList;
            scrollToRefresh.Refresh += ScrollToRefreshOnRefresh;
        }

        private void ScrollToRefreshOnRefresh(object sender, EventArgs e)
        {
            (RootView as ScrollableSwipeToRefreshLayout).Refreshing = false;
            ViewModelLocator.GeneralMain.RefreshDataCommand.Execute(null);
        }

        private View GetMessageTemplateDelegate(int i, MalMessageModel malMessageModel, View convertView)
        {
            var view = convertView;
            if (view == null)
            {
                view = Activity.LayoutInflater.Inflate(Resource.Layout.MessagingPageItem, null);
                view.Click += MessageOnClick;
            }

            view.Tag = malMessageModel.Wrap();

            view.FindViewById<TextView>(Resource.Id.MessagingPageItemSender).Text = malMessageModel.Sender;
            view.FindViewById<TextView>(Resource.Id.MessagingPageItemDate).Text = malMessageModel.Date;
            view.FindViewById<TextView>(Resource.Id.MessagingPageItemContent).Text = malMessageModel.Content;
            view.FindViewById<TextView>(Resource.Id.MessagingPageItemTitle).Text = malMessageModel.Subject;
            var stateImg = view.FindViewById<ImageView>(Resource.Id.MessagingPageItemStateSymbol);
            if (malMessageModel.IsRead)
            {
                stateImg.SetImageResource(Resource.Drawable.icon_message_read);
                stateImg.ImageTintList = ColorStateList.ValueOf(new Color(ResourceExtension.BrushText));
            }
            else
            {
                if (malMessageModel.IsMine)
                {
                    stateImg.SetImageResource(Resource.Drawable.icon_message_sent);
                    stateImg.ImageTintList = ColorStateList.ValueOf(new Color(ResourceExtension.BrushText));
                }
                else
                {
                    stateImg.SetImageResource(Resource.Drawable.icon_message_alert);
                    stateImg.ImageTintList = ColorStateList.ValueOf(new Color(ResourceExtension.AccentColourDark));
                }

            }

            return view;
        }

        private bool _canNavigate = true;
        private async void MessageOnClick(object sender, EventArgs eventArgs)
        {
            if(!_canNavigate)
                return;
            _canNavigate = false;
            ViewModel.NavigateMessageCommand.Execute((sender as View).Tag.Unwrap<MalMessageModel>());
            await Task.Delay(200);
            _canNavigate = true;
        }

        private View BuildFabActionButton(ViewGroup.LayoutParams param, int icon)
        {
            var b1 = new FloatingActionButton(Activity)
            {
                LayoutParameters = param,
                Clickable = true,
                Focusable = true
            };
            b1.Size = FloatingActionButton.SizeMini;
            b1.SetScaleType(ImageView.ScaleType.Center);
            b1.SetImageResource(icon);
            b1.ImageTintList = ColorStateList.ValueOf(Color.White);
            b1.BackgroundTintList = ColorStateList.ValueOf(new Color(ResourceExtension.AccentColourContrast));
            b1.Tag = icon;
            b1.Click += OnFloatingActionButtonOptionClick;
            return b1;
        }

        private void OnFloatingActionButtonOptionClick(object sender, EventArgs eventArgs)
        {
            _actionMenu.Close(true);
            switch ((int)(sender as View).Tag)
            {
                case Resource.Drawable.icon_message_new:
                    ViewModel.ComposeNewCommand.Execute(null);
                    break;
                case Resource.Drawable.icon_message_sent:
                    ViewModel.DisplaySentMessages = !ViewModel.DisplaySentMessages;
                    ViewModelLocator.GeneralMain.CurrentStatus = ViewModel.DisplaySentMessages
                        ? $"{Credentials.UserName} - Sent Messages"
                        : $"{Credentials.UserName} - Messages";
                    break;
            }
        }

        public override int LayoutResourceId => Resource.Layout.MessagingPage;


        #region Views

        private ListView _messagingPageList;
        private FloatingActionButton _messagingPageActionButton;
        private ProgressBar _messagingPageProgressSpinner;

        public ListView MessagingPageList => _messagingPageList ?? (_messagingPageList = FindViewById<ListView>(Resource.Id.MessagingPageList));

        public FloatingActionButton MessagingPageActionButton => _messagingPageActionButton ?? (_messagingPageActionButton = FindViewById<FloatingActionButton>(Resource.Id.MessagingPageActionButton));

        public ProgressBar MessagingPageProgressSpinner => _messagingPageProgressSpinner ?? (_messagingPageProgressSpinner = FindViewById<ProgressBar>(Resource.Id.MessagingPageProgressSpinner));


        #endregion
    }
}