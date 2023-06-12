using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.Runtime;
using HtmlAgilityPack;
using JikanDotNet;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.Utils;

namespace MALClient.XShared.Comm.Anime
{
    public class AnimeReviewsQuery : Query
    {
        private readonly bool _anime;
        private readonly int _targetId;

        public AnimeReviewsQuery(int id, bool anime = true)
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString($"https://myanimelist.net/{(anime ? "anime" : "manga")}/{id}/whatever/reviews"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
            _targetId = id;
            _anime = anime;
        }

        public async Task<List<AnimeReviewData>> GetAnimeReviews(bool force = false)
        {
            var output = force
                ? new List<AnimeReviewData>()
                : await DataCache.RetrieveReviewsData(_targetId, _anime) ?? new List<AnimeReviewData>();
            if (output.Count != 0) return output;

            try
            {
                var jikan = new Jikan();
                Root reviews;
                if (_anime)
                {
                    var json = await _client.GetStringAsync($"https://api.jikan.moe/v4/anime/{_targetId}/reviews?page=1");
                    reviews = JsonSerializer.Deserialize<Root>(json);
                }
                else
                {
                    var json = await _client.GetStringAsync($"https://api.jikan.moe/v4/manga/{_targetId}/reviews?page=1");
                    reviews = JsonSerializer.Deserialize<Root>(json);
                }

                foreach (var review in reviews.Data)
                {
                    output.Add(new AnimeReviewData
                    {
                        AuthorAvatar = review.User.Images.Jpg.ImageUrl ?? review.User.Images.Webp.ImageUrl,
                        Author = review.User.Username,
                        Date = review.Date?.ToString("d") ?? "N/A",
                        EpisodesSeen = review.EpisodesWatched?.ToString() ?? "N/A",
                        HelpfulCount = review.Reactions.Informative.ToString() ?? "N/A",
                        Id = review.MalId.ToString(),
                        OverallRating = review.Score?.ToString() ?? "N/A",
                        Review = review.Review,
                        HasSpoilers = review.IsSpoiler,
                        IsPreliminary = review.IsPreliminary,
                        Score = new List<ReviewScore>
                        {
                            new ReviewScore
                            {
                                Field = "Informative",
                                Score = review.Reactions.Informative?.ToString() ?? "N/A"
                            },
                            new ReviewScore
                            {
                                Field = "Confusing",
                                Score = review.Reactions.Confusing?.ToString() ?? "N/A"
                            },
                            new ReviewScore
                            {
                                Field = "Creative",
                                Score = review.Reactions.Creative?.ToString() ?? "N/A"
                            },
                            new ReviewScore
                            {
                                Field = "Funny",
                                Score = review.Reactions.Funny?.ToString() ?? "N/A"
                            },
                            new ReviewScore
                            {
                                Field = "Love It",
                                Score = review.Reactions.LoveIt?.ToString() ?? "N/A"
                            },
                            new ReviewScore
                            {
                                Field = "Well Written",
                                Score = review.Reactions.WellWritten?.ToString() ?? "N/A"
                            },
                        }
                    });

                    DataCache.SaveAnimeReviews(_targetId, output, _anime);
                }
            }
            catch (Exception e)
            {
                
            }
            
            return output;
        }
    }

    [Preserve(AllMembers = true)]
    public class Datum
    {
        [JsonPropertyName("mal_id")]
        public int MalId { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("reactions")]
        public Reactions Reactions { get; set; }

        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }

        [JsonPropertyName("review")]
        public string Review { get; set; }

        [JsonPropertyName("score")]
        public int? Score { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }

        [JsonPropertyName("is_spoiler")]
        public bool IsSpoiler { get; set; }

        [JsonPropertyName("is_preliminary")]
        public bool IsPreliminary { get; set; }

        [JsonPropertyName("episodes_watched")]
        public int? EpisodesWatched { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Images
    {
        [JsonPropertyName("jpg")]
        public Jpg Jpg { get; set; }

        [JsonPropertyName("webp")]
        public Webp Webp { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Jpg
    {
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Pagination
    {
        [JsonPropertyName("last_visible_page")]
        public int LastVisiblePage { get; set; }

        [JsonPropertyName("has_next_page")]
        public bool HasNextPage { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Reactions
    {
        [JsonPropertyName("overall")]
        public int? Overall { get; set; }

        [JsonPropertyName("nice")]
        public int? Nice { get; set; }

        [JsonPropertyName("love_it")]
        public int? LoveIt { get; set; }

        [JsonPropertyName("funny")]
        public int? Funny { get; set; }

        [JsonPropertyName("confusing")]
        public int? Confusing { get; set; }

        [JsonPropertyName("informative")]
        public int? Informative { get; set; }

        [JsonPropertyName("well_written")]
        public int? WellWritten { get; set; }

        [JsonPropertyName("creative")]
        public int? Creative { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Root
    {
        [JsonPropertyName("pagination")]
        public Pagination Pagination { get; set; }

        [JsonPropertyName("data")]
        public List<Datum> Data { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class User
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("images")]
        public Images Images { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Webp
    {
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }
    }
}