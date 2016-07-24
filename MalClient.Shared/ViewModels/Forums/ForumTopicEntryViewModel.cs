using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontAwesome.UWP;
using GalaSoft.MvvmLight;
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
    }
}
