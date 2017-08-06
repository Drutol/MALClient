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
using FFImageLoading.Views;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Clubs;

namespace MALClient.Android.Fragments.Clubs
{
    public abstract class ClubIndexTabFragmentBase : MalFragmentBase
    {
        protected readonly ClubIndexViewModel ViewModel = ViewModelLocator.ClubIndex;

        protected View ContainerTemplate(int i)
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.ClubsIndexItem, null);
            view.Click += ViewOnClick;
            return view;
        }

        private void ViewOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.NavigateDetailsCommand.Execute((sender as View).Tag.Unwrap<MalClubEntry>());
        }

        protected void DataTemplateBasic(View view, int i, MalClubEntry arg3, ClubItemViewHolder arg4)
        {

        }

        protected void DataTemplateFling(View view, int i, MalClubEntry arg3, ClubItemViewHolder arg4)
        {
  
        }

        protected void DataTemplateFull(View view, int i, MalClubEntry arg3, ClubItemViewHolder arg4)
        {

        }

        protected ClubItemViewHolder ViewHolderFactory(View view)
        {
            return new ClubItemViewHolder(view);
        }


        protected class ClubItemViewHolder
        {
            private readonly View _view;

            public ClubItemViewHolder(View view)
            {
                _view = view;
            }
            private ImageViewAsync _image;
            private TextView _name;
            private TextView _description;
            private TextView _lastCommentDate;
            private LinearLayout _lastCommentSection;
            private TextView _lastPostDate;
            private LinearLayout _lastPostSection;
            private TextView _members;

            public ImageViewAsync Image => _image ?? (_image = _view.FindViewById<ImageViewAsync>(Resource.Id.Image));

            public TextView Name => _name ?? (_name = _view.FindViewById<TextView>(Resource.Id.Name));

            public TextView Description => _description ?? (_description = _view.FindViewById<TextView>(Resource.Id.Description));

            public TextView LastCommentDate => _lastCommentDate ?? (_lastCommentDate = _view.FindViewById<TextView>(Resource.Id.LastCommentDate));

            public LinearLayout LastCommentSection => _lastCommentSection ?? (_lastCommentSection = _view.FindViewById<LinearLayout>(Resource.Id.LastCommentSection));

            public TextView LastPostDate => _lastPostDate ?? (_lastPostDate = _view.FindViewById<TextView>(Resource.Id.LastPostDate));

            public LinearLayout LastPostSection => _lastPostSection ?? (_lastPostSection = _view.FindViewById<LinearLayout>(Resource.Id.LastPostSection));

            public TextView Members => _members ?? (_members = _view.FindViewById<TextView>(Resource.Id.Members));

        }
    }
}