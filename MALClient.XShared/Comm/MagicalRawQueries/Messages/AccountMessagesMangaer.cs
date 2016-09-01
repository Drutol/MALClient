using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MALClient.Models.Models.MalSpecific;

namespace MALClient.XShared.Comm.MagicalRawQueries.Messages
{
    public static class AccountMessagesManager
    {
        private static int MaxPage { get; set; } = 9999;

        public static async Task<List<MalMessageModel>> GetMessagesAsync(int page)
        {
            if (page >= MaxPage)
                throw new ArgumentOutOfRangeException();

            var output = await new MalMessagesQuery().GetMessages(page);

            if (output.Count != 0)
                return output;

            MaxPage = page;
            throw new ArgumentOutOfRangeException();
        }

        public static async Task<List<MalMessageModel>> GetSentMessagesAsync()
        {
            return await new MalMessagesQuery().GetSentMessages();
        }
    }
}