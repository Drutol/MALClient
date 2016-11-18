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

        protected Dictionary<int, List<Binding>> Bindings = new Dictionary<int, List<Binding>>();

        protected abstract void Init(Bundle savedInstanceState);

        protected abstract void InitBindings();

        public abstract int LayoutResourceId { get; }

        protected T FindViewById<T>(int id) where T : View => RootView.FindViewById<T>(id);

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Init(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (RootView == null)
                RootView = inflater.Inflate(LayoutResourceId, container, false);
            InitBindings();
            return RootView;
        }

        public sealed override void OnStop()
        {
            Bindings?.ForEach(pair => pair.Value.ForEach(binding => binding.Detach()));
            Bindings = new Dictionary<int, List<Binding>>();
            Cleanup();
            base.OnStop();
        }

        protected virtual void Cleanup()
        {

        }
    }
}