using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.Models.MalSpecific
{
    public class MalClubComment
    {
        public MalUser User { get; set; } = new MalUser();
        public string Date { get; set; }
        public string Content { get; set; }
        public string Id { get; set; }
    }

    public class MalClubDetails
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public bool IsPublic { get; set; }
        public string DescriptionHtml { get; set; }
        public string ImgUrl { get; set; }
        public bool Joined { get; set; }

        public List<(string user, string role)> Officers { get; set; } = new List<(string user, string role)>();
        public List<MalUser> MembersPeek { get; set; } = new List<MalUser>();
        public List<MalClubComment> RecentComments { get; set; } = new List<MalClubComment>();
        public List<(string title,string id)> AnimeRelations { get; set; } = new List<(string title, string id)>();
        public List<(string title,string id)> MangaRelations { get; set; } = new List<(string title, string id)>();
        public List<(string name,string id)> CharacterRelations { get; set; } = new List<(string name, string id)>();
        public List<(string name,string value)> GeneralInfo { get; set; } = new List<(string name, string value)>();    
    }
}
