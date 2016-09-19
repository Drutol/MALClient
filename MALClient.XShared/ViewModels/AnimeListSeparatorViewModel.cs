using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MALClient.Models.Interfaces;

namespace MALClient.XShared.ViewModels
{
    public class AnimeListSeparatorViewModel : ViewModelBase,IAnimeListItem
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
    }
}
