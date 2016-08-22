namespace MALClient.Models.Models.Favourites
{
    public class AnimeCharacter : FavouriteBase
    {
        public string ShowId { get; set; }
        public bool FromAnime { get; set; }

        public override FavouriteType Type { get; } = FavouriteType.Character;
    }
}