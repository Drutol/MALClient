using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace MALClient.Comm
{
    /// <summary>
    /// As we are scrapping html here this thing here will try to fetch class data file from github
    /// so I don't have to compile next build in case something changes.
    /// </summary>
    public static class HtmlClassMgr
    {
        /// <summary>
        /// Container for all http class definitions. You guessed right xd
        /// </summary>
        public static Dictionary<string,string> ClassDefs { get; private set; }

        /// <summary>
        /// Fetches newest definitions or loads default ones.
        /// Called on the very beggining even before window is loaded.
        /// </summary>
        public static async void Init()
        {
            try
            {
                var json = await new HtmlClassDefinitionsQuery().GetRequestResponse(false);
                ClassDefs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch (Exception)
            {
                //we have to load defaults for this compilation
                LoadDefaults();
            }
        }

        private static async void LoadDefaults()
        {
            Uri appUri = new Uri("ms-appx:///Comm/HtmlClassesDefinitions.json");
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(appUri);
            var json = await FileIO.ReadTextAsync(file);
            ClassDefs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
          
    }
}
