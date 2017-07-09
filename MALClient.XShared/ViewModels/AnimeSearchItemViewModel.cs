using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;
using MALClient.Models.Models.Library;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.XShared.ViewModels
{
    public class AnimeSearchItemViewModel : ViewModelBase , IAnimeData
    {
        public readonly bool AnimeMode;
        private readonly AnimeGeneralDetailsData _item;

        public int Id { get; set; }
        public float GlobalScore { get; set; }
        public int AllEpisodes { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Notes { get; set; }
        public bool IsRewatching { get; set; }
        public string AlaternateTitle { get; }
        public int MyVolumes { get; set; }
        public int AllVolumes { get; set; }
        public string Title { get; set; }

        public string Type { get; set; }
        private string Status { get; }
        //They must be here because reasons (interface reasons)
        public int MyEpisodes { get; set; }
        public float MyScore { get; set; }
        public AnimeStatus MyStatus { get; set; }

        public string GlobalScoreBind => GlobalScore == 0 ? "N/A" : GlobalScore.ToString("N2");
        public string Synopsis { get; set; }
        public string ImgUrl { get; set; }
        public string WatchedEps => $"{(AnimeMode ? "Episodes" : "Chapters")}: {(AllEpisodes == 0 ? "?" : AllEpisodes.ToString())}";

        public ICommand NavigateDetailsCommand => new RelayCommand(NavigateDetails);

        public AnimeSearchItemViewModel(AnimeGeneralDetailsData data, bool anime = true)
        {
            _item = data;
            Id = data.Id;
            GlobalScore = data.GlobalScore;
            AllEpisodes = data.AllEpisodes;
            if (!anime)
                AllVolumes = data.AllVolumes;
            Title = data.Title;
            Type = data.Type;
            Status = data.Status;
            Synopsis = data.Synopsis;
            ImgUrl = data.ImgUrl;
            AnimeMode = anime;
            AlaternateTitle = data.AlternateTitle;
        }

        public void NavigateDetails()
        {
            if (ViewModelLocator.AnimeDetails.Id == Id)
                return;
            ViewModelLocator.GeneralMain
                    .Navigate(PageIndex.PageAnimeDetails,
                        new AnimeDetailsPageNavigationArgs(Id, Title, _item, this,
                            new SearchPageNavigationArgs
                            {
                                Query = ViewModelLocator.SearchPage.PrevQuery,
                                Anime = AnimeMode,
                                DisplayMode = ViewModelLocator.SearchPage.PrevArgs.DisplayMode
                            })
                        {
                            Source = AnimeMode ? PageIndex.PageSearch : PageIndex.PageMangaSearch,
                            AnimeMode = AnimeMode
                        });
        }
    }
}
