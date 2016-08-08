using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using MalClient.Shared.Comm.Forums;
using MalClient.Shared.Delegates;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.ViewModels.Forums
{
    public class ForumTopicViewModel : ViewModelBase
    {
        public event WebViewNavigationRequest WebViewTopicNavigationRequested;
        public event WebViewNavigationRequest WebViewNewTopicNavigationRequested;

        public void Init(ForumsTopicNavigationArgs args)
        {
            LoadingTopic = Visibility.Visible;
            if (args.CreateNewTopic)
            {
                WebViewNewTopicNavigationRequested?.Invoke(((int)args.SourceBoard).ToString(),false);
            }
            else
            {
                WebViewTopicNavigationRequested?.Invoke(args.TopicId, args.Lastpost);
            }

        }

        private Visibility _loadingTopic;

        public Visibility LoadingTopic
        {
            get { return _loadingTopic; }
            set
            {
                _loadingTopic = value;
                RaisePropertyChanged(() => LoadingTopic);
            }
        }
    }
}
