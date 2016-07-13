using System;
using Windows.UI.Xaml.Media.Imaging;
using MALClient.Comm.MagicalRawQueries;

namespace MALClient.Models.Favourites
{
    public class AnimeStaffPerson : FavouriteBase
    {
        protected override FavouriteType Type { get; } = FavouriteType.Person;
    }
}