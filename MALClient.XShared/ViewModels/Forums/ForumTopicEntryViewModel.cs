using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FontAwesome.UWP;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MalClient.Shared.Models.Forums;

namespace MalClient.Shared.ViewModels.Forums
{
    public class ForumTopicEntryViewModel : ViewModelBase
    {
        
        public ForumTopicEntry Data { get; set; }

        public ForumTopicEntryViewModel(ForumTopicEntry data)
        {
            Data = data;
        }

        public FontAwesomeIcon FontAwesomeIcon => TypeToIcon(Data.Type);

        private FontAwesomeIcon TypeToIcon(string type)
        {
            switch (type)
            {
                case "Poll:  ":
                    return FontAwesomeIcon.BarChart;
                case "Sticky:":
                    return FontAwesomeIcon.StickyNote;         
            }
            return FontAwesomeIcon.None;
        }

        private ICommand _pinCommand;

        public ICommand PinCommand => _pinCommand ?? (_pinCommand = new RelayCommand(() =>
        {
            Pin(false);
        }));

        private ICommand _pinLastpostCommand;

        public ICommand PinLastpostCommand => _pinLastpostCommand ?? (_pinLastpostCommand = new RelayCommand(() =>
        {
            Pin(true);
        }));

        private void Pin(bool lastpost)
        {
            var topic = ForumTopicLightEntry.FromTopicEntry(Data);
            topic.Lastpost = lastpost;
            topic.SourceBoard = ViewModelLocator.ForumsBoard.PrevArgs.TargetBoard;
            ViewModelLocator.ForumsMain.PinnedTopics.Add(topic);
        }
    }
}
