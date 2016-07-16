using System.Runtime.Serialization;
using MalClient.Shared.Comm.MagicalRawQueries;

namespace MalClient.Shared.Models.Favourites
{
    [DataContract]
    public class AnimeCharacter : FavouriteBase
    {
        [DataMember]
        public string ShowId { get; set; }
        [DataMember]
        public bool FromAnime { get; set; }

        protected override FavouriteType Type { get; } = FavouriteType.Character;
    }
}