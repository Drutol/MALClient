using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Models.Forums;
using FontAwesome.UWP;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.ViewModels.Forums
{
    public class ForumBoardEntryViewModel : ViewModelBase
    {
        public ForumBoardEntryViewModel(string name, string description, FontAwesomeIcon icon,ForumBoards board)
        {
            Entry = new ForumBoardEntry {Name = name, Description = description};
            Icon = icon;
            Board = board;
        }

        public ForumBoardEntry Entry { get; }   

        public FontAwesomeIcon Icon { get; }

        public ForumBoards Board { get; }

        public void SetPeekPosts(IEnumerable<ForumBoardEntryPeekPost> posts)
        {
            Entry.PeekPosts = posts;
            RaisePropertyChanged(() => Entry);
        }

        public ICommand AddToFavouritesCommand
            => new RelayCommand<ForumBoards>(board => ViewModelLocator.ForumsMain.AddFavouriteBoard(board));

    }
}
