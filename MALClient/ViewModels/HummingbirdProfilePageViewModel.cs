using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MALClient.Comm;
using MALClient.Items;
using MALClient.Models.ApiResponses;

namespace MALClient.ViewModels
{
    public class HummingbirdProfilePageViewModel : ViewModelBase
    {
        public HumProfileData CurrentData { get; set; } = new HumProfileData();
        public List<HumStoryObject> FeedData { get; set; } = new List<HumStoryObject>();
        public List<HumStoryObject> SocialFeedData { get; set; } = new List<HumStoryObject>();

        public ObservableCollection<AnimeItemViewModel> FavAnime { get; } = new ObservableCollection<AnimeItemViewModel>();

        private bool _loaded;
        public async void Init(bool force = false)
        {
            if (!_loaded || force)
            {
                FavAnime.Clear();
                CurrentData = await new ProfileQuery().GetHumProfileData();
                foreach (var fav in CurrentData.favorites)
                {
                    var data = await ViewModelLocator.AnimeList.TryRetrieveAuthenticatedAnimeItem(fav.item_id);
                    if (data != null)
                    {
                        FavAnime.Add(data as AnimeItemViewModel);
                    }
                }
                RaisePropertyChanged(() => CurrentData);
                FeedData = await new ProfileQuery(true).GetHumFeedData();
                SocialFeedData = FeedData.Where(o => o.story_type == "comment").ToList();
                FeedData = FeedData.Where(o => o.story_type == "media_story").ToList();

                RaisePropertyChanged(() => FeedData);
            }    
        }
    }
}
