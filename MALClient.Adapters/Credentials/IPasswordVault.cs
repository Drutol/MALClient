using System;
using MALClient.Models.AdapterModels;

namespace MALClient.Adapters.Credentials
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
