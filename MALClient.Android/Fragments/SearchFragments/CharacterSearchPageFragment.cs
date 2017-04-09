using System;
using System.Collections.Generic;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Resources;
using MALClient.Android.UserControls;
using MALClient.Models.Enums;
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
            CharacterSearchPageList.InjectFlingAdapter(ViewModel.FoundCharacters, DataTemplateFull, DataTemplateFling,
                ContainerTemplate);
            _gridViewColumnHelper = new GridViewColumnHelper(CharacterSearchPageList);

            
            Bindings.Add(
                this.SetBinding(() => ViewModel.Loading,
                    () => CharacterSearchPageLoadingSpinner.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));
        }

        private View ContainerTemplate(int i)
        {
            return new FavouriteItem(Context);
        }

        private void DataTemplateFling(View view, int i, FavouriteViewModel arg3)
        {
            ((FavouriteItem) view).BindModel(arg3,true);
        }

        private void DataTemplateFull(View view, int i, FavouriteViewModel arg3)
        {
            var item = (FavouriteItem) view;
            var firstRun = !item.Initialized;
            item.BindModel(arg3, false);
            if (firstRun)
            {
                item.Click += ItemOnClick;
            }
        }

        private void ItemOnClick(object sender, EventArgs eventArgs)
        {
            ViewModel.NavigateCharacterDetailsCommand.Execute((sender as View).Tag.Unwrap<FavouriteViewModel>());
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.CharacterSearch;
            ViewModel.Init(_prevArgs);
            ViewModelLocator.NavMgr.DeregisterBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
        }


        public override int LayoutResourceId => Resource.Layout.CharacterSearchPage;

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            _gridViewColumnHelper?.OnConfigurationChanged(newConfig);
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