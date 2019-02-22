using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Net.Http;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace MALClient.XShared.Utils
{
    public static class HttpExtensions
    {
        public static void ConfigureToAcceptCompressedContent(this HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        }

        public static async Task<Stream> GetDecompressionStreamAsync(this HttpResponseMessage response)
        {
            if (response == null)
            {
                throw new ArgumentException("Can't get stream from null response", nameof(response));
            }

            if (response.Content == null)
            {
                throw new InvalidOperationException("HttpResponseMessage appears to have completed but the response content was null");
            }

            HttpContentHeaders contentHeaders = response.Content.Headers;

            var responseStream = await response.Content.ReadAsStreamAsync()
                                                       .ConfigureAwait(false);

            // If header is not present the server may not support any compression algorithm OR the
            // given HttpMessageHandler has support for automatic compression.
            if (contentHeaders != null && contentHeaders.ContentEncoding.Count > 0)
            {
                foreach (var entry in contentHeaders.ContentEncoding)
                {
                    switch (entry.ToLowerInvariant())
                    {
                        case "gzip":
                            {
                                return new GZipStream(responseStream, CompressionMode.Decompress);
                            }
                        case "deflate":
                            {
                                return new DeflateStream(responseStream, CompressionMode.Decompress);
                            }
                    }
                }
            }

            return responseStream;
        }

    }
}
