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
using Com.Mikepenz.Iconics;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.CollectionAdapters;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums;
using MALClient.XShared.ViewModels.Forums.Items;

namespace MALClient.Android.Fragments.ForumFragments.Tabs
{
    class ForumIndexPageFragmentBoardsTabFragment : MalFragmentBase
    {
        private ForumIndexViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.ForumsIndex;
        }

        protected override void InitBindings()
        {
            ForumIndexPageBoardsTabListView.Adapter = ViewModel.Boards.GetAdapter(GetSectionTemplateDelegate);
        }

        private View GetSectionTemplateDelegate(int i, ForumIndexViewModel.ForumBoardEntryGroup forumBoardEntryGroup, View arg3)
        {
            var view = arg3;
            if (view == null)
            {
                view = Activity.LayoutInflater.Inflate(Resource.Layout.ForumsIndexSectionItem, null);
            }

            view.FindViewById<TextView>(Resource.Id.ForumsIndexSectionItemListHeader).Text = forumBoardEntryGroup.Group;
            view.FindViewById<LinearLayout>(Resource.Id.ForumsIndexSectionItemList).SetAdapter(new ForumBoardListAdapter(Activity,
                Resource.Layout.ForumIndexPageBoardItem,forumBoardEntryGroup.Items));

            return view;
        }

        public override int LayoutResourceId => Resource.Layout.ForumIndexPageBoardsTab;

        #region Views

        private ListView _forumIndexPageBoardsTabListView;

        public ListView ForumIndexPageBoardsTabListView => _forumIndexPageBoardsTabListView ?? (_forumIndexPageBoardsTabListView = FindViewById<ListView>(Resource.Id.ForumIndexPageBoardsTabListView));

        #endregion
    }
}