using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalClient.Shared.Models.AnimeScrapped
{
    public class AnimeScrappedDetails
    {
        public int  Id { get; set; }
        public List<string> AlternativeTitles { get; } = new List<string>();
        public List<string> Information { get;  } = new List<string>();
        public List<string> Statistics { get; } = new List<string>();
        public List<string> Openings { get;  } = new List<string>();
        public List<string> Endings { get; } = new List<string>();
    }
}
