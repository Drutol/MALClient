using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models
{
    public enum MalNewsType
    {
        Article,
        News,
    }

    public class MalNewsUnitModel
    {
        public string ImgUrl { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Highlight { get; set; }
        public string Author { get; set; }
        public string Views { get; set; }
        public string Tags { get; set; }
        public MalNewsType Type { get; set; }
    }
}
