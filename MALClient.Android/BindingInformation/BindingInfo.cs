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
        private View _container;

        protected abstract void InitBindings();
        protected abstract void InitOneTimeBindings();
        protected abstract void DetachInnerBindings();
        protected Dictionary<int, List<Binding>> Bindings { get; set; } = new Dictionary<int, List<Binding>>();

        protected TViewModel ViewModel { get; private set; }

        public View Container
        {
            get { return _container; }
            set
            {
                
                Detach();
                _container = value;

                InitOneTimeBindings();
                InitBindings();
            }
        }

        protected BindingInfo(View container, TViewModel viewModel)
        {          
            ViewModel = viewModel;
            Container = container;
        }


        private void DetachBaseBindings()
        {
            foreach (var binding in Bindings.SelectMany(pair => pair.Value))
            {
                binding?.Detach();
            }
            Bindings = new Dictionary<int, List<Binding>>();
        }

        public void Detach()
        {
            DetachBaseBindings();
            DetachInnerBindings();
        }

        private void ContainerOnViewDetachedFromWindow(object sender, View.ViewDetachedFromWindowEventArgs e)
        {
            DetachBaseBindings();
        }

        private void ContainerOnViewAttachedToWindow(object sender, View.ViewAttachedToWindowEventArgs e)
        {
            InitBindings();
        }
    }
}