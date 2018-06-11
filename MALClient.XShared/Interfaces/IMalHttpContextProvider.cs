using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MALClient.XShared.Comm.MagicalRawQueries;

namespace MALClient.XShared.Interfaces
{
    public interface IMalHttpContextProvider
    {
        void ErrorMessage(string what);

        /// <summary>
        ///     Establishes connection with MAL, attempts to authenticate.
        /// </summary>
        /// <param name="updateToken">
        ///     Indicates whether created http client is meant to be used further or do we want to dispose it and return null.
        /// </param>
        /// <exception cref="WebException">
        ///     Unable to authorize.
        /// </exception>
        /// <returns>
        ///     Returns valid http client which can interact with website API.
        /// </returns>
        Task<CsrfHttpClient> GetHttpContextAsync(bool skipAuthCheck = false);

        void Invalidate();
    }
}
