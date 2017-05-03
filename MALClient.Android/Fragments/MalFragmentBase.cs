using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Oguzdev.Circularfloatingactionmenu.Library;
using GalaSoft.MvvmLight.Helpers;
using MALClient.XShared.Utils;


namespace MALClient.Android.Fragments
{
    public abstract class MalFragmentBase : Fragment
    {
        private readonly bool _initBindings;
        private readonly bool _detachBindingOnDestroy;

        protected MalFragmentBase(bool initBindings = true,bool detachBindingOnDestroy = true)
        {
            _initBindings = initBindings;
            _detachBindingOnDestroy = detachBindingOnDestroy;
        }

        protected bool HasOnlyManualBindings { get; set; }

        protected View RootView { get; private set; }

        protected List<Binding> Bindings = new List<Binding>();

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
            if (_initBindings && !Bindings.Any())
                InitBindings();
            return RootView;
        }

        public sealed override void OnStop()
        {
            if(_detachBindingOnDestroy)
                DetachBindings();
            base.OnStop();
        }

        public void DetachBindings()
        {
            Bindings?.ForEach(binding => binding.Detach());
            Bindings = new List<Binding>();
            Cleanup();
        }

        public void ReattachBindings()
        {
            if(!Bindings.Any() && RootView != null && !HasOnlyManualBindings)
                InitBindings();
        }

        public override void OnResume()
        {
            ReattachBindings();
            base.OnResume();
        }

        protected virtual void Cleanup()
        {
            HasOnlyManualBindings = false;
        }

        public override Context Context
        {
            get
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.M)
                    return Activity;
                return base.Context;
            }
        }
    }
}