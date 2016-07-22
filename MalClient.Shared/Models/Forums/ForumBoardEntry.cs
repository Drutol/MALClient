using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalClient.Shared.Models.Forums
{
    //staticly defined boards 
    public class ForumBoardEntry
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<ForumBoardEntryPeekPost> PeekPosts { get; set; } = new List<ForumBoardEntryPeekPost>(2);
    }

    //these things on the left aka most recent posts
    public class ForumBoardEntryPeekPost
    {
        public MalUser User { get; set; } = new MalUser();
        public string Title { get; set; }
        public string PostTime { get; set; }
    }
}
