using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Orhanobut.Dialogplus;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.Listeners;
using MALClient.Android.Listeners.DialogListeners;
using MALClient.Models.Enums;
using MALClient.Models.Models;
using MALClient.XShared.Interfaces;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Dialogs
{
    public class PinnedUsersDialog
    {
        private readonly IHandyDataStorageModule<MalUser> _dataStorageModule;
        private readonly DialogPlus _tagsDialog;
        private readonly List<Binding> _tagsDialogBindings;

        public PinnedUsersDialog(IHandyDataStorageModule<MalUser> dataStorageModule)
        {
            _dataStorageModule = dataStorageModule;
            _tagsDialogBindings = new List<Binding>();
            var dialogBuilder = DialogPlus.NewDialog(MainActivity.CurrentContext);
            dialogBuilder.SetContentHolder(new ViewHolder(Resource.Layout.PinnedUsersDialog));
            dialogBuilder.SetOnDismissListener(new DialogDismissedListener(CleanupDialog));
            dialogBuilder.SetOnBackPressListener(new OnBackPressListener(CleanupDialog));
            dialogBuilder.SetGravity((int)GravityFlags.Top);
            _tagsDialog = dialogBuilder.Create();
            var view = _tagsDialog.HolderView;

            var list = view.FindViewById<ListView>(Resource.Id.List);
            list.EmptyView = view.FindViewById(Resource.Id.EmptyNotice);
            list.Adapter = dataStorageModule.StoredItems.GetAdapter(GetTagItem);

            _tagsDialog.Show();
            MainActivity.CurrentContext.DialogToCollapseOnBack = _tagsDialog;
        }

        private View GetTagItem(int i, MalUser s, View convertView)
        {
            var view = convertView;
            if (view == null)
            {
                view = MainActivity.CurrentContext.LayoutInflater.Inflate(Resource.Layout.PinnedUsersDialogItem, null);
                view.FindViewById<ImageButton>(Resource.Id.DeleteButton).Click +=
                    (sender, args) =>
                    {
                        _dataStorageModule.StoredItems.Remove((sender as View).Tag.Unwrap<MalUser>());
                        if (ViewModelLocator.GeneralMain.CurrentMainPage == PageIndex.PageProfile)
                            ViewModelLocator.ProfilePage.RaisePropertyChanged("IsPinned");
                    };

                view.SetOnClickListener(new OnClickListener(v =>
                {
                    ViewModelLocator.GeneralMain.Navigate(
                        PageIndex.PageProfile,
                        new ProfilePageNavigationArgs {TargetUser = v.Tag.Unwrap<MalUser>().Name});

                    CleanupDialog();
                }));

            }
            var tag = s.Wrap();
            view.Tag = tag;
            view.FindViewById<ImageButton>(Resource.Id.DeleteButton).Tag = tag;
            view.FindViewById<TextView>(Resource.Id.Name).Text = s.Name;
            view.FindViewById<ImageViewAsync>(Resource.Id.ProfileImg).Into(s.ImgUrl,new CircleTransformation());
            return view;
        }

        private void CleanupDialog()
        {
            AndroidUtilities.HideKeyboard();
            _tagsDialogBindings?.ForEach(binding => binding.Detach());
            _tagsDialog?.Dismiss();
            MainActivity.CurrentContext.DialogToCollapseOnBack = null;
        }
    }
}