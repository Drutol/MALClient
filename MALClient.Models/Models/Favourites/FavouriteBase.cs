namespace MALClient.Models.Models.Favourites
{
    public abstract class FavouriteBase
    {
        public string Name { get; set; }
        public string Notes { get; set; } //show name, role etc. filled diffrently depending on context
        public string ImgUrl { get; set; }
        public string Id { get; set; }

        public abstract FavouriteType Type { get; }
    }
}
