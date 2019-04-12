using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.View.Animation;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.Android.Activities;
using MALClient.Android.CollectionAdapters;
using MALClient.Android.Listeners;
using MALClient.XShared.ViewModels;

namespace MALClient.Android
{
    public static class AndroidUtilities
    {
        public static T ClassCast<T>(this Java.Lang.Object obj) where T : class
        {
            var propertyInfo = obj.GetType().GetProperty("Instance");
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, null) as T;
        }

        public static T EnumCast<T>(this Java.Lang.Object obj)
        {
            var propertyInfo = obj.GetType().GetProperty("Instance");
            return propertyInfo == null ? default(T) : (T)propertyInfo.GetValue(obj, null);
        }

        private static readonly Dictionary<INotifyPropertyChanged, Tuple<List<Tuple<string, Action>>, PropertyChangedEventHandler>> RegisteredActions =
            new Dictionary<INotifyPropertyChanged, Tuple<List<Tuple<string, Action>>, PropertyChangedEventHandler>>();

        public static void RegisterOneTimeOnPropertyChangedAction(this INotifyPropertyChanged viewModel, string property, Action action)
        {
            if (!RegisteredActions.ContainsKey(viewModel))
            {
                var dlgt = new PropertyChangedEventHandler(OnPropertyChangedHandler);
                RegisteredActions.Add(viewModel,new Tuple<List<Tuple<string, Action>>, PropertyChangedEventHandler>(new List<Tuple<string, Action>>(),dlgt ));
                viewModel.PropertyChanged += dlgt;
            }
            RegisteredActions[viewModel].Item1.Add(new Tuple<string, Action>(property,action));        
        }
        private static void OnPropertyChangedHandler(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var viewModel = sender as INotifyPropertyChanged;
            if (viewModel == null)
                return;
            Tuple<List<Tuple<string, Action>>, PropertyChangedEventHandler> data;
            if (RegisteredActions.TryGetValue(viewModel, out data))
            {
                List<Tuple<string, Action>> removed = new List<Tuple<string, Action>>();
                foreach (var tuple in data.Item1)
                {
                    if (propertyChangedEventArgs.PropertyName == tuple.Item1)
                    {
                        tuple.Item2.Invoke();                        
                        removed.Add(tuple);
                    }
                }
                foreach (var tuple in removed)
                {
                    RegisteredActions[viewModel].Item1.Remove(tuple);
                }
                if(!RegisteredActions[viewModel].Item1.Any())
                    viewModel.PropertyChanged -= data.Item2;
            }
        }

        public static void AnimateFadeIn(this View view)
        {
            Animation fadeIn = new AlphaAnimation(0, 1);
            fadeIn.Interpolator = new LinearInterpolator();
            fadeIn.Duration = 500;
            try
            {            
                MainActivity.CurrentContext.RunOnUiThread(() =>
                {

                    view.StartAnimation(fadeIn);
                    view.Visibility = ViewStates.Visible;
                });
            }
            catch (Exception e)
            {
                //ResourceLocator.TelemetryProvider.LogEvent($"Crash - animating fade in - {e.Message}");
            }

        }

        public static void SetAdapter(this LinearLayout layout, BaseAdapter adapter)
        {
            layout.RemoveAllViews();
            for (int i = 0; i < adapter.Count; i++)
            {
                layout.AddView(adapter.GetView(i,null,layout));
            }
        }

        public static void MakeFlingAware(this AbsListView list, Action<bool> action)
        {
            list.SetOnScrollListener(new ScrollChangedListener((view, state) =>
            {
                action.Invoke(state == ScrollState.Fling);
            }));
        }

        public static TObj Unwrap<TObj>(this Java.Lang.Object obj) where TObj : class
        {
            return (obj as JavaObjectWrapper<TObj>)?.Instance;
        }

        public static JavaObjectWrapper<TObj> Wrap<TObj>(this TObj obj) where TObj : class
        {
            return new JavaObjectWrapper<TObj>(obj);
        }

        public static ObservableAdapter<T> GetAdapter<T>(
            this IList<T> collection,
            Func<int, T, View, View> getTemplateDelegate, View footer, bool forGridView)
        {
            footer.Tag = "Footer";
            if (forGridView)
                return new ObservableGridViewAdapterWithFooter<T>()
                {
                    DataSource = collection,
                    GetTemplateDelegate = getTemplateDelegate,
                    Footer = footer,
                };
            return new ObservableAdapterWithFooter<T>
            {
                DataSource = collection,
                GetTemplateDelegate = getTemplateDelegate,
                Footer = footer,
            };
        }

        public static bool IsKeyboardVisibile()
        {
            var fragmentRootView = MainActivity.CurrentContext.MainPageRoot;
            var r = new Rect();
            fragmentRootView.GetWindowVisibleDisplayFrame(r);
            int keypadHeight = fragmentRootView.RootView.Height - r.Bottom;

            if (keypadHeight > fragmentRootView.Height * 0.15)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void HideKeyboard()
        {
            if (IsKeyboardVisibile())
            {
                var inputManager = (InputMethodManager)MainActivity.CurrentContext.GetSystemService(Context.InputMethodService);
                inputManager.ToggleSoftInput(ShowFlags.Forced, 0);
            }
        }

        /// <summary>
        /// Sets margins in dp
        /// </summary>
        /// <param name="view"></param>
        /// <param name="l"></param>
        /// <param name="t"></param>
        /// <param name="r"></param>
        /// <param name="b"></param>
        public static void SetMargins(this View view, float l, float t, float r, float b)
        {
            var param = view.LayoutParameters as ViewGroup.MarginLayoutParams;
            param.SetMargins(DimensionsHelper.DpToPx(l), DimensionsHelper.DpToPx(t), DimensionsHelper.DpToPx(r),
                DimensionsHelper.DpToPx(b));
            view.LayoutParameters = param;
        }

    }
}