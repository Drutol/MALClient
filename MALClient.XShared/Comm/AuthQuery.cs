using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.Models.Models.Auth;
using MALClient.XShared.Comm.MagicalRawQueries;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;

namespace MALClient.XShared.Comm
{
    public class AuthQuery : Query
    {
        private readonly FormUrlEncodedContent _content;

        public AuthQuery(FormUrlEncodedContent content)
        {
            _content = content;
        }

        public override async Task<string> GetRequestResponse()
        {
            try
            {
                var response = await _client.PostAsync("https://myanimelist.net/v1/oauth2/token", _content);
                response.EnsureSuccessStatusCode();
                var tokens = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());

                tokens.ValidTill = DateTime.UtcNow.AddSeconds(tokens.ExpiresIn);

                Credentials.SetAuthTokenResponse(tokens);

                
                return "ok";
            }
            catch (Exception e)
            {
                return null;
            }         
        }
    }
}