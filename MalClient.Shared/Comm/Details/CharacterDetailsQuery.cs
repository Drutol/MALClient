using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MalClient.Shared.Comm.Details
{
    public class CharacterDetailsQuery : Query
    {
        private readonly int _id;

        public CharacterDetailsQuery(int id)
        {
            _id = id;
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"http://myanimelist.net/character/{id}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}
