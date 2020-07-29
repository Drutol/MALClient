using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AoLibs.Adapters.Android.Recycler;
using AoLibs.Adapters.Core;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.UserControls;
using MALClient.Models.Models.Favourites;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Details;
using Orientation = Android.Content.Res.Orientation;


namespace MALClient.Android.Fragments.AnimeDetailsPageTabs
{
    class AnimeDetailsPageStaffTabFragment : MalFragmentBase
    {
        private AnimeDetailsPageViewModel ViewModel;
        private GridViewColumnHelper _gridHelper;

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.AnimeDetails;
        }

        protected override void InitBindings()
        {
            //_gridHelper = new GridViewColumnHelper(AnimeDetailsPageCharactersTabGridView,null,2,3);
            //Bindings.Add(this.SetBinding(() => ViewModel.AnimeStaffData).WhenSourceChanges(() =>
            //{
            //    if (ViewModel.AnimeStaffData == null)
            //        AnimeDetailsPageCharactersTabGridView.Adapter = null;
            //    else
            //        AnimeDetailsPageCharactersTabGridView.InjectFlingAdapter(ViewModel.AnimeStaffData.AnimeStaff, DataTemplateFull, DataTemplateFling, ContainerTemplate);
            //}));

            //_gridHelper = new GridViewColumnHelper(AnimeDetailsPageCharactersTabGridView,340,1);
            Bindings.Add(this.SetBinding(() => ViewModel.AnimeStaffData).WhenSourceChanges(() =>
            {
                //if (ViewModel.AnimeStaffData == null)
                //    AnimeDetailsPageCharactersTabGridView.Adapter = null;
                //else
                //    AnimeDetailsPageCharactersTabGridView.InjectFlingAdapter(ViewModel.AnimeStaffData.AnimeCharacterPairs, DataTemplateFull, DataTemplateFling, ContainerTemplate);

                if (ViewModel.AnimeStaffData == null)
                    AnimeDetailsPageCharactersTabGridView.SetAdapter(null);
                else
                    AnimeDetailsPageCharactersTabGridView.SetAdapter(
                        new ObservableRecyclerAdapter<
                            FavouriteViewModel, 
                            Holder>(
                            ViewModel.AnimeStaffData.AnimeStaff,
                            DataTemplate, ItemTemplate, HolderFactory));

            }));

            AnimeDetailsPageCharactersTabGridView.SetLayoutManager(new GridLayoutManager(Activity, 3));

            Bindings.Add(
                this.SetBinding(() => ViewModel.LoadingCharactersVisibility,
                    () => AnimeDetailsPageCharactersTabLoadingSpinner.Visibility)
                    .ConvertSourceToTarget(Converters.BoolToVisibility));
        }

        private Holder HolderFactory(ViewGroup parent, int viewtype, View view)
        {
            return new Holder(view);
        }

        private View ItemTemplate(int viewtype)
        {
            var item = new FavouriteItem(Activity);
            return item;
        }

        private void DataTemplate(FavouriteViewModel item, Holder holder, int position)
        {
            var favItem = holder.ItemView as FavouriteItem;
            favItem.BindModel(item, false);
            favItem.RootContainer.SetOnClickListener(new OnClickListener(view => PersonOnClick(item)));
        }

        private void PersonOnClick(FavouriteViewModel item)
        {
            ViewModel.NavigateStaffDetailsCommand.Execute(item.Data);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            //SetUpForOrientation(newConfig.Orientation);
            //_gridHelper.OnConfigurationChanged(newConfig);
            base.OnConfigurationChanged(newConfig);
        }

        private void SetUpForOrientation(Orientation orientation)
        {
            ViewGroup.LayoutParams param;
            switch (orientation)
            {
                case Orientation.Landscape:
                    param = RootView.LayoutParameters;
                    param.Height = ViewGroup.LayoutParams.WrapContent;
                    RootView.LayoutParameters = param;
                    break;
                case Orientation.Portrait:
                case Orientation.Square:
                case Orientation.Undefined:
                    param = RootView.LayoutParameters;
                    param.Height = ViewGroup.LayoutParams.MatchParent;
                    RootView.LayoutParameters = param;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }


        class Holder : RecyclerView.ViewHolder
        {
            private readonly View _view;

            public Holder(View view) : base(view)
            {
                _view = view;
            }

            private ProgressBar _favouriteItemImgPlaceholder;
            private ImageView _favouriteItemNoImageIcon;
            private ImageViewAsync _favouriteItemImage;
            private FavouriteButton _favouriteItemFavButton;
            private RelativeLayout _favouriteItemUpperSection;
            private TextView _favouriteItemName;
            private TextView _favouriteItemRole;
            private LinearLayout _favouriteItemLowerSection;

            public ProgressBar FavouriteItemImgPlaceholder => _favouriteItemImgPlaceholder ?? (_favouriteItemImgPlaceholder = _view.FindViewById<ProgressBar>(Resource.Id.FavouriteItemImgPlaceholder));
            public ImageView FavouriteItemNoImageIcon => _favouriteItemNoImageIcon ?? (_favouriteItemNoImageIcon = _view.FindViewById<ImageView>(Resource.Id.FavouriteItemNoImageIcon));
            public ImageViewAsync FavouriteItemImage => _favouriteItemImage ?? (_favouriteItemImage = _view.FindViewById<ImageViewAsync>(Resource.Id.FavouriteItemImage));
            public FavouriteButton FavouriteItemFavButton => _favouriteItemFavButton ?? (_favouriteItemFavButton = _view.FindViewById<FavouriteButton>(Resource.Id.FavouriteItemFavButton));
            public RelativeLayout FavouriteItemUpperSection => _favouriteItemUpperSection ?? (_favouriteItemUpperSection = _view.FindViewById<RelativeLayout>(Resource.Id.FavouriteItemUpperSection));
            public TextView FavouriteItemName => _favouriteItemName ?? (_favouriteItemName = _view.FindViewById<TextView>(Resource.Id.FavouriteItemName));
            public TextView FavouriteItemRole => _favouriteItemRole ?? (_favouriteItemRole = _view.FindViewById<TextView>(Resource.Id.FavouriteItemRole));
            public LinearLayout FavouriteItemLowerSection => _favouriteItemLowerSection ?? (_favouriteItemLowerSection = _view.FindViewById<LinearLayout>(Resource.Id.FavouriteItemLowerSection));
        }


        public override int LayoutResourceId => Resource.Layout.AnimeDetailsPageCharactersTab;

        #region Views

        private RecyclerView _animeDetailsPageCharactersTabGridView;
        private ProgressBar _animeDetailsPageCharactersTabLoadingSpinner;

        public RecyclerView AnimeDetailsPageCharactersTabGridView => _animeDetailsPageCharactersTabGridView ?? (_animeDetailsPageCharactersTabGridView = FindViewById<RecyclerView>(Resource.Id.AnimeDetailsPageCharactersTabGridView));

        public ProgressBar AnimeDetailsPageCharactersTabLoadingSpinner => _animeDetailsPageCharactersTabLoadingSpinner ?? (_animeDetailsPageCharactersTabLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.AnimeDetailsPageCharactersTabLoadingSpinner));

        #endregion
    }
}