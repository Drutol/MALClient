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
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
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
            PersonDetailsPagePivot.Adapter = new PersonDetailsPagerAdapter(ChildFragmentManager);
            PersonDetailsPageTabStrip.SetViewPager(PersonDetailsPagePivot);
            PersonDetailsPageTabStrip.CenterTabs();

            Bindings.Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => PersonDetailsPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(this.SetBinding(() => ViewModel.Data).WhenSourceChanges(() =>
            {
                if (ViewModel.Data == null)
                    return;

                if (!ViewModel.Data.ShowCharacterPairs.Any() && ViewModel.Data.StaffPositions.Any())
                    PersonDetailsPagePivot.SetCurrentItem(1, false);

                PersonDetailsPageDescription.Text = string.Join("\n\n", ViewModel.Data.Details);
                if (string.IsNullOrWhiteSpace(ViewModel.Data.ImgUrl))
                    PersonDetailsPageNoImgNotice.Visibility = ViewStates.Visible;
                else
                {
                    PersonDetailsPageNoImgNotice.Visibility = ViewStates.Gone;
                    PersonDetailsPageImage.Into(ViewModel.Data.ImgUrl, null, img => img.HandleScaling());
                }
                PersonDetailsPageFavButton.BindModel(ViewModel.FavouriteViewModel);
            }));

            PersonDetailsPageLinkButton.SetOnClickListener(new OnClickListener(view => ViewModel.OpenInMalCommand.Execute(null)));
        }

        public override int LayoutResourceId => Resource.Layout.PersonDetailsPage;

        #region Views

        private ImageView _personDetailsPageNoImgNotice;
        private ImageViewAsync _personDetailsPageImage;
        private FavouriteButton _personDetailsPageFavButton;
        private ImageButton _personDetailsPageLinkButton;
        private TextView _personDetailsPageDescription;
        private com.refractored.PagerSlidingTabStrip _personDetailsPageTabStrip;
        private ViewPager _personDetailsPagePivot;
        private RelativeLayout _personDetailsPageLoadingSpinner;

        public ImageView PersonDetailsPageNoImgNotice => _personDetailsPageNoImgNotice ?? (_personDetailsPageNoImgNotice = FindViewById<ImageView>(Resource.Id.PersonDetailsPageNoImgNotice));

        public ImageViewAsync PersonDetailsPageImage => _personDetailsPageImage ?? (_personDetailsPageImage = FindViewById<ImageViewAsync>(Resource.Id.PersonDetailsPageImage));

        public FavouriteButton PersonDetailsPageFavButton => _personDetailsPageFavButton ?? (_personDetailsPageFavButton = FindViewById<FavouriteButton>(Resource.Id.PersonDetailsPageFavButton));

        public ImageButton PersonDetailsPageLinkButton => _personDetailsPageLinkButton ?? (_personDetailsPageLinkButton = FindViewById<ImageButton>(Resource.Id.PersonDetailsPageLinkButton));

        public TextView PersonDetailsPageDescription => _personDetailsPageDescription ?? (_personDetailsPageDescription = FindViewById<TextView>(Resource.Id.PersonDetailsPageDescription));

        public com.refractored.PagerSlidingTabStrip PersonDetailsPageTabStrip => _personDetailsPageTabStrip ?? (_personDetailsPageTabStrip = FindViewById<com.refractored.PagerSlidingTabStrip>(Resource.Id.PersonDetailsPageTabStrip));

        public ViewPager PersonDetailsPagePivot => _personDetailsPagePivot ?? (_personDetailsPagePivot = FindViewById<ViewPager>(Resource.Id.PersonDetailsPagePivot));

        public RelativeLayout PersonDetailsPageLoadingSpinner => _personDetailsPageLoadingSpinner ?? (_personDetailsPageLoadingSpinner = FindViewById<RelativeLayout>(Resource.Id.PersonDetailsPageLoadingSpinner));


        #endregion
    }
}