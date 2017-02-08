using System.Collections.Generic;
using System.Linq;
using Android.Views;
using GalaSoft.MvvmLight.Helpers;

namespace MALClient.Android.BindingInformation
{
    public abstract class BindingInfo<TViewModel>
    {
        private View _container;

        private bool _initialized;
        protected abstract void InitBindings();
        protected abstract void InitOneTimeBindings();
        protected abstract void DetachInnerBindings();
        protected Dictionary<int, List<Binding>> Bindings { get; set; } = new Dictionary<int, List<Binding>>();

        protected TViewModel ViewModel { get; private set; }
        public bool Fling { get; set; }

        public View Container
        {
            get { return _container; }
            set
            {
                
                Detach();
                _container = value;
                if(_initialized)
                    PrepareContainer();
            }
        }

        protected BindingInfo(View container, TViewModel viewModel,bool fling)
        {          
            ViewModel = viewModel;
            Fling = fling;
            Container = container;
            _initialized = true;
        }

        protected void PrepareContainer()
        {
            InitOneTimeBindings();
            InitBindings();
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