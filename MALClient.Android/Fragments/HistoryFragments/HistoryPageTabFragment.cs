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
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Fragments.HistoryFragments
{
    public class HistoryPageTabFragment : MalFragmentBase
    {
        private readonly List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>> _data;

        public HistoryPageTabFragment(List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>> data)
        {
            _data = data;
        }

        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {
            (RootView as ListView).InjectFlingAdapter(_data,DataTemplateFull,DataTemplateFling,ContainerTemplate);
        }

        private View ContainerTemplate()
        {
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.HistoryPageTabItem, null);
            return view;
        }

        private void DataTemplateFling(View view, Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>> tuple)
        {
            
        }

        private void DataTemplateFull(View view, Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>> tuple)
        {
            
        }

        public override int LayoutResourceId => Resource.Layout.HistoryPageTab;
    }
}