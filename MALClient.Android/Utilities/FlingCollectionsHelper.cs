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
        private static volatile Dictionary<View, bool> _flingStates = new Dictionary<View, bool>();

        public static void InjectFlingAdapter<T>(this AbsListView container, List<T> items,
            Action<View, T> dataTemplateFull, Action<View,T> dataTemplateFling,
            Func<View> containerTemplate) where T : class
        {
            _flingStates.Add(container,false);
            container.MakeFlingAware(b =>
            {
                if(_flingStates[container] == b)
                    return;
                _flingStates[container] = b;
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
                if (_flingStates[container])
                    dataTemplateFling(root,arg2);
                else
                    dataTemplateFull(root,arg2);
                return root;
            });
        }

        public static void ClearFlingAdapter(this AbsListView container)
        {
            if (_flingStates.ContainsKey(container))
                _flingStates.Remove(container);
            container.SetOnScrollListener(null);
        }
    }
}