using MalClient.Shared.Comm.MagicalRawQueries;

namespace MalClient.Shared.Models.Favourites
{
    public class AnimeStaffPerson : FavouriteBase
    {
        protected override FavouriteType Type { get; } = FavouriteType.Person;
    }
}