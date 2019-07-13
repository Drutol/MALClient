using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Models.ApiResponses;
using Newtonsoft.Json;

namespace MALClient.XShared.Comm.Search
{
    public class EverywhereSearchQuery : Query
    {
        public async Task<SearchEverywhereResponse> GetResult(string query)
        {
            try
            {
                var json = await _client.GetStringAsync(
                    $"https://myanimelist.net/search/prefix.json?type=all&keyword={query}&v=1");
                var jsonUser = await _client.GetStringAsync(
                    $"https://myanimelist.net/search/prefix.json?type=user&keyword={query}&v=1");

                var userReponse = JsonConvert.DeserializeObject<SearchEverywhereResponse>(jsonUser);

                var response = JsonConvert.DeserializeObject<SearchEverywhereResponse>(json);

                if (userReponse.Categories.Any())
                {
                    userReponse.Categories[0].Items =
                        userReponse.Categories[0].Items.OrderByDescending(item => item.EsScore).Take(5).ToList();
                    response.Categories.Add(userReponse.Categories.First());
                }

                return response;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
