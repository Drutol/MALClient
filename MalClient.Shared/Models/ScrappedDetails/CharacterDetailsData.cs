using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MalClient.Shared.Models.Anime;

namespace MalClient.Shared.Models.ScrappedDetails
{
    public class CharacterDetailsData
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string ImgUrl { get; set; }
        public string Content { get; set; }
        public string SpoilerContent { get; set; }
        public string TotalFavs { get; set; }
        public List<AnimeLightEntry> Animeography { get; set; } = new List<AnimeLightEntry>();
        public List<AnimeLightEntry> Mangaography { get; set; }  = new List<AnimeLightEntry>();
    }
}
