using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using MalClient.Shared.Comm;
using MalClient.Shared.Models.MalSpecific;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils;

namespace MalClient.Shared.ViewModels.Main
{
    public class HistoryViewModel : ViewModelBase
    {
        public Dictionary<string, List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>>> _history;

        public Dictionary<string, List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>>> History
        {
            get { return _history; }
            set
            {
                _history = value;
                RaisePropertyChanged(() => History);
            }
        }

        private Visibility _loadingVisibility;

        public Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                RaisePropertyChanged(() => LoadingVisibility);
            }
        }

        private HistoryNavigationArgs _prevArgs;

        public async void Init(HistoryNavigationArgs args,bool force = false)
        {
            if (args == null && force)
                args = _prevArgs;

            args = args ?? new HistoryNavigationArgs { Source = Credentials.UserName };

            if (!force &&_prevArgs?.Source == args.Source )
                return;
        
            _prevArgs = args;
            History = null;

            LoadingVisibility = Visibility.Visible;
            Dictionary<string, List<MalProfileHistoryEntry>> history = null;
            await Task.Run(async () => history = await new ProfileHistoryQuery(args.Source).GetProfileHistory());

            var data = new Dictionary<string, List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>>>();


            if (args.Source == Credentials.UserName)
            {
                foreach (var key in history.Keys)
                {
                    List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>> entries = new List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>>();
                    var distinctIds = history[key].Select(entry => entry.Id).Distinct();
                    foreach (var distinctId in distinctIds)
                    {
                        var vm = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(distinctId,
                            history[key].First(entry => entry.Id == distinctId).IsAnime) as AnimeItemViewModel;

                        entries.Add(new Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>(vm, history[key].Where(entry => entry.Id == distinctId).ToList()));
                    }
                    data.Add(key, entries);
                }
            }
            else
            {
                var others = ViewModelLocator.GeneralProfile.OthersAbstractions[args.Source];
                foreach (var key in history.Keys)
                {
                    List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>> entries = new List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>>();
                    var distinctIds = history[key].Select(entry => entry.Id).Distinct();
                    foreach (var distinctId in distinctIds)
                    {
                        bool anime =
                            history[key].
                                First(entry => entry.Id == distinctId).IsAnime;
                        var vm = anime
                            ? others.Item1.FirstOrDefault(abstraction => abstraction.Id == distinctId)
                            : others.Item2.FirstOrDefault(abstraction => abstraction.Id == distinctId);

                        if (vm != null)
                        {
                            entries.Add(new Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>(vm.ViewModel, history[key].Where(entry => entry.Id == distinctId).ToList()));
                        }                      
                    }
                    data.Add(key, entries);
                }
            }

            History = data;
            LoadingVisibility = Visibility.Collapsed;
        }
    }
}
