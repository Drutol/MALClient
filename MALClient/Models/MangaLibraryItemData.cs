using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models
{
    public class MangaLibraryItemData : AnimeLibraryItemData
    {
        public int MyVolumes;
        public int AllVolumes;
        public string SlugId { get; set; } //manga on hummingbird does not have integer id
    }
}
