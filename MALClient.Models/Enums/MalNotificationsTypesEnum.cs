using System;

namespace MALClient.Models.Enums
{
    [Flags]
    public enum MalNotificationsTypes
    {
        [EnumUtilities.Description("All")]
        Generic =0x0,
        [EnumUtilities.Description("Friend Request")]
        FriendRequest = 0x1,
        [EnumUtilities.Description("Friend Request Result")]
        FriendRequestAcceptDeny = 0x2,
        [EnumUtilities.Description("Profile Comment")]
        ProfileComment = 0x4,
        [EnumUtilities.Description("Blog Comment")]
        BlogComment = 0x8,
        [EnumUtilities.Description("Forum Quoute")]
        ForumQuoute = 0x10,
        [EnumUtilities.Description("User Mentions")]
        UserMentions = 0x20,
        [EnumUtilities.Description("Watched Forum Topics")]
        WatchedTopics = 0x40,
        [EnumUtilities.Description("Club Messages")]
        ClubMessages = 0x80,
        [EnumUtilities.Description("New Related Anime")]
        NewRelatedAnime = 0x100,
        [EnumUtilities.Description("Now On Air")]
        NowAiring = 0x200,
        [EnumUtilities.Description("Payment")]
        Payment = 0x400,
        [EnumUtilities.Description("Messages")]
        Messages = 0x800,      
    }
}