using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MALClient.Models.Models.Anime
{
    public class AnimeGeneralDetailsData
    {
        private int _id = -1;

        public int Id
        {
            get { return _id == -1 ? MalId : _id; }
            set { _id = value; }
        }

        public int MalId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Synopsis { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ImgUrl { get; set; }
        public int AllEpisodes { get; set; }
        public int AllVolumes { get; set; }
        public float GlobalScore { get; set; }
        public List<string> Synonyms { get; set; } = new List<string>();

        public void ParseXElement(XElement xmlObj, bool anime)
        {
            float score;
            if (!float.TryParse(xmlObj.Element("score").Value, out score))
                score = 0;
            MalId = Convert.ToInt32(xmlObj.Element("id").Value);
            Title = xmlObj.Element("title").Value;
            GlobalScore = score;
            Type = xmlObj.Element("type").Value;
            Status = xmlObj.Element("status").Value;
            Synopsis = Utilities.DecodeXmlSynopsisDetail(xmlObj.Element("synopsis").Value);
            StartDate = xmlObj.Element("start_date").Value;
            EndDate = xmlObj.Element("end_date").Value;
            ImgUrl = xmlObj.Element("image").Value;
            Synonyms = xmlObj.Element("synonyms").Value.Split(',').ToList();
            AllEpisodes = Convert.ToInt32(xmlObj.Element(anime ? "episodes" : "chapters").Value);
            AllVolumes = !anime ? Convert.ToInt32(xmlObj.Element("volumes").Value) : 0;
        }
    }
}