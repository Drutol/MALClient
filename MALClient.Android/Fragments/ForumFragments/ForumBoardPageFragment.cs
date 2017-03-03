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
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels.Forums;

namespace MALClient.Android.Fragments.ForumFragments
{
    public class ForumBoardPageFragment : MalFragmentBase
    {
        private ForumBoardViewModel ViewModel;
        private readonly ForumsBoardNavigationArgs _args;

        public ForumBoardPageFragment(ForumsBoardNavigationArgs args)
        {
            _args = args;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel.Init(_args);
        }

        protected override void InitBindings()
        {
            throw new NotImplementedException();
        }

        public override int LayoutResourceId { get; }
    }
}