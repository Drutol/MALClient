using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Com.Astuetz;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.PagerAdapters;
using MALClient.Android.UserControls;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;

namespace MALClient.Android.Fragments.DetailsFragments
{
    public class PersonDetailsPageFragment : MalFragmentBase
    {
        private readonly StaffDetailsNaviagtionArgs _args;
        private StaffDetailsViewModel ViewModel;

        public PersonDetailsPageFragment(StaffDetailsNaviagtionArgs args)
        {
            _args = args;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.StaffDetails;
            ViewModel.Init(_args);
        }

        protected override void InitBindings()
        {
            PersonDetailsPagePivot.Adapter = new PersonDetailsPagerAdapter(FragmentManager);
            PersonDetailsPageTabStrip.SetViewPager(PersonDetailsPagePivot);
            PersonDetailsPageTabStrip.CenterTabs();

            Bindings.Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => PersonDetailsPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(this.SetBinding(() => ViewModel.Data).WhenSourceChanges(() =>
            {
                if(ViewModel.Data == null)
                    return;

                PersonDetailsPageDescription.Text = string.Join("\n\n", ViewModel.Data.Details);
                PersonDetailsPageImage.Into(ViewModel.Data.ImgUrl,null,img => img.HandleScaling());
                PersonDetailsPageFavButton.BindModel(ViewModel.FavouriteViewModel);
            }));
        }

        public override int LayoutResourceId => Resource.Layout.PersonDetailsPage;

        #region Views

        private ImageViewAsync _personDetailsPageImage;
        private FavouriteButton _personDetailsPageFavButton;
        private ImageButton _personDetailsPageLinkButton;
        private TextView _personDetailsPageDescription;
        private PagerSlidingTabStrip _personDetailsPageTabStrip;
        private ViewPager _personDetailsPagePivot;
        private RelativeLayout _personDetailsPageLoadingSpinner;

        public ImageViewAsync PersonDetailsPageImage => _personDetailsPageImage ?? (_personDetailsPageImage = FindViewById<ImageViewAsync>(Resource.Id.PersonDetailsPageImage));

        public FavouriteButton PersonDetailsPageFavButton => _personDetailsPageFavButton ?? (_personDetailsPageFavButton = FindViewById<FavouriteButton>(Resource.Id.PersonDetailsPageFavButton));

        public ImageButton PersonDetailsPageLinkButton => _personDetailsPageLinkButton ?? (_personDetailsPageLinkButton = FindViewById<ImageButton>(Resource.Id.PersonDetailsPageLinkButton));

        public TextView PersonDetailsPageDescription => _personDetailsPageDescription ?? (_personDetailsPageDescription = FindViewById<TextView>(Resource.Id.PersonDetailsPageDescription));

        public PagerSlidingTabStrip PersonDetailsPageTabStrip => _personDetailsPageTabStrip ?? (_personDetailsPageTabStrip = FindViewById<PagerSlidingTabStrip>(Resource.Id.PersonDetailsPageTabStrip));

        public ViewPager PersonDetailsPagePivot => _personDetailsPagePivot ?? (_personDetailsPagePivot = FindViewById<ViewPager>(Resource.Id.PersonDetailsPagePivot));

        public RelativeLayout PersonDetailsPageLoadingSpinner => _personDetailsPageLoadingSpinner ?? (_personDetailsPageLoadingSpinner = FindViewById<RelativeLayout>(Resource.Id.PersonDetailsPageLoadingSpinner));



        #endregion
    }
}