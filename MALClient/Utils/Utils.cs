using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;

namespace MALClient.Utils
{
    public static class Utils
    {
        public static string StatusToString(int status)
        {
            switch (status)
            {
                case 1:
                    return "Watching";
                case 2:
                    return "Completed";
                case 3:
                    return "On hold";
                case 4:
                    return "Dropped";
                case 6:
                    return "Plan to watch";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void RegisterTile(string id)
        {
            var tiles = (string) ApplicationData.Current.LocalSettings.Values["tiles"];
            if (string.IsNullOrWhiteSpace(tiles))
                tiles = "";
            tiles += id + ";";
            ApplicationData.Current.LocalSettings.Values["tiles"] = tiles;
        }

        public static async void CheckTiles()
        {
            string tiles = (string)ApplicationData.Current.LocalSettings.Values["tiles"];
            if(string.IsNullOrWhiteSpace(tiles))
                return;
     

            string newTiles = "";
            foreach (var tileId in tiles.Split(';'))
            {
                if (!SecondaryTile.Exists(tileId))
                {
                    var file = await ApplicationData.Current.LocalFolder.GetFileAsync($"{tileId}.png");
                    await file.DeleteAsync();
                }
                else
                {
                    newTiles += tileId + ";";
                }
            }
            ApplicationData.Current.LocalSettings.Values["tiles"] = newTiles;
        }

    }

    

}
