using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Items;
using MALClient.Pages;

namespace MALClient.ViewModels
{
    public class AnimeGridItemViewModel : ViewModelBase , IAnimeData
    {
        private AnimeItemAbstraction _parentAbstraction;

        public int Id
        {
            get { return _parentAbstraction.Id; }
            set { }
        }

        public int MyScore
        {
            get { return _parentAbstraction.MyScore; }
            set { RaisePropertyChanged(() => MyScore); }
        }

        public int MyEpisodes
        {
            get { return _parentAbstraction.MyEpisodes; }
            set { RaisePropertyChanged(() => MyEpisodes); }
        }

        public int MyStatus
        {
            get { return _parentAbstraction.MyStatus; }
            set { RaisePropertyChanged(() => MyStatus); }
        }

        public string Title
        {
            get { return _parentAbstraction.Title; }
            set { }
        }

        public static double MaxWidth { get; set; }

        public float GlobalScore { get; set; }
        public int AllEpisodes { get; }
        public string Img => _parentAbstraction.img;

        private ICommand _navigateDetailsCommand;

        public ICommand NavigateDetailsCommand
        {
            get { return _navigateDetailsCommand ?? (_navigateDetailsCommand = new RelayCommand(NavigateDetails)); }
        }

        //unused
        public int MyVolumes { get; set; }
        public int AllVolumes { get; }

        public AnimeGridItemViewModel(AnimeItemAbstraction parent)
        {
            _parentAbstraction = parent;

        }

        static AnimeGridItemViewModel()
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            MaxWidth = bounds.Width * scaleFactor / 2.1;
        }

        public async void NavigateDetails()
        {
            await ViewModelLocator.Main
                .Navigate(PageIndex.PageAnimeDetails,
                    new AnimeDetailsPageNavigationArgs(Id, Title, null, this,
                        Utils.GetMainPageInstance().GetCurrentListOrderParams())
                    {
                        Source = _parentAbstraction.RepresentsAnime ? PageIndex.PageAnimeList : PageIndex.PageMangaList,
                        AnimeMode = _parentAbstraction.RepresentsAnime
                    });
        }
    }
}
