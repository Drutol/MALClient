using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Runtime;
using MALClient.Models.Enums;

// ReSharper disable InconsistentNaming
namespace Android.Runtime
{
    internal sealed class PreserveAttribute : System.Attribute
    {
        public bool AllMembers;
        public bool Conditional;
    }
}

namespace MALClient.Models.Models.Misc
{
    public class AnimeWallpaperData
    {
        public string RedditUrl { get; set; }
        public string Title { get; set; }
        public string FileUrl { get; set; }
        public bool Nsfw { get; set; }
        public int Upvotes { get; set; }
        public string Thumb { get; set; }
        public DateTime DateTime { get; set; }
        public WallpaperSources Source { get; set; }

        public override bool Equals(object obj)
        {
            var img = obj as AnimeWallpaperData;
            return img?.FileUrl == FileUrl;
        }

        protected bool Equals(AnimeWallpaperData other)
        {
            return string.Equals(FileUrl, other.FileUrl);
        }

        public override int GetHashCode()
        {
            return FileUrl?.GetHashCode() ?? 0;
        }
    }

    [Preserve(AllMembers = true)]
    public class Facets
    {
    }
    [Preserve(AllMembers = true)]
    public class Oembed
    {
        public string provider_url { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public int thumbnail_width { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string html { get; set; }
        public string version { get; set; }
        public string provider_name { get; set; }
        public string thumbnail_url { get; set; }
        public int thumbnail_height { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class SecureMedia
    {
        public Oembed oembed { get; set; }
        public string type { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class SecureMediaEmbed
    {
        public string content { get; set; }
        public int? width { get; set; }
        public bool? scrolling { get; set; }
        public int? height { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Oembed2
    {
        public string provider_url { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public int thumbnail_width { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string html { get; set; }
        public string version { get; set; }
        public string provider_name { get; set; }
        public string thumbnail_url { get; set; }
        public int thumbnail_height { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Media
    {
        public Oembed2 oembed { get; set; }
        public string type { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class MediaEmbed
    {
        public string content { get; set; }
        public int? width { get; set; }
        public bool? scrolling { get; set; }
        public int? height { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Source
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Resolution
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Variants
    {
    }
    [Preserve(AllMembers = true)]
    public class Image
    {
        public Source source { get; set; }
        public List<Resolution> resolutions { get; set; }
        public Variants variants { get; set; }
        public string id { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Preview
    {
        public List<Image> images { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Data2
    {
        public bool contest_mode { get; set; }
        public object banned_by { get; set; }
        public string domain { get; set; }
        public string subreddit { get; set; }
        public string selftext_html { get; set; }
        public string selftext { get; set; }
        public object likes { get; set; }
        public object suggested_sort { get; set; }
        public List<object> user_reports { get; set; }
        public SecureMedia secure_media { get; set; }
        public bool saved { get; set; }
        public string id { get; set; }
        public int gilded { get; set; }
        public SecureMediaEmbed secure_media_embed { get; set; }
        public bool clicked { get; set; }
        public object report_reasons { get; set; }
        public string author { get; set; }
        public Media media { get; set; }
        public string name { get; set; }
        public int score { get; set; }
        public object approved_by { get; set; }
        public bool over_18 { get; set; }
        public object removal_reason { get; set; }
        public bool hidden { get; set; }
        public string thumbnail { get; set; }
        public string subreddit_id { get; set; }
        public object edited { get; set; }
        public string link_flair_css_class { get; set; }
        public string author_flair_css_class { get; set; }
        public int downs { get; set; }
        public List<object> mod_reports { get; set; }
        public bool archived { get; set; }
        public MediaEmbed media_embed { get; set; }
        public bool is_self { get; set; }
        public bool hide_score { get; set; }
        public string permalink { get; set; }
        public bool locked { get; set; }
        public bool stickied { get; set; }
        public double created { get; set; }
        public string url { get; set; }
        public string author_flair_text { get; set; }
        public bool quarantine { get; set; }
        public string title { get; set; }
        public double created_utc { get; set; }
        public string link_flair_text { get; set; }
        public object distinguished { get; set; }
        public int num_comments { get; set; }
        public bool visited { get; set; }
        public object num_reports { get; set; }
        public int ups { get; set; }
        public Preview preview { get; set; }
        public string post_hint { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Child
    {
        public string kind { get; set; }
        public Data2 data { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class Data
    {
        public Facets facets { get; set; }
        public string modhash { get; set; }
        public List<Child> children { get; set; }
        public object after { get; set; }
        public object before { get; set; }
    }
    [Preserve(AllMembers = true)]
    public class RedditSearchRoot
    {
        public string kind { get; set; }
        public Data data { get; set; }
    }
}
