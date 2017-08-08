using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Views;
using Android.Widget;
using FFImageLoading.Extensions;
using GalaSoft.MvvmLight.Helpers;

namespace MALClient.Android
{
    public static class FlingCollectionsHelper
    {
        private static readonly Dictionary<AbsListView, bool> FlingStates = new Dictionary<AbsListView, bool>();
        private static readonly Dictionary<AbsListView, Dictionary<View, object>> ViewHolders = new Dictionary<AbsListView, Dictionary<View, object>>();

        public static void InjectFlingAdapter<T>(this AbsListView container, IList<T> items,
            Action<View,int, T> dataTemplateFull, Action<View,int,T> dataTemplateFling,
            Func<int,View> containerTemplate,Action<View,int,T> dataTemplateBasic = null,View footer = null,bool skipBugFix = false) where T : class
        {
            if(!FlingStates.ContainsKey(container))
                FlingStates.Add(container,false);
            container.MakeFlingAware(b =>
            {
                if(FlingStates[container] == b)
                    return;
                FlingStates[container] = b;
                if (!b)
                {
                    for (int i = 0; i < container.ChildCount; i++)
                    {
                        var view = container.GetChildAt(i);
                        var item = view.Tag.Unwrap<T>();
                        if (view.Tag?.ToString() == "Footer")
                            continue;  
                        dataTemplateFull(view,items.IndexOf(item),item);
                    }
                }
            });
            if (footer == null)
            {
                container.Adapter = items.GetAdapter((i, arg2, arg3) =>
                {
                    var root = arg3 ?? containerTemplate(i);
                    root.Tag = arg2.Wrap();
                    dataTemplateBasic?.Invoke(root, i, arg2);
                    if (FlingStates[container])
                        dataTemplateFling(root, i, arg2);
                    else
                        dataTemplateFull(root, i, arg2);
                    return root;
                });
            }
            else
            {
                container.Adapter = items.GetAdapter((i, arg2, arg3) =>
                {
                    var root = arg3 ?? containerTemplate(i);
                    root.Tag = arg2.Wrap();
                    if (FlingStates[container])
                        dataTemplateFling(root, i, arg2);
                    else
                        dataTemplateFull(root, i, arg2);
                    return root;
                },footer,container is GridView && !skipBugFix);
            }

        }

        public static void InjectFlingAdapter<T, TViewHolder>(this AbsListView container, IList<T> items, Func<View, TViewHolder> holderFactory,
            Action<View, int, T, TViewHolder> dataTemplateFull, Action<View, int, T, TViewHolder> dataTemplateFling,
            Action<View, int, T, TViewHolder> dataTemplateBasic, Func<int, View> containerTemplate, View footer = null, bool skipBugFix = false) where T : class
        {
            if (!FlingStates.ContainsKey(container))
                FlingStates.Add(container, false);
            if (!ViewHolders.ContainsKey(container))
                ViewHolders.Add(container, new Dictionary<View, object>());
            container.MakeFlingAware(b =>
            {
                if (FlingStates[container] == b)
                    return;
                FlingStates[container] = b;
                if (!b)
                {
                    for (int i = 0; i < container.ChildCount; i++)
                    {
                        var view = container.GetChildAt(i);
                        var item = view.Tag.Unwrap<T>();
                        if (view.Tag?.ToString() == "Footer")
                            continue;
                        dataTemplateFull(view, items.IndexOf(item), item, (TViewHolder) ViewHolders[container][view]);
                    }
                }
            });
            if (footer == null)
            {
                container.Adapter = items.GetAdapter((i, arg2, arg3) =>
                {
                    TViewHolder holder;
                    View root = null;
                    if (arg3 == null)
                    {
                        root = containerTemplate(i);
                        ViewHolders[container][root] = holder = holderFactory(root);
                    }
                    else
                    {
                        root = arg3;
                        holder = (TViewHolder) ViewHolders[container][root];
                    }
                    root.Tag = arg2.Wrap();
                    dataTemplateBasic.Invoke(root, i, arg2, holder);
                    if (FlingStates[container])
                        dataTemplateFling(root, i, arg2, holder);
                    else
                        dataTemplateFull(root, i, arg2, holder);
                    return root;
                });
            }
            else
            {
                container.Adapter = items.GetAdapter((i, arg2, arg3) =>
                {
                    TViewHolder holder;
                    View root = null;
                    if (arg3 == null)
                    {
                        root = containerTemplate(i);
                        ViewHolders[container][root] = holder = holderFactory(root);
                    }
                    else
                    {
                        root = arg3;
                        holder = (TViewHolder) ViewHolders[container][root];
                    }
                    root.Tag = arg2.Wrap();
                    if (FlingStates[container])
                        dataTemplateFling(root, i, arg2, holder);
                    else
                        dataTemplateFull(root, i, arg2, holder);
                    return root;
                }, footer, container is GridView && !skipBugFix);
            }

        }

        public static void ClearFlingAdapter(this AbsListView container)
        {
            if (FlingStates.ContainsKey(container))
                FlingStates.Remove(container);
            if (ViewHolders.ContainsKey(container))
                ViewHolders.Remove(container);
            
            container.SetOnScrollListener(null);
            container.Adapter = null;
        }
    }
}