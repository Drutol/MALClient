using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace MALClient.XShared.ViewModels.Forums.Items
{
    public class ForumTopicQestionModel
    {
        public string Answer { get; set; }
        public bool Removable { get; set; } = true;
    }
}
