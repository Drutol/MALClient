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
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Views;
using MALClient.XShared.Utils;

namespace MALClient.Android.Activities
{
    public abstract class MalActivityBase : ActivityBase
    {
        protected Dictionary<int, Binding> _bindings;

        protected abstract void InitBindings();

        protected virtual void Cleanup()
        {
            
        }

        protected abstract void Init(Bundle savedInstanceState);

        protected sealed override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Init(savedInstanceState);
            InitBindings();

        }

        protected sealed override void OnDestroy()
        {
            _bindings.ForEach(pair => pair.Value.Detach());
            _bindings = null;
            Cleanup();
            base.OnDestroy();
        }
    }
}