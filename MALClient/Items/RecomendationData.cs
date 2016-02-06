using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using MALClient.Comm;

namespace MALClient.Items
{
    public class RecomendationData
    {
        //Keys
        public string DependentTitle { get; set; }
        public int DependentId { get; set; }
        //Moar info
        public string DependentImgUrl { get; private set; }
        public float DependentGlobalScore { get; private set; }
        public string DependentSynopsis { get; private set; }
        public string DependentStartDate { get; private set; }
        public string DependentEndDate { get; private set; }
        public string DependentType { get; private set; }
        public string DependentStatus { get; private set; }

        //Keys
        public string RecommendationTitle { get; set; }
        public int RecommendationId { get; set; }
        //Moar info
        public string RecommendationImgUrl { get; private set; }
        public float RecommendationGlobalScore { get; private set; }
        public string RecommendationSynopsis { get; private set; }
        public string RecommendationStartDate { get; private set; }
        public string RecommendationEndDate { get; private set; }
        public string RecommendationType { get; private set; }
        public string RecommendationStatus { get; private set; }

        public string Description { get; set; }

        public XElement DependentData { get; set; }
        public XElement RecommendationData { get; set; }

        private bool _loaded;

        public async Task FetchData()
        {
            if (_loaded)
                return;
            //Find for first
            string data = await new AnimeSearchQuery(Utils.CleanAnimeTitle(DependentTitle)).GetRequestResponse();
            data = WebUtility.HtmlDecode(data);
            data = data.Replace("&mdash", "").Replace("&rsquo", "").Replace("&", "");

            XDocument parsedData = XDocument.Parse(data);
            var elements = parsedData.Element("anime").Elements("entry");
            DependentData = elements.First(element => Convert.ToInt32(element.Element("id").Value) == DependentId);

            //Find for second
            data = await new AnimeSearchQuery(Utils.CleanAnimeTitle(RecommendationTitle)).GetRequestResponse();
            data = WebUtility.HtmlDecode(data);
            data = data.Replace("&mdash", "").Replace("&rsquo", "").Replace("&", "");

            parsedData = XDocument.Parse(data);
            elements = parsedData.Element("anime").Elements("entry");
            RecommendationData = elements.First(element => Convert.ToInt32(element.Element("id").Value) == RecommendationId);
            
            //If for some reason we fail
            if(DependentData == null || RecommendationData == null)
                throw new ArgumentNullException(); // I'm to lazy to create my own so this will suffice

            //Set data - Dependant

            DependentGlobalScore = float.Parse(DependentData.Element("score").Value);
            DependentType = DependentData.Element("type").Value;
            DependentStatus = DependentData.Element("status").Value;
            DependentSynopsis = Regex.Replace(DependentData.Element("synopsis").Value, @"<[^>]+>|&nbsp;", "").Trim().Replace("[i]", "").Replace("[/i]", "");
            DependentStartDate = DependentData.Element("start_date").Value;
            DependentEndDate = DependentData.Element("end_date").Value;
            DependentImgUrl = DependentData.Element("image").Value;

            //Set data - Recom
            RecommendationGlobalScore = float.Parse(RecommendationData.Element("score").Value);
            RecommendationType = RecommendationData.Element("type").Value;
            RecommendationStatus = RecommendationData.Element("status").Value;
            RecommendationSynopsis = Regex.Replace(RecommendationData.Element("synopsis").Value, @"<[^>]+>|&nbsp;", "").Trim().Replace("[i]", "").Replace("[/i]", "");
            RecommendationStartDate = RecommendationData.Element("start_date").Value;
            RecommendationEndDate = RecommendationData.Element("end_date").Value;
            RecommendationImgUrl = RecommendationData.Element("image").Value;

            //Okay we got this data
            _loaded = true;
        }
    }
}
