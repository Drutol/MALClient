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
#if !ANDROID
                RaisePropertyChanged(() => GeneralInfo);
                RaisePropertyChanged(() => Officers);
                RaisePropertyChanged(() => AnimeRelations);
                RaisePropertyChanged(() => MangaRelations);
                RaisePropertyChanged(() => CharacterRelations);

#endif
    }
}

#if !ANDROID
        //Workaround for xaml being unable to bind to values of value type tuples
        public List<Tuple<string, string>> GeneralInfo => Details?.GeneralInfo
            .Select(tuple => new Tuple<string, string>(tuple.name, tuple.value)).ToList();

        public List<Tuple<string,string>> Officers => Details?.Officers
            .Select(tuple => new Tuple<string, string>(tuple.role, tuple.user)).ToList();

        public List<Tuple<string,string>> AnimeRelations => Details?.AnimeRelations
            .Select(tuple => new Tuple<string, string>(tuple.title, tuple.id)).ToList();

        public List<Tuple<string,string>> MangaRelations => Details?.MangaRelations
            .Select(tuple => new Tuple<string, string>(tuple.title, tuple.id)).ToList();

        public List<Tuple<string,string>> CharacterRelations => Details?.CharacterRelations
            .Select(tuple => new Tuple<string, string>(tuple.name, tuple.id)).ToList();
#endif

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
