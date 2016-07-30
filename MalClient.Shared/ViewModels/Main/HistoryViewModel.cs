using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MalClient.Shared.Comm;
using MalClient.Shared.Models.MalSpecific;

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

        private bool _initialized;

        public async void Init(bool force = false)
        {
            if(_initialized && !force)
                return;
            _initialized = true;

            var history = await new ProfileHistoryQuery().GetProfileHistory();

            var data = new Dictionary<string, List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>>>();

            foreach (var key in history.Keys)
            {
                List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>> entries = new List<Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>>();
                var distinctIds = history[key].Select(entry => entry.Id).Distinct();
                foreach (var distinctId in distinctIds)
                {
                    var vm = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(distinctId,
                        history[key].First(entry => entry.Id == distinctId).IsAnime) as AnimeItemViewModel;

                    entries.Add(new Tuple<AnimeItemViewModel, List<MalProfileHistoryEntry>>(vm,history[key].Where(entry => entry.Id == distinctId).ToList()));
                }
                data.Add(key, entries);
            }

            History = data;
        }
    }
}
