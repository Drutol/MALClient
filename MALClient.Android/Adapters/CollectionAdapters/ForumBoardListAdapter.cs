using System.Collections.Generic;
using Android.App;
using Android.Views;
using MALClient.Android.BindingInformation;
using MALClient.XShared.ViewModels.Forums.Items;

namespace MALClient.Android.CollectionAdapters
{
    public class ForumBoardListAdapter : DeeplyObservableCollectionAdapter<ForumBoardEntryViewModel>
    {
        public ForumBoardListAdapter(Activity context, int layoutResource, IList<ForumBoardEntryViewModel> items) : base(context, layoutResource, items)
        {
        }

        protected override void DetachOldView(ForumBoardEntryViewModel viewModel)
        {
            if (Bindings.ContainsKey((int)viewModel.Board))
            {
                Bindings[(int)viewModel.Board].Detach();
                Bindings.Remove((int)viewModel.Board);
            }
        }

        protected override void PrepareView(ForumBoardEntryViewModel item, View view)
        {
            if (!Bindings.ContainsKey((int)item.Board))
                Bindings.Add((int)item.Board, new ForumBoardEntryBindingInformation(view,item,false));
            else
            {
                Bindings[(int)item.Board].Fling = false;
                Bindings[(int)item.Board].Container = view;
            }
        }

        protected override void PrepareViewQuickly(ForumBoardEntryViewModel item, View view)
        {
            if (!Bindings.ContainsKey((int)item.Board))
                Bindings.Add((int)item.Board, new ForumBoardEntryBindingInformation(view, item, true));
            else
            {
                Bindings[(int)item.Board].Fling = true;
                Bindings[(int)item.Board].Container = view;
            }
        }

        protected override long GetItemId(ForumBoardEntryViewModel item) => (long) item.Board;

    }
}