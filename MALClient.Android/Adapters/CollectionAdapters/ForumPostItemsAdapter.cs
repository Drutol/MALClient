using System.Collections.Generic;
using Android.App;
using Android.Views;
using MALClient.Android.BindingInformation;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums.Items;

namespace MALClient.Android.CollectionAdapters
{
    public class ForumPostItemsAdapter : DeeplyObservableCollectionAdapter<ForumTopicMessageEntryViewModel>
    {
        public ForumPostItemsAdapter(Activity context, int layoutResource, IList<ForumTopicMessageEntryViewModel> items) : base(context, layoutResource, items)
        {
        }

        protected override void DetachOldView(ForumTopicMessageEntryViewModel viewModel)
        {
            var hash = viewModel.Data.Id.GetHashCode();
            if (Bindings.ContainsKey(hash))
            {
                Bindings[hash].Detach();
                Bindings.Remove(hash);
            }
        }

        protected override void PrepareView(ForumTopicMessageEntryViewModel item, View view)
        {
            var hash = item.Data.Id.GetHashCode();
            if (!Bindings.ContainsKey(hash))
                Bindings.Add(hash, new ForumPostBidningInfo(view,item,false));
            else
            {
                Bindings[hash].Fling = false;
                Bindings[hash].Container = view;
            }
        }

        protected override void PrepareViewQuickly(ForumTopicMessageEntryViewModel item, View view)
        { 
            var hash = item.Data.Id.GetHashCode();
            if (!Bindings.ContainsKey(hash))
                Bindings.Add(hash, new ForumPostBidningInfo(view,item,true));
            else
            {
                Bindings[hash].Fling = true;
                Bindings[hash].Container = view;
            }
        }

        protected override long GetItemId(ForumTopicMessageEntryViewModel item) => item.Data.Id.GetHashCode();

    }
}