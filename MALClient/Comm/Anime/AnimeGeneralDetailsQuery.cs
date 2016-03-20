﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
using MALClient.Items;
using MALClient.Models;

namespace MALClient.Comm
{
    public class AnimeGeneralDetailsQuery : Query
    {
        private readonly int _id;
        private readonly string _rootTitle;

        public AnimeGeneralDetailsQuery(string title,int id,string rootTitle)
        {
            title = title.Replace('+', ' ');
            Request = WebRequest.Create(Uri.EscapeUriString($"http://cdn.animenewsnetwork.com/encyclopedia/api.xml?title=~{title}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
            _id = id;
            _rootTitle = rootTitle;
        }

        public async Task<AnimeGeneralDetailsData> GetGeneralDetailsData(bool force = false)
        {
            var possibleData = force ? null : await DataCache.RetrieveAnimeGeneralDetailsData(_id);
            if (possibleData != null)
                return possibleData;

            string raw = await GetRequestResponse();
            if (string.IsNullOrEmpty(raw))
                return null;

            AnimeGeneralDetailsData output;

            try
            {
                XDocument data = XDocument.Parse(raw);
                var nodes = data.Element("ann").Elements("anime");
                //there may be many things , maybe we are lucky enough to grab original title
                var node =
                    nodes.FirstOrDefault(
                        element =>
                            string.Equals(element.Attribute("name").Value, _rootTitle,
                                StringComparison.CurrentCultureIgnoreCase));

                if (node == null)
                    node = nodes.First();

                output = new AnimeGeneralDetailsData
                {
                    AnnId = node.Attribute("id").Value,
                    Genres =
                        node.Elements("info")
                            .Where(element => element.Attribute("type").Value == "Genres")
                            .Select(element => element.Value)
                            .ToList(),
                    Episodes = node.Elements("episode").Select(element => element.Value).ToList(),
                    OPs =
                        node.Elements("info")
                            .Where(element => element.Attribute("type").Value == "Opening Theme")
                            .Select(element => element.Value)
                            .ToList(),
                    EDs =
                        node.Elements("info")
                            .Where(element => element.Attribute("type").Value == "Ending Theme")
                            .Select(element => element.Value)
                            .ToList(),
                };

                DataCache.SaveAnimeDetails(_id, output);

                return output;
            }
            catch (Exception)
            {
                return null;
            }        
        }

    }
}