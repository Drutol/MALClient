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
        public event WebViewNavigationRequest WebViewNavigationRequested;

        public void Init(ForumsTopicNavigationArgs args)
        {
            ViewModelLocator.NavMgr.RegisterBackNav(PageIndex.PageForumIndex, ViewModelLocator.ForumsBoard.PrevArgs);
            WebViewNavigationRequested?.Invoke(args.TopicId);
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
