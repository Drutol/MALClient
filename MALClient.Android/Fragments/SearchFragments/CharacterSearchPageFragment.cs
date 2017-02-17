using System.Collections.Generic;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.BindingInformation;
using MALClient.Android.BindingInformation.StaticBindings;
using MALClient.Android.Resources;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.SearchFragments
{
    public class CharacterSearchPageFragment : MalFragmentBase
    {
        private static SearchPageNavArgsBase _prevArgs;

        private CharacterSearchViewModel ViewModel;
        private GridViewColumnHelper _gridViewColumnHelper;

        private CharacterSearchPageFragment(bool initBindings) : base(initBindings)
        {
            
        }

        protected override void InitBindings()
        {
            CharacterSearchPageList.Adapter = ViewModel.FoundCharacters.GetAdapter(GetTemplateDelegate);
            _gridViewColumnHelper = new GridViewColumnHelper(CharacterSearchPageList);

            
            Bindings.Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => CharacterSearchPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.CharacterSearch;
            ViewModel.Init(_prevArgs);
        }

        private View GetTemplateDelegate(int i, FavouriteViewModel favouriteViewModel, View convertView)
        {
            var view = convertView ?? MainActivity.CurrentContext.LayoutInflater.Inflate(Resource.Layout.CharacterItem,null);

            view.SetBinding(FavouriteItemBindingInfo.Instance,favouriteViewModel);

            return view;
        }

        public override int LayoutResourceId => Resource.Layout.CharacterSearchPage;

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            _gridViewColumnHelper.OnConfigurationChanged(newConfig);
            base.OnConfigurationChanged(newConfig);
        }

        #region Views

        private GridView _characterSearchPageList;
        private ProgressBar _characterSearchPageLoadingSpinner;

        public GridView CharacterSearchPageList => _characterSearchPageList ?? (_characterSearchPageList = FindViewById<GridView>(Resource.Id.CharacterSearchPageList));

        public ProgressBar CharacterSearchPageLoadingSpinner => _characterSearchPageLoadingSpinner ?? (_characterSearchPageLoadingSpinner = FindViewById<ProgressBar>(Resource.Id.CharacterSearchPageLoadingSpinner));



        #endregion

        public static CharacterSearchPageFragment BuildInstance(SearchPageNavArgsBase args,bool initBindings = false)
        {
            _prevArgs = args;
            return new CharacterSearchPageFragment(initBindings);
        }
    }
}