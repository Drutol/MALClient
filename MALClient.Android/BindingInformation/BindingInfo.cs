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

namespace MALClient.Android.CollectionAdapters
{
    public abstract class BindingInfo<TViewModel>
    {
        private bool _attached;
        private View _container;

        public View Container
        {
            get { return _container; }
            set
            {
                Detach();
                _container = value;
                Attach();
                InitBindings();
                Setup();
            }
        }

        public TViewModel ViewModel { get; protected set; }
        protected Dictionary<int, Binding> Bindings { get; set; } = new Dictionary<int, Binding>();

        public BindingInfo(View container, TViewModel viewModel)
        {          
            ViewModel = viewModel;
            Container = container;
        }


        protected abstract void InitBindings();
        public abstract void DetachBindings();


        protected virtual void Setup()
        {
            
        }

        private void Attach()
        {
            Container.ViewAttachedToWindow += ContainerOnViewAttachedToWindow;
            Container.ViewDetachedFromWindow += ContainerOnViewDetachedFromWindow;
        }

        public virtual void Detach()
        {
            if(!_attached)
                return;

            DetachBindings();
            Container.ViewAttachedToWindow -= ContainerOnViewAttachedToWindow;
            Container.ViewDetachedFromWindow -= ContainerOnViewDetachedFromWindow;
        }

        private void ContainerOnViewDetachedFromWindow(object sender, View.ViewDetachedFromWindowEventArgs e)
        {
            DetachBindings();
        }

        private void ContainerOnViewAttachedToWindow(object sender, View.ViewAttachedToWindowEventArgs e)
        {
            InitBindings();
        }
    }
}