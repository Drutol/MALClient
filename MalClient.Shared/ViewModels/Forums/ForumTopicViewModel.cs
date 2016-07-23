using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MalClient.Shared.Comm.Forums;
using MalClient.Shared.Delegates;
using MalClient.Shared.NavArgs;

namespace MalClient.Shared.ViewModels.Forums
{
    public class ForumTopicViewModel : ViewModelBase
    {
        public event WebViewNavigationRequest WebViewNavigationRequested;

        public async void Init(ForumsTopicNavigationArgs args)
        {
            WebViewNavigationRequested?.Invoke(args.TopicId);
        }
    }
}
