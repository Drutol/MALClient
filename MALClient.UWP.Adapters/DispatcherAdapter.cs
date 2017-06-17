using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using MALClient.Adapters;

namespace MALClient.UWP.Adapters
{
    public class DispatcherAdapter : IDispatcherAdapter
    {
        private Grid _obj;

        public DispatcherAdapter()
        {
            _obj = new Grid();
        }


        public async void Run(Action action)
        {
            await _obj.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, action.Invoke);
        }
    }
}
