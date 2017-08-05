using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Resources;
using MALClient.Android.UserControls;
using MALClient.Models.Enums;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.MessagingFragments
{
    public class MessagingDetailsPageFragment : MalFragmentBase
    {
        private readonly MalMessageDetailsNavArgs _args;
        private MalMessageDetailsViewModel ViewModel;

        public MessagingDetailsPageFragment(MalMessageDetailsNavArgs args)
        {
            _args = args;
            if (!_args.BackNavHandled)
            {
                if (_args.WorkMode == MessageDetailsWorkMode.Message)
                    ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageMessanging, null);
                else
                    ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageProfile,
                        new ProfilePageNavigationArgs { TargetUser = ViewModelLocator.ProfilePage.CurrentData.User.Name });
            }
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.MalMessageDetails;
            ViewModel.Init(_args);
        }

        protected override void InitBindings()
        {
            if (ViewModel.NewMessageFieldsVisibility)
            {
                Bindings.Add(
                    this.SetBinding(() => ViewModel.MessageTarget,
                        () => MessagingDetailsPageTargetTextBox.Text,BindingMode.TwoWay));
                Bindings.Add(
                    this.SetBinding(() => ViewModel.MessageSubject,
                        () => MessagingDetailsPageSubjectTextBox.Text,BindingMode.TwoWay));
            }

            Bindings.Add(this.SetBinding(() => ViewModel.NewMessageFieldsVisibility).WhenSourceChanges(() =>
            {
                MessagingDetailsPageTargetTextBox.Visibility =
                    MessagingDetailsPageSubjectTextBox.Visibility =
                        ViewModel.NewMessageFieldsVisibility ? ViewStates.Visible : ViewStates.Gone;
            }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.MessageText,
                    () => MessagingDetailsPageMessageTextBox.Text, BindingMode.TwoWay));

            MessagingDetailsPageSendButton.Click += MessagingDetailsPageSendButtonOnClick;

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingVisibility,
                    () => MessagingDetailsPageLoadingSpinner.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));


            Bindings.Add(this.SetBinding(() => ViewModel.MessageSet).WhenSourceChanges(() =>
            {
                MessagingDetailsPageList.Adapter = ViewModel.MessageSet.GetAdapter(GetMessageTemplateDelegate);
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.IsSendButtonEnabled).WhenSourceChanges(() =>
            {
                
                if (ViewModel.IsSendButtonEnabled)
                {
                    MessagingDetailsPageSendButton.SetBackgroundColor(new Color(ResourceExtension.AccentColourDark));
                    MessagingDetailsPageSendingSpinner.Visibility = ViewStates.Gone;
                }
                else
                {
                    MessagingDetailsPageSendButton.SetBackgroundColor(new Color(ResourceExtension.BrushAnimeItemInnerBackground));
                    MessagingDetailsPageSendingSpinner.Visibility = ViewStates.Visible;
                }
            }));

            var refresh = (RootView as ScrollableSwipeToRefreshLayout);
            refresh.ScrollingView = MessagingDetailsPageList;
            refresh.Refresh += RefreshOnRefresh;
        }

        private void RefreshOnRefresh(object sender, EventArgs eventArgs)
        {
            (RootView as ScrollableSwipeToRefreshLayout).Refreshing = false;

            ViewModel.RefreshData();
        }

        private void MessagingDetailsPageSendButtonOnClick(object sender, EventArgs eventArgs)
        {

            ViewModel.SendMessageCommand.Execute(null);
        }

        private View GetMessageTemplateDelegate(int i, MalMessageModel malMessageModel, View arg3)
        {
            var view = arg3;
            if (view == null || !view.Tag.Unwrap<MalMessageModel>().Sender.Equals(malMessageModel.Sender,StringComparison.InvariantCultureIgnoreCase))
            {
                view =
                    Activity.LayoutInflater.Inflate(
                        malMessageModel.Sender.Equals(Credentials.UserName,StringComparison.InvariantCultureIgnoreCase)
                            ? Resource.Layout.MessagingDetailsPageItemMine
                            : Resource.Layout.MessagingDetailsPageItemOther, null);
            }
            view.Tag = malMessageModel.Wrap();

            view.FindViewById<TextView>(Resource.Id.MessagingDetailsPageItemContent).Text = malMessageModel.Content;
            view.FindViewById<TextView>(Resource.Id.MessagingDetailsPageItemDate).Text = malMessageModel.Date;
            if (malMessageModel.Images.Any())
            {
                var img = view.FindViewById<ImageViewAsync>(Resource.Id.MessagingDetailsPageItemImage);
                img.Visibility = ViewStates.Invisible;
                img.Into(malMessageModel.Images.First());
            }
            else
            {
                view.FindViewById<ImageViewAsync>(Resource.Id.MessagingDetailsPageItemImage).Visibility = ViewStates.Gone;
            }
            
            return view;
        }

        public override int LayoutResourceId => Resource.Layout.MessagingDetailsPage;

        #region Views

        private ListView _messagingDetailsPageList;
        private ProgressBar _messagingDetailsPageLoadingSpinner;
        private EditText _messagingDetailsPageTargetTextBox;
        private EditText _messagingDetailsPageSubjectTextBox;
        private EditText _messagingDetailsPageMessageTextBox;
        private FrameLayout _messagingDetailsPageSendButton;
        private ProgressBar _messagingDetailsPageSendingSpinner;

        public ListView MessagingDetailsPageList => _messagingDetailsPageList ?? (_messagingDetailsPageList = FindViewById<ListView>(Resource.Id.MessagingDetailsPageList));

        public ProgressBar MessagingDetailsPageLoadingSpinner => _messagingDetailsPageLoadingSpinner ?? (_messagingDetailsPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.MessagingDetailsPageLoadingSpinner));

        public EditText MessagingDetailsPageTargetTextBox => _messagingDetailsPageTargetTextBox ?? (_messagingDetailsPageTargetTextBox = FindViewById<EditText>(Resource.Id.MessagingDetailsPageTargetTextBox));

        public EditText MessagingDetailsPageSubjectTextBox => _messagingDetailsPageSubjectTextBox ?? (_messagingDetailsPageSubjectTextBox = FindViewById<EditText>(Resource.Id.MessagingDetailsPageSubjectTextBox));

        public EditText MessagingDetailsPageMessageTextBox => _messagingDetailsPageMessageTextBox ?? (_messagingDetailsPageMessageTextBox = FindViewById<EditText>(Resource.Id.MessagingDetailsPageMessageTextBox));

        public FrameLayout MessagingDetailsPageSendButton => _messagingDetailsPageSendButton ?? (_messagingDetailsPageSendButton = FindViewById<FrameLayout>(Resource.Id.MessagingDetailsPageSendButton));

        public ProgressBar MessagingDetailsPageSendingSpinner => _messagingDetailsPageSendingSpinner ?? (_messagingDetailsPageSendingSpinner = FindViewById<ProgressBar>(Resource.Id.MessagingDetailsPageSendingSpinner));

        #endregion
    }
}