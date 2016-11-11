using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Fragments
{
    public partial class AnimeDetailsPageFragment : MalFragmentBase
    {
        private static AnimeDetailsPageNavigationArgs _navArgs;
        private AnimeDetailsPageViewModel ViewModel;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.AnimeDetails;
            ViewModel.Init(_navArgs);
        }

        protected override void InitBindings()
        {
            Bindings = new Dictionary<int, List<Binding>>();

            Bindings.Add(AnimeDetailsPageScoreButton.Id, new List<Binding>());
            Bindings[AnimeDetailsPageScoreButton.Id].Add(
                this.SetBinding(() => ViewModel.MyScoreBind,
                    () => AnimeDetailsPageScoreButton.Text));
        }

        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPage;

        public static AnimeDetailsPageFragment BuildInstance(object args)
        {
            _navArgs = args as AnimeDetailsPageNavigationArgs;
            return new AnimeDetailsPageFragment();
        }
    }
}