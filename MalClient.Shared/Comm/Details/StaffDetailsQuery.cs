using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MalClient.Shared.Comm.Details
{
    class StaffDetailsQuery : Query
    {

        private readonly int _id;

        public StaffDetailsQuery(int id)
        {
            _id = id;
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"http://myanimelist.net/people/{id}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}
