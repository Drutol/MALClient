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
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Forums;

namespace MALClient.Android.Fragments.ForumFragments
{
    public class ForumTopicPageFragment : MalFragmentBase
    {
        private readonly ForumsTopicNavigationArgs _args;
        private ForumTopicViewModel ViewModel;

        public ForumTopicPageFragment(ForumsTopicNavigationArgs args)
        {
            _args = args;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.ForumsTopic;
            ViewModel.Init(_args);
        }

        protected override void InitBindings()
        {
            throw new NotImplementedException();
        }

        public override int LayoutResourceId { get; }
    }
}