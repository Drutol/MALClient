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
using MalClient.Shared.Utils;

namespace MalClient.Shared.ViewModels.Forums
{
    public class ForumBoardViewModel : ViewModelBase
    {
        public ObservableCollection<ForumTopicEntryViewModel> _topics;

        public ObservableCollection<ForumTopicEntryViewModel> Topics
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
            Topics =
                new ObservableCollection<ForumTopicEntryViewModel>(
                    (await new ForumBoardTopicsQuery(args.TargetBoard).GetTopicPosts()).Select(
                        entry => new ForumTopicEntryViewModel(entry)));
            Title = args.TargetBoard.GetDescription();
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }
    }
}
