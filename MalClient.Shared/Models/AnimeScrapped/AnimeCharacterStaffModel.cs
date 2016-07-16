using MalClient.Shared.Models.Favourites;

namespace MalClient.Shared.Models.AnimeScrapped
{
    public class AnimeCharacterStaffModel
    {
        public AnimeCharacter AnimeCharacter { get; set; } = new AnimeCharacter();
        public AnimeStaffPerson AnimeStaffPerson { get; set; } = new AnimeStaffPerson();
    }
}