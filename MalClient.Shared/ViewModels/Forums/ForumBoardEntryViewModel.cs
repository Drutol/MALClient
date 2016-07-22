using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Models.Forums;
using FontAwesome.UWP;

namespace MalClient.Shared.ViewModels.Forums
{
    public class ForumBoardEntryViewModel : ViewModelBase
    {
        public ForumBoardEntryViewModel(string name, string description, FontAwesomeIcon icon)
        {
            Entry = new ForumBoardEntry {Name = name, Description = description};
            Icon = icon;
        }

        public ForumBoardEntry Entry { get; }

        public string Group { get; }

        public FontAwesomeIcon Icon { get; }

        public void SetPeekPosts(IEnumerable<ForumBoardEntryPeekPost> posts)
        {
            Entry.PeekPosts = posts;
            RaisePropertyChanged(() => Entry);
        }

    }
}
