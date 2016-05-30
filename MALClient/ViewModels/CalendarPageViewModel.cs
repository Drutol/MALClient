using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Comm.Anime;
using MALClient.Items;

namespace MALClient.ViewModels
{
    public class CalendarPivotPage
    {
        public string Header { get; set; }
        public string Sub { get; set; }
        public string FullHeader => Utils.ShortDayToFullDay(Header); //full name
        public List<AnimeItemViewModel> Items { get; set; } = new List<AnimeItemViewModel>();
    }

    //just to indicate different data type
    public sealed class CalendarSummaryPivotPage : CalendarPivotPage
    {
        public new string Header => "Summary";
        public new string Sub => "";
        public List<Tuple<string,List<AnimeItemViewModel>>> Data { get; set; } = new List<Tuple<string, List<AnimeItemViewModel>>>();
    }


    public class CalendarPageViewModel : ViewModelBase
    {
        public ObservableCollection<CalendarPivotPage> CalendarData { get; set; } =
            new ObservableCollection<CalendarPivotPage>();

        public double _maxWidth = 0;
        public double MaxWidth => _maxWidth == 0 ? (_maxWidth = AnimeItemViewModel.MaxWidth) : _maxWidth;

        private int _calendarPivotIndex;
        public int CalendarPivotIndex
        {
            get { return _calendarPivotIndex; }
            set
            {
                _calendarPivotIndex = value;
                RaisePropertyChanged(() => CalendarPivotIndex);
            }
        }

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

        private Visibility _calendarVisibility = Visibility.Collapsed;

        public Visibility CalendarVisibility
        {
            get { return _calendarVisibility; }
            set
            {
                _calendarVisibility = value;
                RaisePropertyChanged(() => CalendarVisibility);
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

        public ICommand _refreshCalendarCommand;

        public ICommand RefreshCalendarCommand
            => _refreshCalendarCommand ?? (_refreshCalendarCommand = new RelayCommand(() => Init(true)));


        public void InitPages()
        {
            CalendarData = new ObservableCollection<CalendarPivotPage>
            {
                new CalendarPivotPage(),
                new CalendarPivotPage(),
                new CalendarPivotPage(),
                new CalendarPivotPage(),
                new CalendarPivotPage(),
                new CalendarPivotPage(),
                new CalendarPivotPage(),
                new CalendarSummaryPivotPage(),
            };
        }


        private bool _initialized;

        public async void Init(bool force = false)
        {
            if (_initialized && !force)
            {
                CalendarVisibility = Visibility.Visible;
                return;
            }
            InitPages();
            _initialized = true;
            List<AnimeItemAbstraction> idsToFetch = new List<AnimeItemAbstraction>();

            foreach (
                var abstraction in
                    ViewModelLocator.AnimeList.AllLoadedAnimeItemAbstractions.Where(
                        abstraction =>
                            (Settings.CalendarIncludePlanned && abstraction.MyStatus == (int) AnimeStatus.PlanToWatch) ||
                            (Settings.CalendarIncludeWatching && abstraction.MyStatus == (int) AnimeStatus.Watching)))
            {
                if (abstraction.AirDay != -1)
                {
                    int day = abstraction.AirDay - 1;
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
                    if(day >= 0 && day <=7)
                     CalendarData[day].Items.Add(abstraction.ViewModel);
                }
                else if (Settings.SelectedApiType == ApiType.Mal && !abstraction.LoadedVolatile)
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
                    var data =
                        await
                            new AnimeGeneralDetailsQuery().GetAnimeDetails(false, abstraction.Id.ToString(),
                                abstraction.Title, true);
                    int day;
                    try
                    {
                        day = data.StartDate != AnimeItemViewModel.InvalidStartEndDate &&
                              (string.Equals(data.Status, "Currently Airing", StringComparison.CurrentCultureIgnoreCase) ||
                               string.Equals(data.Status, "Not yet aired", StringComparison.CurrentCultureIgnoreCase))
                            ? (int) DateTime.Parse(data.StartDate).DayOfWeek + 1
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
                        CalendarData[day].Items.Add(abstraction.ViewModel);
                    }
                    ProgressValue++;
                }
            }

            if (Settings.CalendarSwitchMonSun)
            {
                CalendarData.Move(0, 6);
                CalendarData[0].Header = Utils.DayToString(DayOfWeek.Monday, true);
                CalendarData[6].Header = Utils.DayToString(DayOfWeek.Sunday, true);
                for (int i = 1; i < 6; i++)
                    CalendarData[i].Header = Utils.DayToString((DayOfWeek) i + 1, true);
            }
            else
                for (int i = 0; i < 7; i++)
                    CalendarData[i].Header = Utils.DayToString((DayOfWeek) i, true);
            List<CalendarPivotPage> emptyPages = new List<CalendarPivotPage>();
            foreach (var calendarPivotPage in CalendarData.Take(CalendarData.Count - 1))
            {
                if (calendarPivotPage.Items.Count > 0)
                    calendarPivotPage.Sub = calendarPivotPage.Items.Count.ToString();
                else
                {
                    if (Settings.CalendarRemoveEmptyDays)
                        emptyPages.Add(calendarPivotPage);
                    else
                        calendarPivotPage.Sub = "-";
                }
                if (calendarPivotPage.Items.Count != 0)
                    (CalendarData[7] as CalendarSummaryPivotPage).Data.Add(
                        new Tuple<string, List<AnimeItemViewModel>>(calendarPivotPage.FullHeader, calendarPivotPage.Items));
            }
            foreach (var emptyPage in emptyPages)
                CalendarData.Remove(emptyPage);



            RaisePropertyChanged(() => CalendarData);
            if (Settings.CalendarStartOnToday)
            {
                //we have to find it because it may have been removed
                //we will do this by comparing header string
                string today = Utils.DayToString(DateTime.Now.DayOfWeek, true);
                int index = CalendarData.Count - 1;
                for (int i = 0; i < CalendarData.Count - 1; i++)
                {
                    if (CalendarData[i].Header == today)
                    {
                        index = i;
                        break;
                    }
                }
                CalendarPivotIndex = index;
            }
            else
                CalendarPivotIndex = CalendarData.Count - 1;

            CalendarBuildingVisibility = Visibility.Collapsed;
            CalendarVisibility = Visibility.Visible;
        }
    }
}
