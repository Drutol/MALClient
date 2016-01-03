using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Comm
{
    public static class UriBuilder
    {
        public static string GetUri(UriType type,object parameters)
        {
            string uri;
            switch (type)
            {
                case UriType.AnimeList:
                    AnimeListParameters args = parameters as AnimeListParameters;                    
                    uri = $"http://myanimelist.net/malappinfo.php?u={args.user}&status={args.status}&type={args.type}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return uri;
        }
    }
}
