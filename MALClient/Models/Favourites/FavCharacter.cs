using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.Favourites
{
    public class FavCharacter
    {
        public string Name { get; set; }
        /// <summary>
        /// Originating show.
        /// </summary>
        public string Origin { get; set; }
        public string ImgUrl { get; set; }
    }
}
