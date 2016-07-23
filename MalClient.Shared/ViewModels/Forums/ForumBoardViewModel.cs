using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MalClient.Shared.Comm.Forums;
using MalClient.Shared.Models.Forums;
using MalClient.Shared.NavArgs;

namespace MalClient.Shared.ViewModels.Forums
{
    public class ForumBoardViewModel : ViewModelBase
    {
        public ObservableCollection<ForumTopicEntry> _topics;

        public ObservableCollection<ForumTopicEntry> Topics
        {
            get { return _topics; }
            set
            {
                _topics = value;
                RaisePropertyChanged(() => Topics);
            }
        }

        public async void Init(ForumsBoardNavigationArgs args)
        {
            Topics = new ObservableCollection<ForumTopicEntry>(await new ForumBoardTopicsQuery(args.TargetBoard).GetTopicPosts());
        }
    }
}
