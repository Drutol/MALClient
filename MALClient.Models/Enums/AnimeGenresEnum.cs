using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Enums.Enums;
// ReSharper disable InconsistentNaming

namespace MALClient.Models.Enums
{
    public enum AnimeGenres
    {
        Action = 1,
        Adventure = 2,
        Cars = 3,
        Comedy = 4,
        Dementia = 5,
        Demons = 6,
        Drama = 8,
        Ecchi = 9,
        Fantasy = 10,
        Game = 11,
        Harem = 35,
        Historical = 13,
        Horror = 14,
        Josei = 43,
        Kids = 15,
        Magic = 16,
        [EnumUtilities.Description("Martial Arts")]
        Martial_Arts = 17,
        Mecha = 18,
        Military = 38,
        Music = 19,
        Mystery = 7,
        Parody = 20,
        Police = 39,
        Psychological = 40,
        Romance = 22,
        Samurai = 21,
        School = 23,
        [EnumUtilities.Description("Sci-Fi")]
        Sci_Fi = 24,
        Seinen = 42,
        Shoujo = 25,
        [EnumUtilities.Description("Shoujo Ai")]
        Shoujo_Ai = 26,
        Shounen = 27,
        [EnumUtilities.Description("Shounen Ai")]
        Shounen_Ai = 28,
        [EnumUtilities.Description("Slice of Life")]
        Slice_of_Life = 36,
        Space = 29,
        Sports = 30,
        [EnumUtilities.Description("Super Power")]
        Super_Power = 31,
        Supernatural = 37,
        Thriller = 41,
        Vampire = 32,
        Yaoi = 33,
        Yuri = 34,
    }
}
