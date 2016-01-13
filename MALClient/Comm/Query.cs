using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Comm
{
    public abstract class Query
    {
        protected WebRequest Request;

        public virtual async Task<string> GetRequestResponse()
        {
            var response = await Request.GetResponseAsync();

            string responseString = "";
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                responseString = reader.ReadToEnd();
            }
            return responseString;
        }

    }

    
}
