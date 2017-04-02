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
using MALClient.Android.UserControls.ForumItems;
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
            ForumIndexPageBoardsTabListView.InjectFlingAdapter(ViewModel.Boards.SelectMany(group => group.Items).ToList(),DataTemplateFull, DataTemplateFling, ContainerTemplate);
        }

        private View ContainerTemplate(int i)
        {
            return new ForumIndexItem(Context,ViewModel);
        }

        private void DataTemplateFling(View view, int i, ForumBoardEntryViewModel arg3)
        {
            ((ForumIndexItem)view).BindModel(arg3,true);
        }

        private void DataTemplateFull(View view, int i, ForumBoardEntryViewModel arg3)
        {
            ((ForumIndexItem)view).BindModel(arg3, false);
        }


        public override int LayoutResourceId => Resource.Layout.ForumIndexPageBoardsTab;

        #region Views

        private ListView _forumIndexPageBoardsTabListView;

        public ListView ForumIndexPageBoardsTabListView => _forumIndexPageBoardsTabListView ?? (_forumIndexPageBoardsTabListView = FindViewById<ListView>(Resource.Id.ForumIndexPageBoardsTabListView));

        #endregion
    }
}