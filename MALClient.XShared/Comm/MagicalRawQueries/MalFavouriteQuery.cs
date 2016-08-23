using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MALClient.Models.Enums;

namespace MALClient.XShared.Comm.MagicalRawQueries
{
    public static class MalFavouriteQuery
    {
        public static async Task ModifyFavourite(int id, FavouriteType type, bool add)
        {
            string idFieldName;
            string actionId;
            switch (type)
            {
                case FavouriteType.Anime:
                    idFieldName = "aid";
                    actionId = add ? "13" : "14";
                    break;
                case FavouriteType.Manga:
                    idFieldName = "mid";
                    actionId = add ? "38" : "39";
                    break;
                case FavouriteType.Character:
                    idFieldName = "cid";
                    actionId = add ? "42" : "43";
                    break;
                case FavouriteType.Person:
                    idFieldName = "vaid";
                    actionId = add ? "47" : "48";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            try
            {
                var client = await MalHttpContextProvider.GetHttpContextAsync();

                var charCont = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(idFieldName, id.ToString()),
                    new KeyValuePair<string, string>("csrf_token", client.Token)
                };
                var contentchar = new FormUrlEncodedContent(charCont);
                await client.PostAsync($"/includes/ajax.inc.php?s=1&t={actionId}", contentchar);
            }
            catch (WebException)
            {
                MalHttpContextProvider.ErrorMessage("Favourites");
            }

        }
    }
}