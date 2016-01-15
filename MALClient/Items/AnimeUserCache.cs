using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MALClient.Items
{
    public class AnimeUserCache
    {
        public List<AnimeItem> LoadedAnime { get; set; }
        public List<XElement> DownloadedAnime { get; set; }
        public DateTime LastUpdate { get; set; }
        public Dictionary<int, bool> LoadedStatus { get; set; }

        public void AnimeItemLoaded(AnimeItem item)
        {
            //Remove from downloaded
            try
            {
                DownloadedAnime.Remove(DownloadedAnime.First(element => element.Element("id").Value == item.Id.ToString()));
            }
            catch (Exception)
            {
               //ignored
            }
            
            //Add to loaded
            LoadedAnime.Add(item);
        }
    }
}
