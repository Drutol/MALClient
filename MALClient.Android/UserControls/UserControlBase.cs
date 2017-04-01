using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;

namespace MALClient.Android.UserControls
{
    public abstract class UserControlBase<TViewModel,TViewRootType> : FrameLayout where TViewRootType : ViewGroup
    {
        protected readonly List<Binding> Bindings = new List<Binding>();

        protected TViewRootType RootContainer;
        protected TViewModel ViewModel { get; private set; }

        #region Constructors
        protected UserControlBase(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Init();
        }

        protected UserControlBase(Context context) : base(context)
        {
            Init();
        }

        protected UserControlBase(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        protected UserControlBase(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init();
        }

        protected UserControlBase(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init();
        }
        #endregion

        protected abstract int ResourceId { get; }

        public void BindModel(TViewModel model,bool fling)
        {
            foreach (var binding in Bindings)
                binding.Detach();
            Bindings.Clear();

            bool firstRun = ViewModel == null;
            ViewModel = model;
            if (firstRun)
                RootContainerInit();
            BindModelBasic();
            if(fling)
                BindModelFling();
            else
                BindModelFull();
        }

        protected abstract void BindModelFling();
        protected abstract void BindModelFull();
        protected abstract void BindModelBasic();
        protected abstract void RootContainerInit();

        private void Init()
        {
            ((LayoutInflater) Context
                .GetSystemService(Context.LayoutInflaterService)).Inflate(ResourceId, this);
            RootContainer = GetChildAt(0) as TViewRootType;
        }

    }
}