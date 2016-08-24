using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.AdapterModels
{
    public class VaultCredential
    {
        public string Domain { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public VaultCredential(string domain,string username,string password)
        {
            Domain = domain;
            UserName = username;
            Password = password;
        }
    }
}
