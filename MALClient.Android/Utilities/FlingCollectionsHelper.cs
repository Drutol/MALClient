using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;

namespace MALClient.Android
{
    public static class FlingCollectionsHelper
    {
        private static readonly Dictionary<View, bool> FlingStates = new Dictionary<View, bool>();

        public static void InjectFlingAdapter<T>(this AbsListView container, IList<T> items,
            Action<View, T> dataTemplateFull, Action<View,T> dataTemplateFling,
            Func<View> containerTemplate) where T : class
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
                        dataTemplateFull(view,view.Tag.Unwrap<T>() );
                    }
                }
            });
            container.Adapter = items.GetAdapter((i, arg2, arg3) =>
            {
                var root = arg3 ?? containerTemplate();
                root.Tag = arg2.Wrap();
                if (FlingStates[container])
                    dataTemplateFling(root,arg2);
                else
                    dataTemplateFull(root,arg2);
                return root;
            });
        }

        public static void ClearFlingAdapter(this AbsListView container)
        {
            if (FlingStates.ContainsKey(container))
                FlingStates.Remove(container);
            container.SetOnScrollListener(null);
        }
    }
}