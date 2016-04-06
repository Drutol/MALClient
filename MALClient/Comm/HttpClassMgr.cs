using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace MALClient.Comm
{
    public static class HttpClassMgr
    {
        public static Dictionary<string,string> ClassDefs { get; private set; } = new Dictionary<string, string>();

        public static async void Init()
        {
            try
            {
                var json = await new HttpClassDefinitionsQuery().GetRequestResponse(false);
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
            StorageFile anjFile = await StorageFile.GetFileFromApplicationUriAsync(appUri);
            var json = await FileIO.ReadTextAsync(anjFile);
            ClassDefs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
          
    }
}
