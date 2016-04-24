using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Comm;
using MALClient.Models;
using Newtonsoft.Json;

namespace MALClient.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        private ICommand _reviewCommand;

        public ICommand ReviewCommand => _reviewCommand ?? (_reviewCommand = new RelayCommand(async () =>
        {
            Settings.RatePopUpEnable = false;
            await
                Launcher.LaunchUriAsync(
                    new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
        }));

        public ObservableCollection<Tuple<AnimeListDisplayModes, string>> DisplayModes { get; } = new ObservableCollection
            <Tuple<AnimeListDisplayModes, string>>
        {
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.PivotPages, "Pages"),
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteList, "List"),
            new Tuple<AnimeListDisplayModes, string>(AnimeListDisplayModes.IndefiniteGrid, "Grid")
        };

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForWatching
        {
            get { return DisplayModes[(int) Settings.WatchingDisplayMode]; }
            set { Settings.WatchingDisplayMode = value.Item1; }
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForCompleted
        {
            get { return DisplayModes[(int) Settings.CompletedDisplayMode]; }
            set { Settings.CompletedDisplayMode = value.Item1; }
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForOnHold
        {
            get { return DisplayModes[(int) Settings.OnHoldDisplayMode]; }
            set { Settings.OnHoldDisplayMode = value.Item1; }
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForDropped
        {
            get { return DisplayModes[(int) Settings.DroppedDisplayMode]; }
            set { Settings.DroppedDisplayMode = value.Item1; }
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForPlanned
        {
            get { return DisplayModes[(int) Settings.PlannedDisplayMode]; }
            set { Settings.PlannedDisplayMode = value.Item1; }
        }

        public Tuple<AnimeListDisplayModes, string> SelectedDefaultViewForAll
        {
            get { return DisplayModes[(int) Settings.AllDisplayMode]; }
            set { Settings.AllDisplayMode = value.Item1; }
        }

        public bool LockDisplayMode
        {
            get { return Settings.LockDisplayMode; }
            set { Settings.LockDisplayMode = value; }
        }

        public bool HideFilterSelectionFlyout
        {
            get { return Settings.HideFilterSelectionFlyout; }
            set { Settings.HideFilterSelectionFlyout = value; }
        }

        public bool HideViewSelectionFlyout
        {
            get { return Settings.HideViewSelectionFlyout; }
            set { Settings.HideViewSelectionFlyout = value; }
        }

        public bool HideSortingSelectionFlyout
        {
            get { return Settings.HideSortingSelectionFlyout; }
            set { Settings.HideSortingSelectionFlyout = value; }
        }

        public bool RatePopUpEnable
        {
            get { return Settings.RatePopUpEnable; }
            set
            {
                Settings.RatePopUpEnable = value;
                RaisePropertyChanged(() => RatePopUpEnable);
            }
        }

        public int RatePopUpStartupCounter
        {
            get { return RateReminderPopUp.LaunchThresholdValue - Settings.RatePopUpStartupCounter; }
        }

        public bool DataSourceAnn
        {
            get { return Settings.PrefferedDataSource == DataSource.Ann; }
            set { if (value) Settings.PrefferedDataSource = DataSource.Ann; }
        }

        public bool DataSourceHum
        {
            get { return Settings.PrefferedDataSource == DataSource.Hummingbird; }
            set { if (value) Settings.PrefferedDataSource = DataSource.Hummingbird; }
        }

        public bool DataSourceAnnHum
        {
            get { return Settings.PrefferedDataSource == DataSource.AnnHum; }
            set { if (value) Settings.PrefferedDataSource = DataSource.AnnHum; }
        }

        public static bool SetStartDateOnWatching
        {
            get { return Settings.SetStartDateOnWatching; }
            set { Settings.SetStartDateOnWatching = value; }
        }

        public static bool SetStartDateOnListAdd
        {
            get { return Settings.SetStartDateOnListAdd; }
            set { Settings.SetStartDateOnListAdd = value; }
        }

        public static bool SetEndDateOnDropped
        {
            get { return Settings.SetEndDateOnDropped; }
            set { Settings.SetEndDateOnDropped = value; }
        }

        public static bool SetEndDateOnCompleted
        {
            get { return Settings.SetEndDateOnCompleted; }
            set { Settings.SetEndDateOnCompleted = value; }
        }

        public static bool OverrideValidStartEndDate
        {
            get { return Settings.OverrideValidStartEndDate; }
            set { Settings.OverrideValidStartEndDate = value; }
        }

        public List<NewsData> CurrentNews { get; set; } = new List<NewsData>();

        private bool _newsLoaded;
        public async void LoadNews()
        {
            if (_newsLoaded)
                return;
            _newsLoaded = true;

            List<NewsData> data = new List<NewsData>();
            try
            {
                await
                    Task.Run(
                        async () =>
                            data =
                                JsonConvert.DeserializeObject<List<NewsData>>(
                                    await new NewsQuery().GetRequestResponse(false)));
            }
            catch (Exception e)
            {
                return;
            }

            CurrentNews = data;
            RaisePropertyChanged(() => CurrentNews);
        }
    }
}