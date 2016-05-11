using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using MALClient.Comm;
using MALClient.Comm.Anime;
using MALClient.Items;

namespace MALClient.ViewModels
{
    public class CalendarPageViewModel : ViewModelBase
    {
        public List<List<AnimeItemViewModel>> CalendarData { get; set; } = new List<List<AnimeItemViewModel>>
        {
            new List<AnimeItemViewModel>(),
            new List<AnimeItemViewModel>(),
            new List<AnimeItemViewModel>(),
            new List<AnimeItemViewModel>(),
            new List<AnimeItemViewModel>(),
            new List<AnimeItemViewModel>(),
            new List<AnimeItemViewModel>(),
        };

        private Visibility _calendarBuildingVisibility = Visibility.Collapsed;

        public Visibility CalendarBuildingVisibility
        {
            get { return _calendarBuildingVisibility; }
            set
            {
                _calendarBuildingVisibility = value;
                RaisePropertyChanged(() => CalendarBuildingVisibility);
            }
        }

        private int _progressValue;

        public int ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                RaisePropertyChanged(() => ProgressValue);
            }
        }

        private int _maxProgressValue;

        public int MaxProgressValue
        {
            get { return _maxProgressValue; }
            set
            {
                _maxProgressValue = value;
                RaisePropertyChanged(() => MaxProgressValue);
            }
        }


        private bool _initialized;
        public async void Init(bool force = false)
        {
            if(_initialized && !force)
                return;
            _initialized = true;
            List<AnimeItemAbstraction> idsToFetch = new List<AnimeItemAbstraction>();

            foreach (
                var abstraction in
                    ViewModelLocator.AnimeList.AllLoadedAnimeItemAbstractions.Where(
                        abstraction =>
                            abstraction.MyStatus == (int) AnimeStatus.PlanToWatch ||
                            abstraction.MyStatus == (int) AnimeStatus.Watching))
            {
                if (abstraction.AirDay != -1)
                {
                    int day = abstraction.AirDay-1;
                    if (Settings.AirDayOffset != 0)
                    {
                        var sum = Settings.AirDayOffset + day;
                        if (sum > 6)
                            day = sum - 7;
                        else if (sum < 0)
                            day = 7 + sum;
                        else
                            day += Settings.AirDayOffset;
                    }
                    CalendarData[day].Add(abstraction.ViewModel);
                }
                else if(Settings.SelectedApiType == ApiType.Mal && !abstraction.LoadedVolatile)
                {
                    idsToFetch.Add(abstraction);
                }
            }
            if (idsToFetch.Count > 0)
            {
                CalendarBuildingVisibility = Visibility.Visible;
                MaxProgressValue = idsToFetch.Count;
                foreach (var abstraction in idsToFetch)
                {
                    var data = await new AnimeGeneralDetailsQuery().GetAnimeDetails(false, abstraction.Id.ToString(), abstraction.Title, true);
                    int day = 0;
                    try
                    {
                        day = data.StartDate != AnimeItemViewModel.InvalidStartEndDate &&
                              (string.Equals(data.Status, "Currently Airing", StringComparison.CurrentCultureIgnoreCase) ||
                               string.Equals(data.Status, "Not yet aired", StringComparison.CurrentCultureIgnoreCase))
                            ? (int)DateTime.Parse(data.StartDate).DayOfWeek + 1
                            : -1;
                    }
                    catch (Exception)
                    {
                        day = -1;
                    }

                    DataCache.RegisterVolatileData(abstraction.Id, new VolatileDataCache
                    {
                        DayOfAiring = day,
                        GlobalScore = data.GlobalScore,
                        AirStartDate = data.StartDate == AnimeItemViewModel.InvalidStartEndDate ? null : data.StartDate
                    });
                    if (day != -1)
                    {
                        abstraction.AirDay = day;
                        abstraction.GlobalScore = data.GlobalScore;
                        abstraction.ViewModel.UpdateVolatileData();
                        day--;
                        if (Settings.AirDayOffset != 0)
                        {
                            var sum = Settings.AirDayOffset + day;
                            if (sum > 6)
                                day = sum - 7;
                            else if (sum < 0)
                                day = 7 + sum;
                            else
                                day += Settings.AirDayOffset;
                        }
                        CalendarData[day].Add(abstraction.ViewModel);
                    }
                    ProgressValue++;
                }
            }

            RaisePropertyChanged(() => CalendarData);
            CalendarBuildingVisibility = Visibility.Collapsed;
        }
    }
}
