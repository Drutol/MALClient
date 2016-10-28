using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using MALClient.XShared.Utils;
using Android.Support.V4.App;

namespace MALClient.Android.Fragments
{
    public abstract class MalFragmentBase : Fragment
    {
        protected View RootView { get; private set; }

        protected Dictionary<int, Binding> _bindings;

        protected abstract void Init(Bundle savedInstanceState);

        protected abstract void InitBindings();

        public abstract int LayoutResourceId { get; }

        protected virtual void Cleanup()
        {

        }

        protected T FindViewById<T>(int id) where T : View => RootView.FindViewById<T>(id);

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Init(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            RootView = inflater.Inflate(LayoutResourceId, container, false);
            InitBindings();
            return RootView;
        }

        public sealed override void OnStop()
        {
            _bindings?.ForEach(pair => pair.Value.Detach());
            _bindings = null;
            Cleanup();
            base.OnStop();
        }
    }
}