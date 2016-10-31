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

        protected abstract void InitBindings();
        protected abstract void InitOneTimeBindings();
        protected Dictionary<int, Binding> Bindings { get; set; } = new Dictionary<int, Binding>();

        protected TViewModel ViewModel { get; private set; }

        public View Container
        {
            get { return _container; }
            set
            {
                Detach();
                _container = value;
                InitBindings();
                InitOneTimeBindings();
            }
        }

        protected BindingInfo(View container, TViewModel viewModel)
        {          
            ViewModel = viewModel;
            Container = container;
        }


        protected virtual void DetachBindings()
        {
            foreach (var binding in Bindings)
            {
                binding.Value?.Detach();
            }
            Bindings = new Dictionary<int, Binding>();
        }

        public void Detach()
        {
            DetachBindings();
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