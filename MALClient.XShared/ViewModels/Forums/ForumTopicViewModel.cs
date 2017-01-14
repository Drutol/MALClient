using GalaSoft.MvvmLight;
using MALClient.Models.Enums;
using MALClient.XShared.Delegates;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;

namespace MALClient.XShared.ViewModels.Forums
{
    public class ForumTopicViewModel : ViewModelBase
    {
        public event WebViewNavigationRequest WebViewTopicNavigationRequested;
        public event WebViewNavigationRequest WebViewNewTopicNavigationRequested;
        public event WebViewNavigationRequest WebViewNewAnimeMangaTopicNavigationRequested;

        public ForumBoards? CurrentBoard { get; set; }

        public bool IsMangaBoard
            =>
                CurrentBoard == null || CurrentBoard.Value == ForumBoards.MangaDisc ||
                CurrentBoard.Value == ForumBoards.MangaSeriesDisc;


        public void Init(ForumsTopicNavigationArgs args)
        {
            LoadingTopic = true;
            
            if (args.CreateNewTopic)
            {
                if (args.CreateNewAnimeTopic == null)
                    WebViewNewTopicNavigationRequested?.Invoke(((int) args.SourceBoard).ToString(), false);
                else
                    WebViewNewAnimeMangaTopicNavigationRequested?.Invoke(
                        $"{(args.CreateNewAnimeTopic == true ? "anime_id" : "manga_id")}={args.MediaId}", false);
            }
            else
            {
                CurrentBoard = args.SourceBoard;
                WebViewTopicNavigationRequested?.Invoke(args.TopicId, args.Lastpost);
            }

        }

        private bool _loadingTopic;

        public bool LoadingTopic
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
