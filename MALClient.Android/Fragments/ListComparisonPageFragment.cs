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
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public class ListComparisonPageFragment : MalFragmentBase
    {
        private readonly ListComparisonPageNavigationArgs _args;
        private ListComparisonViewModel _viewModel;

        public ListComparisonPageFragment(ListComparisonPageNavigationArgs args)
        {
            _args = args;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            _viewModel = ViewModelLocator.Comparison;
            _viewModel.NavigatedTo(_args);
        }

        protected override void InitBindings()
        {

        }

        public override int LayoutResourceId => Resource.Layout.ListComparisonPage;
    }
}