using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.AdapterModels;

namespace MALClient.Adapters.Credentails
{
    public interface IPasswordVault
    {
        void Add(VaultCredential credential);
        
        /// <summary>
        /// <exception cref="Exception">
        ///     Should throw exception in case of failure.
        /// </exception>
        /// </summary>
        /// <returns></returns>
        VaultCredential Get(string domain);

        void Reset();
    }
}
