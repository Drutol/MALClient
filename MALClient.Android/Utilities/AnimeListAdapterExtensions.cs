using System;
using System.Collections;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.UserControls;
using MALClient.Android.UserControls.AnimeItems;
using MALClient.Models.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android
{
    public static class AnimeListAdapterExtensions
    {
        public static void InjectAnimeListAdapter(this AbsListView listView, Context context,
            IList<AnimeItemViewModel> items,
            AnimeListDisplayModes mode, Action<AnimeItemViewModel> onClick = null, bool allowGridItemSwipe = true)
        {
            switch (mode)
            {
                case AnimeListDisplayModes.IndefiniteList:
                    listView.InjectFlingAdapter(items, (view,i, model) =>
                        {
                            ((AnimeListItem) view).BindModel(model, false);
                        },
                        (view,i, model) =>
                        {
                            ((AnimeListItem) view).BindModel(model, true);
                        },
                        i => new AnimeListItem(context, onClick)
                    );
                    break;
                case AnimeListDisplayModes.IndefiniteGrid:
                    listView.InjectFlingAdapter(items, (view,i, model) =>
                        {
                            ((AnimeGridItem) view).BindModel(model, false);
                        },
                        (view,i, model) =>
                        {
                            ((AnimeGridItem) view).BindModel(model, true);
                        },
                        i => new AnimeGridItem(context, allowGridItemSwipe, onClick)
                    );
                    break;
                case AnimeListDisplayModes.IndefiniteCompactList:
                    listView.InjectFlingAdapter(items, (view,i, model) =>
                        {
                            var item = (AnimeCompactItem)view;
                            item.Position = i;
                            item.BindModel(model, false);
                        },
                        (view,i, model) =>
                        {
                            var item = (AnimeCompactItem)view;
                            item.Position = i;
                            item.BindModel(model, true);
                        },
                        i => new AnimeCompactItem(context, i, onClick)
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public static void InjectAnimeListAdapterWithFooter(this AbsListView listView, Context context,
            IList<AnimeItemViewModel> items,
            AnimeListDisplayModes mode, View footer, Action<AnimeItemViewModel> onClick = null, bool allowGridItemSwipe = true)
        {
            switch (mode)
            {
                case AnimeListDisplayModes.IndefiniteList:
                    listView.InjectFlingAdapter(items, (view, i, model) =>
                        {
                            ((AnimeListItem) view).BindModel(model, false);
                        },
                        (view, i, model) =>
                        {
                            ((AnimeListItem) view).BindModel(model, true);
                        },
                        i => new AnimeListItem(context, onClick)
                        , footer);
                    break;
                case AnimeListDisplayModes.IndefiniteGrid:
                    listView.InjectFlingAdapter(items, (view, i, model) =>
                        {
                            ((AnimeGridItem) view).BindModel(model, false);
                        },
                        (view, i, model) =>
                        {
                            ((AnimeGridItem) view).BindModel(model, true);
                        },
                        i => new AnimeGridItem(context, allowGridItemSwipe, onClick)
                        , footer);
                    break;
                case AnimeListDisplayModes.IndefiniteCompactList:
                    listView.InjectFlingAdapter(items, (view, i, model) =>
                        {
                            var item = (AnimeCompactItem) view;
                            item.Position = i;
                            item.BindModel(model, false);
                        },
                        (view, i, model) =>
                        {
                            var item = (AnimeCompactItem) view;
                            item.Position = i;
                            item.BindModel(model, true);
                        },
                        i => new AnimeCompactItem(context, i, onClick)
                        , footer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public static void SetAnimeListAdapter(this LinearLayout listView, Context context,
            IList<AnimeItemViewModel> items,
            AnimeListDisplayModes mode, Action<AnimeItemViewModel> onClick = null)
        {
            switch (mode)
            {
                case AnimeListDisplayModes.IndefiniteList:
                    listView.SetAdapter(items.GetAdapter((i, model, arg3) =>
                        {
                            var view = new AnimeListItem(context, onClick);
                            view.BindModel(model, false);
                            return view;
                        })
                    );
                    break;
                case AnimeListDisplayModes.IndefiniteGrid:
                    listView.SetAdapter(items.GetAdapter((i, model, arg3) =>
                        {
                            var view = new AnimeGridItem(context, false, onClick);
                            view.BindModel(model, false);
                            return view;
                        })
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode,
                        "SetAnimeListAdapter, do we want compact now?");
            }
        }
    }
}