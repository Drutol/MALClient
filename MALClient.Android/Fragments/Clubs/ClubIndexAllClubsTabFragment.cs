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
using MALClient.Android.Listeners;
using MALClient.Android.Resources;
using MALClient.Models.Enums;
using MALClient.XShared.Comm.MagicalRawQueries.Clubs;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Fragments.Clubs
{
    public class ClubIndexAllClubsTabFragment : ClubIndexTabFragmentBase
    {
        protected override void Init(Bundle savedInstanceState)
        {

        }

        protected override void InitBindings()
        {
            Bindings.Add(this.SetBinding(() => ViewModel.Clubs).WhenSourceChanges(() =>
            {
                if (ViewModel.MyClubs == null)
                    List.Adapter = null;
                else
                    List.InjectFlingAdapter(ViewModel.Clubs, ViewHolderFactory, DataTemplateFull, DataTemplateFling, DataTemplateBasic, ContainerTemplate);
            }));

            Bindings.Add(this.SetBinding(() => ViewModel.QueryType).WhenSourceChanges(async () =>
            {
                if (ViewModel.QueryType == MalClubQueries.QueryType.All)
                {
                    await Task.Delay(400);
                    InitDrawer();
                }
            }));

            Bindings.Add(
                this.SetBinding(() => ViewModel.SearchQuery,
                    () => SearchEditBox.Text,BindingMode.TwoWay));

            SearchButton.SetOnClickListener(new OnClickListener(v => ViewModel.SearchCommand.Execute(null)));

            ActionButton.SetOnClickListener(new OnClickListener(v=> OpenFiltersDrawer()));
        }

        public override int LayoutResourceId => Resource.Layout.ClubsIndexAllClubsTab;

        #region Hamburger
        private Drawer _rightDrawer;

        private void InitDrawer()
        {
            if (_rightDrawer != null)
                return;

            var builder = new DrawerBuilder().WithActivity(Activity);
            builder.WithSliderBackgroundColorRes(ResourceExtension.BrushHamburgerBackgroundRes);
            builder.WithStickyFooterShadow(true);
            builder.WithDisplayBelowStatusBar(true);
            builder.WithDrawerGravity((int)GravityFlags.Right);

            builder.WithStickyHeaderShadow(true);
            builder.WithStickyHeader(Resource.Layout.AnimeListPageDrawerHeader);

            _rightDrawer = builder.Build();
            _rightDrawer.DrawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            _rightDrawer.StickyHeader.SetBackgroundColor(new Color(ResourceExtension.BrushAppBars));
            _rightDrawer.DrawerLayout.AddDrawerListener(new DrawerListener(() => ViewModelLocator.NavMgr.ResetOneTimeOverride(), null));
        }

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

        private EditText _searchEditBox;
        private ImageButton _searchButton;
        private ListView _list;
        private FloatingActionButton _actionButton;

        public EditText SearchEditBox => _searchEditBox ?? (_searchEditBox = FindViewById<EditText>(Resource.Id.SearchEditBox));

        public ImageButton SearchButton => _searchButton ?? (_searchButton = FindViewById<ImageButton>(Resource.Id.SearchButton));

        public ListView List => _list ?? (_list = FindViewById<ListView>(Resource.Id.List));

        public FloatingActionButton ActionButton => _actionButton ?? (_actionButton = FindViewById<FloatingActionButton>(Resource.Id.ActionButton));

        #endregion

        public override bool ShowJoinControls { get; } = true;
    }
}