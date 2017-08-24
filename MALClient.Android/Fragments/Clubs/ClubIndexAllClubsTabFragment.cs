using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Com.Mikepenz.Materialdrawer;
using Com.Mikepenz.Materialdrawer.Model.Interfaces;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.BindingConverters;
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.Comm.MagicalRawQueries.Clubs;
using MALClient.XShared.ViewModels;
using SearchView = Android.Support.V7.Widget.SearchView;

namespace MALClient.Android.Fragments.Clubs
{
    public class ClubIndexAllClubsTabFragment : ClubIndexTabFragmentBase
    {
        private bool _settingQuery;

        private Drawer _rightDrawer;

        public ClubIndexAllClubsTabFragment(Drawer drawer)
        {
            _rightDrawer = drawer;
        }

        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {
            Bindings.Add(this.SetBinding(() => ViewModel.Clubs).WhenSourceChanges(() =>
            {
                if (ViewModel.MyClubs == null)
                {
                    List.ClearFlingAdapter();
                    List.Adapter = null;
                }
                else
                {
                    List.ClearFlingAdapter();
                    List.InjectFlingAdapter(ViewModel.Clubs, ViewHolderFactory, DataTemplateFull, DataTemplateFling, DataTemplateBasic, ContainerTemplate);
                }
            }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.EmptyNoticeVisibility,
                    () => EmptyNotice.Visibility).ConvertSourceToTarget(Converters.BoolToVisibility));

            Bindings.Add(this.SetBinding(() => ViewModel.SearchQuery).WhenSourceChanges(() =>
            {
                if(!_settingQuery)
                    SearchView.SetQuery(ViewModel.SearchQuery,false);
                if(ViewModel.SearchQuery?.Length > 2)
                    ActionButton.Show();
                else
                    ActionButton.Hide();
            }));

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                List.SetOnScrollChangeListener(new ScrollListener(i =>
                {
                    if (ViewModel.Loading || !ViewModel.MoreButtonVisibility || List.Adapter == null ||
                        ViewModel.QueryType == MalClubQueries.QueryType.My)
                        return;
                    if (List.Adapter.Count - List.FirstVisiblePosition <= 2)
                        ViewModel.MoreCommand.Execute(null);
                }));
            }


            SearchView.FindViewById(Resource.Id.search_close_btn).Alpha = 1;
            SearchView.FindViewById(Resource.Id.search_close_btn).Clickable = true;
            SearchView.FindViewById(Resource.Id.search_close_btn).SetOnClickListener(new OnClickListener(view =>
            {
                SearchView.SetQuery("",false);
                ViewModel.SearchCommand.Execute(null);
            }));

            SearchView.QueryTextChange += SearchViewOnQueryTextChange;
            SearchView.QueryTextSubmit += SearchViewOnQueryTextSubmit;

            ActionButton.SetOnClickListener(new OnClickListener(v=> OpenFiltersDrawer()));
        }

        private void SearchViewOnQueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs queryTextSubmitEventArgs)
        {
            AndroidUtilities.HideKeyboard();
            ViewModel.SearchCommand.Execute(null);
        }

        private void SearchViewOnQueryTextChange(object sender, SearchView.QueryTextChangeEventArgs queryTextChangeEventArgs)
        {
            _settingQuery = true;
            ViewModel.SearchQuery = queryTextChangeEventArgs.NewText;
            _settingQuery = false;
            queryTextChangeEventArgs.Handled = true;
        }

        public override int LayoutResourceId => Resource.Layout.ClubsIndexAllClubsTab;

        #region Hamburger

        private void OpenFiltersDrawer()
        {


            if (ViewModel.Loading || _rightDrawer == null)
                return;

            var items = new List<IDrawerItem>();
            foreach (MalClubQueries.SearchCategory group in Enum.GetValues(typeof(MalClubQueries.SearchCategory)))
            {
                var item = HamburgerUtilities.GetBaseSecondaryItem();
                item.WithName(group.GetDescription());
                item.WithIdentifier((int)group);

                items.Add(item);
            }

            _rightDrawer.SetItems(items);
            _rightDrawer.SetSelection((int)ViewModel.SearchCategory);


            _rightDrawer.StickyHeader.FindViewById<TextView>(Resource.Id.AnimeListPageDrawerHeaderText).Text = "Filters";
            _rightDrawer.StickyHeader.FindViewById<ImageView>(Resource.Id.AnimeListPageDrawerHeaderIcon).SetImageResource(
                Resource.Drawable.icon_filter);
            _rightDrawer.OnDrawerItemClickListener = new HamburgerItemClickListener((view, i, arg3) =>
            {
                ViewModel.SearchCategory = (MalClubQueries.SearchCategory)arg3.Identifier;
                ViewModel.SearchCommand.Execute(null);

                _rightDrawer.OnDrawerItemClickListener = null;
                _rightDrawer.CloseDrawer();
            });


            ViewModelLocator.NavMgr.RegisterOneTimeMainOverride(new RelayCommand(CloseDrawer));
            _rightDrawer.OpenDrawer();
        }

        private void CloseDrawer()
        {
            _rightDrawer.CloseDrawer();
        }

        #endregion

        #region Views

        private SearchView _searchView;
        private ListView _list;
        private FloatingActionButton _actionButton;
        private TextView _emptyNotice;


        public SearchView SearchView => _searchView ?? (_searchView = FindViewById<SearchView>(Resource.Id.SearchView));

        public ListView List => _list ?? (_list = FindViewById<ListView>(Resource.Id.List));

        public FloatingActionButton ActionButton => _actionButton ?? (_actionButton = FindViewById<FloatingActionButton>(Resource.Id.ActionButton));

        public TextView EmptyNotice => _emptyNotice ?? (_emptyNotice = FindViewById<TextView>(Resource.Id.EmptyNotice));

        #endregion

    }
}