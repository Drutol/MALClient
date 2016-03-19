using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Pages;
using MALClient.ViewModels;

namespace MALClient.Models
{
    public class RelatedAnimeData : IDetailsPageArgs
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string WholeRelation { get; set; }
        public RelatedItemType Type { get; set; }
    }
}
