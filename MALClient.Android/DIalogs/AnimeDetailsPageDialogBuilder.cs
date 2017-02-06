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
using FFImageLoading;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.Listeners.DialogListeners;
using MALClient.Android.Resources;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.Models.Models.Library;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.DIalogs
{
    public class AnimeDetailsPageDialogBuilder
    {
        private static DialogPlus _videosDialog;
        private static List<Binding> _videosDialogBindings;
        public static void BuildPromotionalVideoDialog(AnimeDetailsPageViewModel viewModel)
        {
            viewModel.LoadVideosCommand.Execute(null);
            var dialogBuilder = DialogPlus.NewDialog(MainActivity.CurrentContext);
            dialogBuilder.SetGravity((int) GravityFlags.Center);
            dialogBuilder.SetContentHolder(new ViewHolder(Resource.Layout.AnimeDetailsPageVideosDialog));
            dialogBuilder.SetContentBackgroundResource(ResourceExtension.BrushFlyoutBackgroundRes);
            dialogBuilder.SetOnDismissListener(
                new DialogDismissedListener(() => ViewModelLocator.NavMgr.ResetOneTimeOverride()));
            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CleanupVideosDialog));
            _videosDialog = dialogBuilder.Create();
            var dialogView = _videosDialog.HolderView;

            var spinner = dialogView.FindViewById(Resource.Id.AnimeDetailsPageVideosDialogProgressBar);
            _videosDialogBindings = new List<Binding>
            {
                new Binding<bool, ViewStates>(
                    viewModel,
                    () => viewModel.LoadingVideosVisibility,
                    spinner,
                    () => spinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility)
            };

            dialogView.FindViewById<ListView>(Resource.Id.AnimeDetailsPageVideosDialogList).Adapter = viewModel
                .AvailableVideos.GetAdapter(
                    (i, data, convertView) =>
                    {
                        var view = convertView;
                        if (view == null)
                        {
                            view =
                                MainActivity.CurrentContext.LayoutInflater.Inflate(
                                    Resource.Layout.AnimeDetailsPageVideosDialogItem, null);
                            view.SetOnClickListener(new OnClickListener(VideoItemOnClick));
                            
                        }

                        ImageService.Instance.LoadUrl(data.Thumb)
                            .FadeAnimation(false)
                            .Success(view.FindViewById(Resource.Id.AnimeDetailsPageVideosDialogItemImage).AnimateFadeIn)
                            .Into(view.FindViewById<ImageViewAsync>(Resource.Id.AnimeDetailsPageVideosDialogItemImage));

                        view.FindViewById<TextView>(Resource.Id.AnimeDetailsPageVideosDialogItemText).Text = data.Name;

                        view.Tag = data.Wrap();

                        return view;
                    });

            _videosDialog.Show();
        }

        private static void VideoItemOnClick(View view)
        {
            ViewModelLocator.AnimeDetails.OpenVideoCommand.Execute(view.Tag.Unwrap<AnimeVideoData>());
        }

        private static void CleanupVideosDialog()
        {
            ViewModelLocator.NavMgr.ResetOneTimeMainOverride();
            _videosDialogBindings.ForEach(binding => binding.Detach());
            _videosDialogBindings = new List<Binding>();
            _videosDialog?.Dismiss();
            _videosDialog = null;
        }
    }
}