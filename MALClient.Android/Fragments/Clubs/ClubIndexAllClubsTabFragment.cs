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

namespace MALClient.Android.Fragments.Clubs
{
    public class ClubIndexAllClubsTabFragment : ClubIndexTabFragmentBase
    {
        protected override void Init(Bundle savedInstanceState)
        {
            throw new NotImplementedException();
        }

        protected override void InitBindings()
        {
            throw new NotImplementedException();
        }

        public override int LayoutResourceId { get; }

        #region Views

        private Spinner _comboBox;
        private EditText _searchEditBox;
        private Button _searchButton;
        private ListView _list;

        public Spinner ComboBox => _comboBox ?? (_comboBox = FindViewById<Spinner>(Resource.Id.ComboBox));

        public EditText SearchEditBox => _searchEditBox ?? (_searchEditBox = FindViewById<EditText>(Resource.Id.SearchEditBox));

        public Button SearchButton => _searchButton ?? (_searchButton = FindViewById<Button>(Resource.Id.SearchButton));

        public ListView List => _list ?? (_list = FindViewById<ListView>(Resource.Id.List));

        #endregion
    }
}