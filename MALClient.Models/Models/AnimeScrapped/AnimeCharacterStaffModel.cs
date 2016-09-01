using MALClient.Models.Models.Favourites;

namespace MALClient.Models.Models.AnimeScrapped
{
    public class AnimeCharacterStaffModel
    {
        public AnimeCharacter AnimeCharacter { get; set; } = new AnimeCharacter();
        public AnimeStaffPerson AnimeStaffPerson { get; set; } = new AnimeStaffPerson();
    }
}