using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MALClient.Comm
{
    public abstract class Query
    {
        protected string HttpQuery;
        protected WebRequest Request;

        public async Task<XDocument> GetRequestResponse()
        {
            var response = await Request.GetResponseAsync();

            string responseString = "";
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                responseString = reader.ReadToEnd();
            }
            return XDocument.Parse(responseString);
        }

    }

    
}
