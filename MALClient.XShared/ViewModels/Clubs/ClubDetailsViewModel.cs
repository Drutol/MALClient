using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MALClient.Models.Models.MalSpecific;
using MALClient.XShared.Comm.MagicalRawQueries.Clubs;
using MALClient.XShared.NavArgs;

namespace MALClient.XShared.ViewModels.Clubs
{
    public class ClubDetailsViewModel : ViewModelBase
    {
        private bool _loading;
        private ClubDetailsPageNavArgs _lastArgs;
        private MalClubDetails _details;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged();
            }
        }

        public MalClubDetails Details
        {
            get { return _details; }
            set
            {
                _details = value;
                RaisePropertyChanged();
            }
        }

        public async void NavigatedTo(ClubDetailsPageNavArgs args)
        {
            //if(args.Equals(_lastArgs))
            //    return;

            _lastArgs = args;

            Loading = true;
            Details = await MalClubDetailsQuery.GetClubDetails(args.Id);
            Loading = false;

        }
    }
}
