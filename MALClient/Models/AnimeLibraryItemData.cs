using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models
{
    public class AnimeLibraryItemData
    {
        private int _id = -1;
        public int Id { get { return _id == 0 ? MalId : _id; } set { _id = value; } }
        public int MalId;
        public string Title;
        public AnimeStatus MyStatus;
        public float MyScore;
        public int MyEpisodes;
        public int AllEpisodes;
        public string ImgUrl;
        public string Type;
    }
}
