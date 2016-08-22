using GalaSoft.MvvmLight;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;

namespace MALClient.XShared.ViewModels.Forums
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
