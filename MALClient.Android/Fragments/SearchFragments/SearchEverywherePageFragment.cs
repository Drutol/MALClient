using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content.Res;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AoLibs.Adapters.Android.Recycler;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.BindingConverters;
using MALClient.Android.Resources;
using MALClient.Android.UserControls;
using MALClient.Models.Enums;
using MALClient.Models.Models.Search;
using MALClient.XShared.NavArgs;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.Android.Fragments.SearchFragments
{
    public class SearchEverywherePageFragment : MalFragmentBase
    {
        private static SearchPageNavArgsBase _prevArgs;

        private SearchEverywhereViewModel ViewModel;

        private SearchEverywherePageFragment(bool initBindings) : base(initBindings)
        {
            
        }

        protected override void InitBindings()
        {
            SearchRecyclerView.SetAdapter(new ObservableRecyclerAdapterWithMultipleViewTypes<ISearchEverywhereItem, RecyclerView.ViewHolder>(new Dictionary<Type, ObservableRecyclerAdapterWithMultipleViewTypes<ISearchEverywhereItem, RecyclerView.ViewHolder>.IItemEntry>
            {
                {
                    typeof(SearchCategoryItem),
                    new ObservableRecyclerAdapterWithMultipleViewTypes<ISearchEverywhereItem, RecyclerView.ViewHolder>.SpecializedItemEntry<SearchCategoryItem, CategoryHolder>
                    {
                        ItemTemplate = CategoryItemTemplate,
                        SpecializedDataTemplate = CategoryDataTemplate
                    }
                },
                {
                    typeof(SearchEverywhereAnimeItem),
                    new ObservableRecyclerAdapterWithMultipleViewTypes<ISearchEverywhereItem, RecyclerView.ViewHolder>.SpecializedItemEntry<SearchEverywhereAnimeItem, SearchItemHolder>
                    {
                        ItemTemplate = AnimeItemTemplate,
                        SpecializedDataTemplate = AnimeDataTemplate
                    }
                },
                {
                    typeof(SearchEverywhereMangaItem),
                    new ObservableRecyclerAdapterWithMultipleViewTypes<ISearchEverywhereItem, RecyclerView.ViewHolder>.SpecializedItemEntry<SearchEverywhereMangaItem, SearchItemHolder>
                    {
                        ItemTemplate = MangaItemTemplate,
                        SpecializedDataTemplate = MangaDataTemplate
                    }
                },
                {
                    typeof(SearchEverywhereCharacterItem),
                    new ObservableRecyclerAdapterWithMultipleViewTypes<ISearchEverywhereItem, RecyclerView.ViewHolder>.SpecializedItemEntry<SearchEverywhereCharacterItem, SearchItemHolder>
                    {
                        ItemTemplate = CharacterItemTemplate,
                        SpecializedDataTemplate = CharacterDataTemplate
                    }
                },
                {
                    typeof(SearchEverywherePersonItem),
                    new ObservableRecyclerAdapterWithMultipleViewTypes<ISearchEverywhereItem, RecyclerView.ViewHolder>.SpecializedItemEntry<SearchEverywherePersonItem, SearchItemHolder>
                    {
                        ItemTemplate = PersonItemTemplate,
                        SpecializedDataTemplate = PersonDataTemplate
                    }
                },
                {
                    typeof(SearchEverywhereUserItem),
                    new ObservableRecyclerAdapterWithMultipleViewTypes<ISearchEverywhereItem, RecyclerView.ViewHolder>.SpecializedItemEntry<SearchEverywhereUserItem, SearchItemHolder>
                    {
                        ItemTemplate = UserItemTemplate,
                        SpecializedDataTemplate = UserDataTemplate
                    }
                },
            }, 
            ViewModel.SearchResults)
            {
                StretchContentHorizonatally = true
            });
            SearchRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
        }

        private void CategoryDataTemplate(SearchCategoryItem item, CategoryHolder holder, int position)
        {
            holder.Category.Text = item.Name;

        }

        private View CategoryItemTemplate(int viewtype)
        {
            return LayoutInflater.Inflate(Resource.Layout.SearchEverywhereCategoryItem, null);
        }

        private View MangaItemTemplate(int viewtype)
        {
            return LayoutInflater.Inflate(Resource.Layout.SearchEverywhereItem, null);
        }

        private View CharacterItemTemplate(int viewtype)
        {
            return LayoutInflater.Inflate(Resource.Layout.SearchEverywhereItem, null);
        }

        private View PersonItemTemplate(int viewtype)
        {
            return LayoutInflater.Inflate(Resource.Layout.SearchEverywhereItem, null);
        }

        private View UserItemTemplate(int viewtype)
        {
            return LayoutInflater.Inflate(Resource.Layout.SearchEverywhereItem, null);
        }

        private View AnimeItemTemplate(int viewtype)
        {
            return LayoutInflater.Inflate(Resource.Layout.SearchEverywhereItem, null);
        }

        private void AnimeDataTemplate(SearchEverywhereAnimeItem item, SearchItemHolder holder, int position)
        {
            holder.Image.Into(item.Item.ImageUrl);
            holder.Title.Text = item.Item.Name;
            holder.Subtitle.Text = item.Item.Payload.Aired;
            holder.RightMarker.Text = item.Item.Payload.MediaType;
        }

        private void MangaDataTemplate(SearchEverywhereMangaItem item, SearchItemHolder holder, int position)
        {
            holder.Image.Into(item.Item.ImageUrl);
            holder.Title.Text = item.Item.Name;
            holder.Subtitle.Text = item.Item.Payload.Aired;
            holder.RightMarker.Text = item.Item.Payload.MediaType;
        }


        private void CharacterDataTemplate(SearchEverywhereCharacterItem item, SearchItemHolder holder, int position)
        {
            holder.Image.Into(item.Item.ImageUrl);
            holder.Title.Text = item.Item.Name;
            holder.Subtitle.Text = string.Join(", ", item.Item.Payload.RelatedWorks?.Take(2) ?? new List<string>());
            holder.RightMarker.Text = $"Favs: {item.Item.Payload.Favorites}";
        }


        private void PersonDataTemplate(SearchEverywherePersonItem item, SearchItemHolder holder, int position)
        {
            holder.Image.Into(item.Item.ImageUrl);
            holder.Title.Text = item.Item.Name;
            holder.Subtitle.Text = string.Empty;
            holder.RightMarker.Text = $"Favs: {item.Item.Payload.Favorites}";
        }


        private void UserDataTemplate(SearchEverywhereUserItem item, SearchItemHolder holder, int position)
        {
            holder.Image.Into(item.Item.ImageUrl);
            holder.Title.Text = item.Item.Name;
            holder.Subtitle.Text = string.Empty;
            holder.RightMarker.Text = string.Empty;
        }

        protected override void Init(Bundle savedInstanceState)
        {
            ViewModel = ViewModelLocator.SearchEverywhereViewModel;
            ViewModel.Init(_prevArgs);
            ViewModelLocator.NavMgr.DeregisterBackNav();
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageAnimeList, null);
        }

        public override int LayoutResourceId => Resource.Layout.SearchEverywherePage;

        #region Views

        private RecyclerView _searchRecyclerView;

        public RecyclerView SearchRecyclerView => _searchRecyclerView ?? (_searchRecyclerView = FindViewById<RecyclerView>(Resource.Id.SearchRecyclerView));

        #endregion

        class SearchItemHolder : RecyclerView.ViewHolder
        {
            private readonly View _view;

            public SearchItemHolder(View view) : base(view)
            {
                _view = view;
            }
            private ImageView _image;
            private TextView _title;
            private TextView _subtitle;
            private TextView _rightMarker;

            public ImageView Image => _image ?? (_image = _view.FindViewById<ImageView>(Resource.Id.Image));
            public TextView Title => _title ?? (_title = _view.FindViewById<TextView>(Resource.Id.Title));
            public TextView Subtitle => _subtitle ?? (_subtitle = _view.FindViewById<TextView>(Resource.Id.Subtitle));
            public TextView RightMarker => _rightMarker ?? (_rightMarker = _view.FindViewById<TextView>(Resource.Id.RightMarker));
        }

        class CategoryHolder : RecyclerView.ViewHolder
        {
            private readonly View _view;

            public CategoryHolder(View view) : base(view)
            {
                _view = view;
            }
            private TextView _category;

            public TextView Category => _category ?? (_category = _view.FindViewById<TextView>(Resource.Id.Category));
        }


        public static SearchEverywherePageFragment BuildInstance(SearchPageNavArgsBase args,bool initBindings = false)
        {
            _prevArgs = args;
            return new SearchEverywherePageFragment(initBindings);
        }
    }
}