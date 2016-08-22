using System.Runtime.Serialization;
using MalClient.Shared.Comm.MagicalRawQueries;

namespace MalClient.Shared.Models.Favourites
{
    public class AnimeCharacter : FavouriteBase
    {
        public string ShowId { get; set; }
        public bool FromAnime { get; set; }

        public override FavouriteType Type { get; } = FavouriteType.Character;
    }
}