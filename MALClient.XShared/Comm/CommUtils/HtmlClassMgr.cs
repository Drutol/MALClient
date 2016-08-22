using System.Collections.Generic;
using Newtonsoft.Json;

namespace MalClient.Shared.Comm.CommUtils
{
    /// <summary>
    ///     As we are scrapping html here this thing here will try to fetch class data file from github
    ///     so I don't have to compile next build in case something changes.
    /// </summary>
    public static class HtmlClassMgr
    {
        private const string ClassDefsJson = @"{
  ""#DirectRecomm:recommNode:class"": ""borderClass"",
  ""#DirectRecomm:recommNode:descClass"": ""borderClass bgColor1"",
  ""#Related:relationsNode:class"": ""anime_detail_related_anime"",
  ""#Reviews:reviewNode:class"": ""borderDark pt4 pb8 pl4 pr4 mb8"",
  ""#Reviews:reviewNode:pictureNodeClass"": ""picSurround"",
  ""#Reviews:reviewNode:helpfulCountNode"": ""lightLink spaceit"",
  ""#Reviews:reviewNode:contentNode"": ""spaceit textReadability word-break"",
  ""#Seasonal:mainNode:class"": ""seasonal-anime-list js-seasonal-anime-list js-seasonal-anime-list-key-1 clearfix"",
  ""#Seasonal:seasonInfo:class"": ""horiznav_nav"",
  ""#Seasonal:seasonInfoCurrent:class"": ""on"",
  ""#Seasonal:entryNode:class"": ""seasonal-anime js-seasonal-anime"",
  ""#Seasonal:entryNode:image:class"": ""image lazyload"",
  ""#Seasonal:entryNode:score:class"": ""score"",
  ""#Seasonal:entryNode:info:class"": ""info"",
  ""#Seasonal:entryNode:eps:class"": ""eps"",
  ""#Seasonal:entryNode:genres:class"": ""genres-inner js-genre-inner"",
  ""#Top:mainNode:class"": ""top-ranking-table"",
  ""#Top:topNode:class"": ""ranking-list"",
  ""#Top:topNode:eps:class"": ""information di-ib mt4"",
  ""#Top:topNode:titleNode:class"": ""hoverinfo_trigger fl-l fs14 fw-b"",
  ""#Top:topMangaNode:titleNode:class"": ""hoverinfo_trigger fs14 fw-b"",
  ""#Top:topNode:score:class"": ""text on"",
  ""#Profile:recentUpdateNode:class"": ""statistics-updates di-b w100 mb8"",
  ""#Profile:favCharacterNode:class"": ""favorites-list characters"",
  ""#Profile:favAnimeNode:class"": ""favorites-list anime"",
  ""#Profile:favMangaNode:class"": ""favorites-list manga"",
  ""#Profile:favPeopleNode:class"": ""favorites-list people"",
  ""#Recommendations:recommNode:class"": ""spaceit borderClass"",
  ""#Recommendations:recommNodeDesc:class"": ""spaceit""
}";
        /// <summary>
        ///     Container for all http class definitions. You guessed right xd
        /// </summary>
        public static Dictionary<string, string> ClassDefs { get; private set; }

        /// <summary>
        ///     Fetches newest definitions or loads default ones.
        ///     Called on the very beggining even before window is loaded.
        /// </summary>
        public static void Init()
        {
            ClassDefs = JsonConvert.DeserializeObject<Dictionary<string, string>>(ClassDefsJson);


            //try
            //{
            //    var json = await new HtmlClassDefinitionsQuery().GetRequestResponse(false);
            //    ClassDefs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            //}
            //catch (Exception)
            //{
            //    //we have to load defaults for this compilation
            //    LoadDefaults();
            //}
        }

        //private static async void LoadDefaults()
        //{
        //    //var appUri = new Uri("ms-appx:///Comm/HtmlClassesDefinitions.json");
        //    //var file = await StorageFile.GetFileFromApplicationUriAsync(appUri);
        //    //var json = await FileIO.ReadTextAsync(file);
        //    //ClassDefs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        //}
    }
}