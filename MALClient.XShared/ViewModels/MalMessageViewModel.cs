using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MALClient.Models.Enums;
using MALClient.Models.Models.MalSpecific;

namespace MALClient.XShared.ViewModels
{
    public class MalMessageViewModel : ViewModelBase
    {
        public MalMessageModel Data { get; }

        public MalMessageSymbols Icon => Data.IsMine ? MalMessageSymbols.Mine : IsRead ? MalMessageSymbols.Read : MalMessageSymbols.Unread;

        private bool _isRead { get; set; }

        public bool IsRead
        {
            get { return _isRead; }
            set
            {
                _isRead = value;
                RaisePropertyChanged(() => Icon);
            }
        }
    }
}
