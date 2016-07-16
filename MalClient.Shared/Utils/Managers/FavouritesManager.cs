using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using MalClient.Shared.Comm;
using MalClient.Shared.Comm.MagicalRawQueries;

namespace MalClient.Shared.Utils.Managers
{
    public static class FavouritesManager
    {
        private static bool _changedSth;
        private static Dictionary<FavouriteType,List<string>> KnownFavourites { get; set; }

        public static async void LoadData()
        {
            if (Settings.SyncFavsFromTimeToTime && Credentials.Authenticated && Utilities.ConvertToUnixTimestamp(DateTime.Now) - Settings.LastFavTimeSync > 36000)
            {
                KnownFavourites = new Dictionary<FavouriteType, List<string>>();
                for (int i = 0; i <= (int)FavouriteType.Person; i++) //ensure we have no null fields
                {
                    if (!KnownFavourites.ContainsKey((FavouriteType)i))
                        KnownFavourites[(FavouriteType)i] = new List<string>();
                }
                await new ProfileQuery().GetProfileData(true, true);
                Settings.LastFavTimeSync = Utilities.ConvertToUnixTimestamp(DateTime.Now);
            }
            else
            {
                KnownFavourites = await DataCache.RetrieveData<Dictionary<FavouriteType, List<string>>>("favourites", ApplicationData.Current.RoamingFolder, -1) ??
                  new Dictionary<FavouriteType, List<string>>();
                for (int i = 0; i <= (int)FavouriteType.Person; i++) //ensure we have no null fields
                {
                    if (!KnownFavourites.ContainsKey((FavouriteType)i))
                        KnownFavourites[(FavouriteType)i] = new List<string>();
                }
            }

        }

        public static async Task SaveData()
        {
            if(_changedSth)
                await DataCache.SaveData(KnownFavourites, "favourites", ApplicationData.Current.RoamingFolder);
        }

        public static void ForceNewSet(FavouriteType type,List<string> favs)
        {
            _changedSth = true;
            KnownFavourites[type] = favs;
        }

        public static bool IsFavourite(FavouriteType type, string id)
        {
            return KnownFavourites[type].Any(s => s == id);
        }

        public static async Task AddFavourite(FavouriteType type, string id)
        {
            _changedSth = true;
            KnownFavourites[type].Add(id);
            await MalFavouriteQuery.ModifyFavourite(int.Parse(id), type, true);
        }

        public static async Task RemoveFavourite(FavouriteType type, string id)
        {
            _changedSth = true;
            KnownFavourites[type].Remove(id);
            await MalFavouriteQuery.ModifyFavourite(int.Parse(id), type, false);
        }
    }
}