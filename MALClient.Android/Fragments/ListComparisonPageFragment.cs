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
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments
{
    public class ListComparisonPageFragment : MalFragmentBase
    {
        private readonly ListComparisonPageNavigationArgs _args;
        private ListComparisonViewModel ViewModel;

        public ListComparisonPageFragment(ListComparisonPageNavigationArgs args)
        {
            _args = args;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.Comparison;
            ViewModel.NavigatedTo(_args);
        }

        protected override void InitBindings()
        {
            Bindings.Add(this.SetBinding(() => ViewModel.MyData).WhenSourceChanges(() =>
            {
                if(ViewModel.MyData == null)
                    return;

                MyName.Text = ViewModel.MyData.User.Name;
                MyImage.Into(ViewModel.MyData.User.ImgUrl,new CircleTransformation());
                MyMean.Text = ViewModel.MyData.AnimeMean.ToString("N2");
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.OtherData).WhenSourceChanges(() =>
            {
                if (ViewModel.OtherData == null)
                    return;

                OtherName.Text = ViewModel.OtherData.User.Name;
                OtherImage.Into(ViewModel.OtherData.User.ImgUrl,new CircleTransformation());
                OtherMean.Text = ViewModel.OtherData.AnimeMean.ToString("N2");
            }));
        }

        public override int LayoutResourceId => Resource.Layout.ListComparisonPage;

        #region Views

        private ImageViewAsync _myImage;
        private TextView _myName;
        private TextView _myMean;
        private TextView _otherMean;
        private ImageViewAsync _otherImage;
        private TextView _otherName;
        private ListView _comparisonListView;

        public ImageViewAsync MyImage => _myImage ?? (_myImage = FindViewById<ImageViewAsync>(Resource.Id.MyImage));

        public TextView MyName => _myName ?? (_myName = FindViewById<TextView>(Resource.Id.MyName));

        public TextView MyMean => _myMean ?? (_myMean = FindViewById<TextView>(Resource.Id.MyMean));

        public TextView OtherMean => _otherMean ?? (_otherMean = FindViewById<TextView>(Resource.Id.OtherMean));

        public ImageViewAsync OtherImage => _otherImage ?? (_otherImage = FindViewById<ImageViewAsync>(Resource.Id.OtherImage));

        public TextView OtherName => _otherName ?? (_otherName = FindViewById<TextView>(Resource.Id.OtherName));

        public ListView ComparisonListView => _comparisonListView ?? (_comparisonListView = FindViewById<ListView>(Resource.Id.ComparisonListView));



        #endregion
    }
}