using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MALClient.Models.Models.Forums;

namespace MALClient.XShared.ViewModels.Forums.Items
{
    public class ForumTopicMessageEntryViewModel : ViewModelBase
    {
        public ForumMessageEntry Data { get; }

        public ForumTopicMessageEntryViewModel(ForumMessageEntry data)
        {
            Data = data;
        }
    }
}
