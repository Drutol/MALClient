using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MalClient.Shared.Models.Forums;
using MalClient.Shared.Utils;

namespace MalClient.Shared.Comm.Forums
{
    public class ForumBoardIndexPeekPostsQuery : Query
    {
        public ForumBoardIndexPeekPostsQuery()
        {
            Request =
                WebRequest.Create(Uri.EscapeUriString("http://myanimelist.net/forum/"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<List<ForumBoardEntryPeekPost>>> GetPeekPosts()
        {
            var output = new List<List<ForumBoardEntryPeekPost>>();
#region test string
            var raw = @"<html><head>    

<title>Forums - MyAnimeList.net
</title>
<meta name=""description"" content=""Trying to find a place to discuss anime, manga, and more? Check out the forums on MyAnimeList, the world's most active online anime and manga community and database! Join the online community, create your anime and manga list, read reviews, explore the forums, follow news, and so much more!"">

  
<meta name=""keywords"" content=""anime, myanimelist, anime news, manga"">
  

<meta property=""og:locale"" content=""en_US""><meta property=""fb:app_id"" content=""360769957454434""><meta property=""og:site_name"" content=""MyAnimeList.net""><meta name=""twitter:card"" content=""summary""><meta name=""twitter:site"" content=""@myanimelist""><meta property=""og:title"" content=""Forums - MyAnimeList.net ""><meta property=""og:image"" content=""http://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png""><meta name=""twitter:image:src"" content=""http://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png""><meta property=""og:url"" content=""http://myanimelist.net/forum/""><meta property=""og:description"" content=""Trying to find a place to discuss anime, manga, and more? Check out the forums on MyAnimeList, the world's most active online anime and manga community and database! Join the online community, create your anime and manga list, read reviews, explore the forums, follow news, and so much more!"">
<meta name=""csrf_token"" content=""af7719f00a074da2480e610b1d1f3e1007beae26"">
<link rel=""stylesheet"" type=""text/css"" href=""http://cdn.myanimelist.net/static/assets/css/pc/style-2455f8dda3.css"">

<script src=""http://pagead2.googlesyndication.com/pagead/osd.js""></script><script type=""text/javascript"" async="""" src=""http://www.google-analytics.com/plugins/ua/linkid.js""></script><script type=""text/javascript"" async="""" src=""https://www.gstatic.com/recaptcha/api2/r20160718175036/recaptcha__en.js""></script><script async="""" type=""text/javascript"" src=""http://www.googletagservices.com/tag/js/gpt.js""></script><script async="""" src=""//www.google-analytics.com/analytics.js""></script><script type=""text/javascript"" src=""http://cdn.myanimelist.net/static/assets/js/pc/header-1147b3497b.js""></script>
<script type=""text/javascript"" src=""http://cdn.myanimelist.net/static/assets/js/pc/all-49f169a5c6.js"" id=""alljs"" data-params=""{&quot;origin_url&quot;:&quot;http:\/\/myanimelist.net&quot;,&quot;is_request_bot_filter_log&quot;:true}"" defer=""""></script>



<link rel=""search"" type=""application/opensearchdescription+xml"" href=""http://cdn.myanimelist.net/plugins/myanimelist.xml"" title=""MyAnimeList"">

<link rel=""shortcut icon"" href=""http://cdn.myanimelist.net/images/faviconv5.ico"">

<script>
  (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
  (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
  })(window,document,'script','//www.google-analytics.com/analytics.js','ga');

  ga('create', 'UA-369102-1', 'auto');
  ga('require', 'linkid'); // Enhanced Link Attribution
  ga('send', 'pageview');
</script>

<!-- ### Load GPT Library ### -->
<script type=""text/javascript"">
 var googletag = googletag || {};
  googletag.cmd = googletag.cmd || [];
  (function() {
    var gads = document.createElement('script');
    gads.async = true;
    gads.type = 'text/javascript';
    var useSSL = 'https:' == document.location.protocol;
    gads.src = (useSSL ? 'https:' : 'http:') +
               '//www.googletagservices.com/tag/js/gpt.js';
    var node = document.getElementsByTagName('script')[0];
    node.parentNode.insertBefore(gads, node);
  })();</script>


<script src=""https://apis.google.com/js/platform.js"" async="""" defer="""" gapi_processed=""true"">{lang: 'en-GB'}</script>
<script src=""https://www.google.com/recaptcha/api.js?hl=en""></script>

      <script src=""http://partner.googleadservices.com/gpt/pubads_impl_91.js"" async=""""></script><link rel=""prefetch"" href=""http://tpc.googlesyndication.com/safeframe/1-0-4/html/container.html""></head>

    <body class=""page-common forum"">  
      <div id=""myanimelist"">
      
  

<div class=""_unit "" style=""width:1px;display: block !important;"" data-height=""1"">
  <div id=""skin_detail"" class="""" style=""width: 1px; display: none;"">
    <script type=""text/javascript"">
      googletag.cmd.push(function() {
             var slot = googletag.defineOutOfPageSlot(""/84947469/skin_detail"", ""skin_detail"").addService(googletag.pubads())
    .setTargeting(""adult"", ""gray"")
    .setCollapseEmptyDiv(true,true);googletag.enableServices();

  googletag.display(""skin_detail"");
         });</script>
  <div id=""google_ads_iframe_/84947469/skin_detail_0__container__"" style=""border: 0pt none;""><iframe id=""google_ads_iframe_/84947469/skin_detail_0"" title=""3rd party ad content"" name=""google_ads_iframe_/84947469/skin_detail_0"" width=""1"" height=""1"" scrolling=""no"" marginwidth=""0"" marginheight=""0"" frameborder=""0"" src=""javascript:&quot;<html><body style='background:transparent'></body></html>&quot;"" style=""border: 0px; vertical-align: bottom;""></iframe></div><iframe id=""google_ads_iframe_/84947469/skin_detail_0__hidden__"" title="""" name=""google_ads_iframe_/84947469/skin_detail_0__hidden__"" width=""0"" height=""0"" scrolling=""no"" marginwidth=""0"" marginheight=""0"" frameborder=""0"" src=""javascript:&quot;<html><body style='background:transparent'></body></html>&quot;"" style=""border: 0px; vertical-align: bottom; visibility: hidden; display: none;""></iframe></div>
</div>

    <script type=""text/javascript"">
    window.MAL.SkinAd.prepareForSkin('skin_detail');
  </script>

    <div id=""ad-skin-bg-left"" class=""ad-skin-side-outer ad-skin-side-bg bg-left"">
    <div id=""ad-skin-left"" class=""ad-skin-side left"" style=""display: none;"">
      <div id=""ad-skin-left-absolute-block"">
        <div id=""ad-skin-left-fixed-block""></div>
      </div>
    </div>
  </div><div class=""wrapper"">
        
                        
        <div id=""headerSmall"" class="""">

<a href=""http://myanimelist.net/watch/episode?_location=mal_h_b"" class=""banner-header-anime-straming""><img src=""http://cdn.myanimelist.net/images/stream_banner/banner-watch-offical-anime161511.png"" srcset=""http://cdn.myanimelist.net/images/stream_banner/banner-watch-offical-anime161511.png 1x, http://cdn.myanimelist.net/images/stream_banner/banner-watch-offical-anime161511@2x.png 2x"" alt=""Watch Legal Streaming Anime on MyAnimeList""></a>

<div id=""header-menu"" class=""pulldown""><div class=""header-menu-unit header-list js-header-menu-unit"" data-id=""list""><a class=""header-list-button "" title=""List""><i class=""fa fa-list""></i></a><div class=""header-menu-dropdown header-list-dropdown arrow_box""><ul><li><a href=""http://myanimelist.net/animelist/Drutol"">Anime List</a></li><li><a href=""http://myanimelist.net/mangalist/Drutol"">Manga List</a></li><li><a href=""http://myanimelist.net/addtolist.php"">Quick Add</a></li><li><a href=""http://myanimelist.net/editprofile.php?go=listpreferences"">List Settings</a></li></ul></div></div><div class=""border""></div><div class=""header-menu-unit header-message js-header-menu-unit"" data-id=""message""><a href=""http://myanimelist.net/mymessages.php"" data-unread=""5"" class=""header-message-button has-unread text1"" title=""You have 5 messages.""><i class=""fa fa-envelope""></i></a></div><div class=""border""></div><div class=""header-menu-unit header-notification js-header-notification""><a class=""header-notification-button has-unread new text2"" aria-haspopup=""true"" aria-label=""Notifications"" title=""Notifications"" tabindex=""0"" aria-expanded=""false"" data-unread=""14"">
      <i class=""fa fa-bell""></i>
    </a>
    <div class=""header-notification-dropdown"" style=""display: none;"">
      <div class=""arrow_box"">
        <div class=""header-notification-dropdown-inner"">
          <h3>
            <span class=""settings ml8"">
              <a href=""/notification/setting"">
                <i class=""fa fa-cog mr4""></i>Settings
              </a>
            </span>
            <span class=""mark-all"">
              <i class=""fa fa-check mr4""></i>Mark All as Read
            </span>
            Notifications
          </h3>
          <ol class=""header-notification-item-list"">
            <li class=""header-notification-item profile_comment new"">
              <a class=""background"" href=""http://myanimelist.net/comtocom.php?id1=4952914&amp;id2=5344716""></a>
              <div class=""inner"">
                <div class=""header-notification-item-header"">
                                    <span class=""is-read"">
                    <i class=""fa fa-check""></i>
                    <i class=""fa fa-check-circle""></i>
                  </span>
                                    <span class=""time"">Jul 19, 10:04 AM</span>
                                    <span class=""category"">Profile Comments</span>
                </div>
                                <div class=""header-notification-item-content profile_comment"" on-check-item=""function (n){var i=arguments.length;return i?i>1?e.apply(t,arguments):e.call(t,n):e.call(t)}"">
                                    <div>
      <a href=""http://myanimelist.net/profile/zero_omar"">zero_omar</a>
      posted a comment on your profile:
      <q class=""message"">how was index ? :D you might like Railgun S. it's talking about the sisters arc but from Misaka's perspective...</q>
    </div>
                  
                </div>
              </div>
            </li><li class=""header-notification-item forum_quote new"">
              <a class=""background"" href=""http://myanimelist.net/forum/message/46985793?goto=topic""></a>
              <div class=""inner"">
                <div class=""header-notification-item-header"">
                                    <span class=""is-read"">
                    <i class=""fa fa-check""></i>
                    <i class=""fa fa-check-circle""></i>
                  </span>
                                    <span class=""time"">Jul 19, 1:23 AM</span>
                                    <span class=""category"">Forum Quotes</span>
                </div>
                                <div class=""header-notification-item-content forum_quote"" on-check-item=""function (n){var i=arguments.length;return i?i>1?e.apply(t,arguments):e.call(t,n):e.call(t)}"">
                                    <div>
      <a href=""http://myanimelist.net/profile/Haruka"">Haruka</a>
      quoted your post in the <a href=""http://myanimelist.net/forum/?topicid=1499207"">[App] MALClient for Windows 10</a> topic.
    </div>
                  
                </div>
              </div>
            </li><li class=""header-notification-item user_mention_in_forum_message new"">
              <a class=""background"" href=""http://myanimelist.net/forum/message/46947545?goto=topic""></a>
              <div class=""inner is-read"">
                <div class=""header-notification-item-header"">
                                    <span class=""is-read checked"" style=""display: none;"">
                    <i class=""fa fa-check""></i>
                    <i class=""fa fa-check-circle""></i>
                  </span>
                                    <span class=""time"">Jul 16, 6:11 PM</span>
                                    <span class=""category"">User Mentions</span>
                </div>
                                <div class=""header-notification-item-content user_mention_in_forum_message"" on-check-item=""function (n){var i=arguments.length;return i?i>1?e.apply(t,arguments):e.call(t,n):e.call(t)}"">
                                    <div>
      <a href=""http://myanimelist.net/profile/Utagai-"">Utagai-</a>
      has mentioned your name in the
      <a href=""http://myanimelist.net/forum/?topicid=1533972"">I've written a little Python API wrapper for the Official MAL API</a>
      <span>topic</span>.
    </div>
                  
                </div>
              </div>
            </li><li class=""header-notification-item related_anime_add new"">
              <a class=""background"" href=""http://myanimelist.net/anime/33569/Re_Petit_kara_Hajimeru_Isekai_Seikatsu""></a>
              <div class=""inner"">
                <div class=""header-notification-item-header"">
                                    <span class=""is-read"">
                    <i class=""fa fa-check""></i>
                    <i class=""fa fa-check-circle""></i>
                  </span>
                                    <span class=""time"">Jul 13, 3:27 AM</span>
                                    <span class=""category"">New Related Anime</span>
                </div>
                                <div class=""header-notification-item-content related_anime_add"" on-check-item=""function (n){var i=arguments.length;return i?i>1?e.apply(t,arguments):e.call(t,n):e.call(t)}"">
                                    <div>
      <a href=""http://myanimelist.net/anime/33569/Re_Petit_kara_Hajimeru_Isekai_Seikatsu"">Re:Petit kara Hajimeru Isekai Seikatsu</a>&nbsp;(Special) has just been added to the database.
    </div>
                  
                </div>
              </div>
            </li><li class=""header-notification-item on_air new"">
              <a class=""background"" href=""http://myanimelist.net/anime/season""></a>
              <div class=""inner"">
                <div class=""header-notification-item-header"">
                                    <span class=""is-read"">
                    <i class=""fa fa-check""></i>
                    <i class=""fa fa-check-circle""></i>
                  </span>
                                    <span class=""time"">Jul 11, 4:28 AM</span>
                                    <span class=""category"">Now Airing</span>
                </div>
                                <div class=""header-notification-item-content on_air"" on-check-item=""function (n){var i=arguments.length;return i?i>1?e.apply(t,arguments):e.call(t,n):e.call(t)}"">
                                    <div>
      The anime you plan to watch will begin airing on Jul 12:
      <p class=""ml8 mt4"">
        <a href=""http://myanimelist.net/anime/32182/Mob_Psycho_100"">Mob Psycho 100</a>
      </p>
    </div>
                  
                </div>
              </div>
            </li><li class=""header-notification-item on_air new"">
              <a class=""background"" href=""http://myanimelist.net/anime/season""></a>
              <div class=""inner"">
                <div class=""header-notification-item-header"">
                                    <span class=""is-read"">
                    <i class=""fa fa-check""></i>
                    <i class=""fa fa-check-circle""></i>
                  </span>
                                    <span class=""time"">Jul 10, 4:23 AM</span>
                                    <span class=""category"">Now Airing</span>
                </div>
                                <div class=""header-notification-item-content on_air"" on-check-item=""function (n){var i=arguments.length;return i?i>1?e.apply(t,arguments):e.call(t,n):e.call(t)}"">
                                    <div>
      The anime you plan to watch will begin airing on Jul 10:
      <p class=""ml8 mt4"">
        <a href=""http://myanimelist.net/anime/30911/Tales_of_Zestiria_the_X"">Tales of Zestiria the X</a>
      </p>
    </div>
                  
                </div>
              </div>
            </li><li class=""header-notification-item on_air new"">
              <a class=""background"" href=""http://myanimelist.net/anime/season""></a>
              <div class=""inner"">
                <div class=""header-notification-item-header"">
                                    <span class=""is-read"">
                    <i class=""fa fa-check""></i>
                    <i class=""fa fa-check-circle""></i>
                  </span>
                                    <span class=""time"">Jul 9, 4:19 AM</span>
                                    <span class=""category"">Now Airing</span>
                </div>
                                <div class=""header-notification-item-content on_air"" on-check-item=""function (n){var i=arguments.length;return i?i>1?e.apply(t,arguments):e.call(t,n):e.call(t)}"">
                                    <div>
      The anime you plan to watch will begin airing on Jul 10:
      <p class=""ml8 mt4"">
        <a href=""http://myanimelist.net/anime/32171/Ange_Vierge"">Ange Vierge</a>, <a href=""http://myanimelist.net/anime/32360/Qualidea_Code"">Qualidea Code</a>
      </p>
    </div>
                  
                </div>
              </div>
            </li><li class=""header-notification-item profile_comment new"">
              <a class=""background"" href=""http://myanimelist.net/comtocom.php?id1=4952914&amp;id2=5344716""></a>
              <div class=""inner"">
                <div class=""header-notification-item-header"">
                                    <span class=""is-read"">
                    <i class=""fa fa-check""></i>
                    <i class=""fa fa-check-circle""></i>
                  </span>
                                    <span class=""time"">Jul 8, 3:41 PM</span>
                                    <span class=""category"">Profile Comments</span>
                </div>
                                <div class=""header-notification-item-content profile_comment"" on-check-item=""function (n){var i=arguments.length;return i?i>1?e.apply(t,arguments):e.call(t,n):e.call(t)}"">
                                    <div>
      <a href=""http://myanimelist.net/profile/zero_omar"">zero_omar</a>
      posted a comment on your profile:
      <q class=""message"">release order: index 1 &gt; railgun &gt; index 2 &gt; railgun S cronoligical : index 1 and railgun S happened...</q>
    </div>
                  
                </div>
              </div>
            </li><li class=""header-notification-item on_air"">
              <a class=""background"" href=""http://myanimelist.net/anime/season""></a>
              <div class=""inner"">
                <div class=""header-notification-item-header"">
                                    <span class=""is-read"">
                    <i class=""fa fa-check""></i>
                    <i class=""fa fa-check-circle""></i>
                  </span>
                                    <span class=""time"">Jul 8, 4:25 AM</span>
                                    <span class=""category"">Now Airing</span>
                </div>
                                <div class=""header-notification-item-content on_air"" on-check-item=""function (n){var i=arguments.length;return i?i>1?e.apply(t,arguments):e.call(t,n):e.call(t)}"">
                                    <div>
      The anime you plan to watch will begin airing on Jul 9:
      <p class=""ml8 mt4"">
        <a href=""http://myanimelist.net/anime/32998/91_Days"">91 Days</a>
      </p>
    </div>
                  
                </div>
              </div>
            </li><li class=""header-notification-item profile_comment"">
              <a class=""background"" href=""http://myanimelist.net/comtocom.php?id1=4952914&amp;id2=5344716""></a>
              <div class=""inner"">
                <div class=""header-notification-item-header"">
                                    <span class=""is-read"">
                    <i class=""fa fa-check""></i>
                    <i class=""fa fa-check-circle""></i>
                  </span>
                                    <span class=""time"">Jul 8, 12:16 AM</span>
                                    <span class=""category"">Profile Comments</span>
                </div>
                                <div class=""header-notification-item-content profile_comment"" on-check-item=""function (n){var i=arguments.length;return i?i>1?e.apply(t,arguments):e.call(t,n):e.call(t)}"">
                                    <div>
      <a href=""http://myanimelist.net/profile/zero_omar"">zero_omar</a>
      posted a comment on your profile:
      <q class=""message"">I watched Higurashi no Naku Koro ni and Toaru Majutsu no Index at the same time too :D</q>
    </div>
                  
                </div>
              </div>
            </li><li class=""header-notification-item on_air"">
              <a class=""background"" href=""http://myanimelist.net/anime/season""></a>
              <div class=""inner"">
                <div class=""header-notification-item-header"">
                                    <span class=""is-read"">
                    <i class=""fa fa-check""></i>
                    <i class=""fa fa-check-circle""></i>
                  </span>
                                    <span class=""time"">Jul 7, 4:22 AM</span>
                                    <span class=""category"">Now Airing</span>
                </div>
                                <div class=""header-notification-item-content on_air"" on-check-item=""function (n){var i=arguments.length;return i?i>1?e.apply(t,arguments):e.call(t,n):e.call(t)}"">
                                    <div>
      The anime you plan to watch will begin airing on Jul 8:
      <p class=""ml8 mt4"">
        <a href=""http://myanimelist.net/anime/32648/Handa-kun"">Handa-kun</a>, <a href=""http://myanimelist.net/anime/33091/Planetarian__Chiisana_Hoshi_no_Yume"">Planetarian: Chiisana Hoshi no Yume</a>
      </p>
    </div>
                  
                </div>
              </div>
            </li><li class=""header-notification-item related_anime_add"">
              <a class=""background"" href=""http://myanimelist.net/anime/33524/Sakamoto_desu_ga_Special""></a>
              <div class=""inner"">
                <div class=""header-notification-item-header"">
                                    <span class=""is-read"">
                    <i class=""fa fa-check""></i>
                    <i class=""fa fa-check-circle""></i>
                  </span>
                                    <span class=""time"">Jul 3, 8:11 PM</span>
                                    <span class=""category"">New Related Anime</span>
                </div>
                                <div class=""header-notification-item-content related_anime_add"" on-check-item=""function (n){var i=arguments.length;return i?i>1?e.apply(t,arguments):e.call(t,n):e.call(t)}"">
                                    <div>
      <a href=""http://myanimelist.net/anime/33524/Sakamoto_desu_ga_Special"">Sakamoto desu ga? Special</a>&nbsp;(Special) has just been added to the database.
    </div>
                  
                </div>
              </div>
            </li><li class=""header-notification-item on_air"">
              <a class=""background"" href=""http://myanimelist.net/anime/season""></a>
              <div class=""inner"">
                <div class=""header-notification-item-header"">
                                    <span class=""is-read"">
                    <i class=""fa fa-check""></i>
                    <i class=""fa fa-check-circle""></i>
                  </span>
                                    <span class=""time"">Jul 3, 4:30 AM</span>
                                    <span class=""category"">Now Airing</span>
                </div>
                                <div class=""header-notification-item-content on_air"" on-check-item=""function (n){var i=arguments.length;return i?i>1?e.apply(t,arguments):e.call(t,n):e.call(t)}"">
                                    <div>
      The anime you plan to watch will begin airing on Jul 3:
      <p class=""ml8 mt4"">
        <a href=""http://myanimelist.net/anime/32281/Kimi_no_Na_wa"">Kimi no Na wa.</a>, <a href=""http://myanimelist.net/anime/32729/Orange"">Orange</a>, <a href=""http://myanimelist.net/anime/33558/Tales_of_Zestiria_the_X__Saiyaku_no_Jidai"">Tales of Zestiria the X: Saiyaku no Jidai</a>
      </p>
    </div>
                  
                </div>
              </div>
            </li><li class=""header-notification-item on_air"">
              <a class=""background"" href=""http://myanimelist.net/anime/season""></a>
              <div class=""inner"">
                <div class=""header-notification-item-header"">
                                    <span class=""is-read"">
                    <i class=""fa fa-check""></i>
                    <i class=""fa fa-check-circle""></i>
                  </span>
                                    <span class=""time"">Jul 2, 4:33 AM</span>
                                    <span class=""category"">Now Airing</span>
                </div>
                                <div class=""header-notification-item-content on_air"" on-check-item=""function (n){var i=arguments.length;return i?i>1?e.apply(t,arguments):e.call(t,n):e.call(t)}"">
                                    <div>
      The anime you plan to watch will begin airing on Jul 2:
      <p class=""ml8 mt4"">
        <a href=""http://myanimelist.net/anime/31716/Rewrite"">Rewrite</a>, <a href=""http://myanimelist.net/anime/32282/Shokugeki_no_Souma__Ni_no_Sara"">Shokugeki no Souma: Ni no Sara</a>
      </p>
    </div>
                  
                </div>
              </div>
            </li><li class=""header-notification-item on_air"">
              <a class=""background"" href=""http://myanimelist.net/anime/season""></a>
              <div class=""inner"">
                <div class=""header-notification-item-header"">
                                    <span class=""is-read"">
                    <i class=""fa fa-check""></i>
                    <i class=""fa fa-check-circle""></i>
                  </span>
                                    <span class=""time"">Jul 1, 4:21 AM</span>
                                    <span class=""category"">Now Airing</span>
                </div>
                                <div class=""header-notification-item-content on_air"" on-check-item=""function (n){var i=arguments.length;return i?i>1?e.apply(t,arguments):e.call(t,n):e.call(t)}"">
                                    <div>
      The anime you plan to watch will begin airing on Jul 1:
      <p class=""ml8 mt4"">
        <a href=""http://myanimelist.net/anime/32379/Berserk_2016"">Berserk (2016)</a>
      </p>
    </div>
                  
                </div>
              </div>
            </li>
            
          </ol>
          <div class=""header-notification-view-all"">
            <a href=""http://myanimelist.net/notification"">
              View All
              <span>
                (14)
              </span>
            </a>
          </div>
        </div>
      </div>
    </div></div><script>
window.MAL.headerNotification = {""items"":[{""id"":""8429659"",""typeIdentifier"":""profile_comment"",""categoryName"":""Profile Comments"",""createdAt"":1468915459,""createdAtForDisplay"":""Jul 19, 10:04 AM"",""isRead"":false,""url"":""http:\/\/myanimelist.net\/comtocom.php?id1=4952914&id2=5344716"",""isDeleted"":false,""commentUserName"":""zero_omar"",""commentUserProfileUrl"":""http:\/\/myanimelist.net\/profile\/zero_omar"",""commentUserImageUrl"":""http:\/\/cdn.myanimelist.net\/images\/userimages\/thumbs\/5344716_thumb.jpg"",""text"":""how was index ? :D you might like Railgun S. it's talking about the sisters arc but from Misaka's perspective...""},{""id"":""8422625"",""typeIdentifier"":""forum_quote"",""categoryName"":""Forum Quotes"",""createdAt"":1468884239,""createdAtForDisplay"":""Jul 19, 1:23 AM"",""isRead"":false,""url"":""http:\/\/myanimelist.net\/forum\/message\/46985793?goto=topic"",""isDeleted"":false,""quoteUserName"":""Haruka"",""quoteUserProfileUrl"":""http:\/\/myanimelist.net\/profile\/Haruka"",""topicUrl"":""http:\/\/myanimelist.net\/forum\/?topicid=1499207"",""topicTitle"":""[App] MALClient for Windows 10""},{""id"":""8204208"",""typeIdentifier"":""user_mention_in_forum_message"",""categoryName"":""User Mentions"",""createdAt"":1468685468,""createdAtForDisplay"":""Jul 16, 6:11 PM"",""isRead"":true,""url"":""http:\/\/myanimelist.net\/forum\/message\/46947545?goto=topic"",""isDeleted"":false,""senderName"":""Utagai-"",""senderProfileUrl"":""http:\/\/myanimelist.net\/profile\/Utagai-"",""pageUrl"":""http:\/\/myanimelist.net\/forum\/?topicid=1533972"",""pageTitle"":""I've written a little Python API wrapper for the Official MAL API""},{""id"":""7643503"",""typeIdentifier"":""related_anime_add"",""categoryName"":""New Related Anime"",""createdAt"":1468373279,""createdAtForDisplay"":""Jul 13, 3:27 AM"",""isRead"":false,""url"":""http:\/\/myanimelist.net\/anime\/33569\/Re_Petit_kara_Hajimeru_Isekai_Seikatsu"",""isDeleted"":false,""anime"":{""title"":""Re:Petit kara Hajimeru Isekai Seikatsu"",""mediaType"":""Special""}},{""id"":""7435740"",""typeIdentifier"":""on_air"",""categoryName"":""Now Airing"",""createdAt"":1468204089,""createdAtForDisplay"":""Jul 11, 4:28 AM"",""isRead"":false,""url"":""http:\/\/myanimelist.net\/anime\/season"",""isDeleted"":false,""date"":""Jul 12"",""animes"":[{""title"":""Mob Psycho 100"",""url"":""http:\/\/myanimelist.net\/anime\/32182\/Mob_Psycho_100""}]},{""id"":""7342850"",""typeIdentifier"":""on_air"",""categoryName"":""Now Airing"",""createdAt"":1468117422,""createdAtForDisplay"":""Jul 10, 4:23 AM"",""isRead"":false,""url"":""http:\/\/myanimelist.net\/anime\/season"",""isDeleted"":false,""date"":""Jul 10"",""animes"":[{""title"":""Tales of Zestiria the X"",""url"":""http:\/\/myanimelist.net\/anime\/30911\/Tales_of_Zestiria_the_X""}]},{""id"":""7263431"",""typeIdentifier"":""on_air"",""categoryName"":""Now Airing"",""createdAt"":1468030759,""createdAtForDisplay"":""Jul 9, 4:19 AM"",""isRead"":false,""url"":""http:\/\/myanimelist.net\/anime\/season"",""isDeleted"":false,""date"":""Jul 10"",""animes"":[{""title"":""Ange Vierge"",""url"":""http:\/\/myanimelist.net\/anime\/32171\/Ange_Vierge""},{""title"":""Qualidea Code"",""url"":""http:\/\/myanimelist.net\/anime\/32360\/Qualidea_Code""}]},{""id"":""7228685"",""typeIdentifier"":""profile_comment"",""categoryName"":""Profile Comments"",""createdAt"":1467985260,""createdAtForDisplay"":""Jul 8, 3:41 PM"",""isRead"":false,""url"":""http:\/\/myanimelist.net\/comtocom.php?id1=4952914&id2=5344716"",""isDeleted"":false,""commentUserName"":""zero_omar"",""commentUserProfileUrl"":""http:\/\/myanimelist.net\/profile\/zero_omar"",""commentUserImageUrl"":""http:\/\/cdn.myanimelist.net\/images\/userimages\/thumbs\/5344716_thumb.jpg"",""text"":""release order: index 1 > railgun > index 2 > railgun S cronoligical : index 1 and railgun S happened...""},{""id"":""7205359"",""typeIdentifier"":""on_air"",""categoryName"":""Now Airing"",""createdAt"":1467944745,""createdAtForDisplay"":""Jul 8, 4:25 AM"",""isRead"":false,""url"":""http:\/\/myanimelist.net\/anime\/season"",""isDeleted"":false,""date"":""Jul 9"",""animes"":[{""title"":""91 Days"",""url"":""http:\/\/myanimelist.net\/anime\/32998\/91_Days""}]},{""id"":""7147486"",""typeIdentifier"":""profile_comment"",""categoryName"":""Profile Comments"",""createdAt"":1467929819,""createdAtForDisplay"":""Jul 8, 12:16 AM"",""isRead"":false,""url"":""http:\/\/myanimelist.net\/comtocom.php?id1=4952914&id2=5344716"",""isDeleted"":false,""commentUserName"":""zero_omar"",""commentUserProfileUrl"":""http:\/\/myanimelist.net\/profile\/zero_omar"",""commentUserImageUrl"":""http:\/\/cdn.myanimelist.net\/images\/userimages\/thumbs\/5344716_thumb.jpg"",""text"":""I watched Higurashi no Naku Koro ni and Toaru Majutsu no Index at the same time too :D""},{""id"":""7116864"",""typeIdentifier"":""on_air"",""categoryName"":""Now Airing"",""createdAt"":1467858153,""createdAtForDisplay"":""Jul 7, 4:22 AM"",""isRead"":false,""url"":""http:\/\/myanimelist.net\/anime\/season"",""isDeleted"":false,""date"":""Jul 8"",""animes"":[{""title"":""Handa-kun"",""url"":""http:\/\/myanimelist.net\/anime\/32648\/Handa-kun""},{""title"":""Planetarian: Chiisana Hoshi no Yume"",""url"":""http:\/\/myanimelist.net\/anime\/33091\/Planetarian__Chiisana_Hoshi_no_Yume""}]},{""id"":""6216198"",""typeIdentifier"":""related_anime_add"",""categoryName"":""New Related Anime"",""createdAt"":1467569475,""createdAtForDisplay"":""Jul 3, 8:11 PM"",""isRead"":false,""url"":""http:\/\/myanimelist.net\/anime\/33524\/Sakamoto_desu_ga_Special"",""isDeleted"":false,""anime"":{""title"":""Sakamoto desu ga? Special"",""mediaType"":""Special""}},{""id"":""5995549"",""typeIdentifier"":""on_air"",""categoryName"":""Now Airing"",""createdAt"":1467513013,""createdAtForDisplay"":""Jul 3, 4:30 AM"",""isRead"":false,""url"":""http:\/\/myanimelist.net\/anime\/season"",""isDeleted"":false,""date"":""Jul 3"",""animes"":[{""title"":""Kimi no Na wa."",""url"":""http:\/\/myanimelist.net\/anime\/32281\/Kimi_no_Na_wa""},{""title"":""Orange"",""url"":""http:\/\/myanimelist.net\/anime\/32729\/Orange""},{""title"":""Tales of Zestiria the X: Saiyaku no Jidai"",""url"":""http:\/\/myanimelist.net\/anime\/33558\/Tales_of_Zestiria_the_X__Saiyaku_no_Jidai""}]},{""id"":""5818676"",""typeIdentifier"":""on_air"",""categoryName"":""Now Airing"",""createdAt"":1467426782,""createdAtForDisplay"":""Jul 2, 4:33 AM"",""isRead"":false,""url"":""http:\/\/myanimelist.net\/anime\/season"",""isDeleted"":false,""date"":""Jul 2"",""animes"":[{""title"":""Rewrite"",""url"":""http:\/\/myanimelist.net\/anime\/31716\/Rewrite""},{""title"":""Shokugeki no Souma: Ni no Sara"",""url"":""http:\/\/myanimelist.net\/anime\/32282\/Shokugeki_no_Souma__Ni_no_Sara""}]},{""id"":""5720077"",""typeIdentifier"":""on_air"",""categoryName"":""Now Airing"",""createdAt"":1467339707,""createdAtForDisplay"":""Jul 1, 4:21 AM"",""isRead"":false,""url"":""http:\/\/myanimelist.net\/anime\/season"",""isDeleted"":false,""date"":""Jul 1"",""animes"":[{""title"":""Berserk (2016)"",""url"":""http:\/\/myanimelist.net\/anime\/32379\/Berserk_2016""}]}],""countDigest"":14,""dropdownOpenedAt"":1467965123};
window.MAL.headerNotification.templates = {""root"":""    <header-notification-button\n      :has-new=\""hasNewItems\""\n      :is-dropdown-visible=\""isDropdownVisible\""\n      :on-click=\""toggleDropdown\""\n      :num-whole-unread-items=\""numWholeUnreadItems\"">\n    <\/header-notification-button>\n    <header-notification-dropdown\n      :items=\""items\""\n      :items-checked-in-this-session=\""itemsCheckedInThisSession\""\n      :num-whole-unread-items=\""numWholeUnreadItems\""\n      :dropdown-opened-at=\""dropdownOpenedAt\""\n      :is-visible=\""isDropdownVisible\""\n      :was-dropdown-closed=\""wasDropdownClosed\"">\n    <\/header-notification-dropdown>\n  "",""button"":""    <a class=\""header-notification-button\""\n      :class=\""[(hasUnread ? 'has-unread' : ''), (hasNew ? 'new' : ''), ('text' + numWholeUnreadItemsAsString.length)]\""\n      @click=\""onClick\""\n      @keypress.enter=\""onClick\""\n      :aria-expanded=\""isDropdownVisible ? 'true' : 'false'\""\n      aria-haspopup=\""true\""\n      aria-label=\""Notifications\""\n      title=\""Notifications\""\n      tabindex=\""0\""\n            :data-unread=\""numWholeUnreadItemsAsString\"">\n      <i class=\""fa fa-bell\""><\/i>\n    <\/a>\n  "",""dropdown"":""    <div class=\""header-notification-dropdown\"" v-show=\""isVisible\"">\n      <div class=\""arrow_box\"">\n        <div class=\""header-notification-dropdown-inner\"">\n          <h3>\n            <span class=\""settings ml8\"">\n              <a href=\""\/notification\/setting\"">\n                <i class=\""fa fa-cog mr4\""><\/i>Settings\n              <\/a>\n            <\/span>\n            <span class=\""mark-all\""\n              :class=\""{disabled: !hasUnreadItems}\""\n              @click=\""checkAllItems\"">\n              <i class=\""fa fa-check mr4\""><\/i>Mark All as Read\n            <\/span>\n            Notifications\n          <\/h3>\n          <ol class=\""header-notification-item-list\"">\n            <li class=\""header-notification-item\""\n              :class=\""[item.typeIdentifier, isNewItem(item) ? 'new' : '']\""\n              v-for=\""item in items\"">\n              <a v-if=\""item.url\"" @click=\""checkItem(item)\"" :href=\""item.url\"" class=\""background\""><\/a>\n              <div class=\""inner\""\n                :class=\""{'is-read': item.isRead && !isItemCheckedInThisSession(item)}\"">\n                <div class=\""header-notification-item-header\"">\n                                    <span class=\""is-read\""\n                    v-show=\""!item.isRead || isItemCheckedInThisSession(item)\""\n                    :class=\""{checked: item.isRead}\""\n                    @click=\""checkItem(item)\"">\n                    <i class=\""fa fa-check\""><\/i>\n                    <i class=\""fa fa-check-circle\""><\/i>\n                  <\/span>\n                                    <span class=\""time\"">${item.createdAtForDisplay}<\/span>\n                                    <span class=\""category\"">${item.categoryName}<\/span>\n                <\/div>\n                                <div class=\""header-notification-item-content\""\n                  :class=\""[item.typeIdentifier]\"" :on-check-item=\""checkItem\"">\n                                    <component :is=\""resolveComponent(item)\""\n                    v-if=\""!item.isDeleted\""\n                    :item=\""item\""\n                    >\n                  <\/component>\n                  <div class=\""deleted\"" v-if=\""item.isDeleted\"">(deleted)<\/div>\n                <\/div>\n              <\/div>\n            <\/li>\n            <li class=\""header-notification-item empty\"" v-if=\""items.length === 0\"">\n              <p>No notifications.<\/p>\n            <\/li>\n          <\/ol>\n          <div class=\""header-notification-view-all\"">\n            <a href=\""http:\/\/myanimelist.net\/notification\"">\n              View All\n              <span v-if=\""numWholeUnreadItems > 0\"">\n                (${numWholeUnreadItems &lt; 100 ? numWholeUnreadItems : &quot;99+&quot;})\n              <\/span>\n            <\/a>\n          <\/div>\n        <\/div>\n      <\/div>\n    <\/div>\n  "",""itemFriendRequest"":""    <div>\n      <p>\n        <a :href=\""item.friendProfileUrl\"" class=\""fl-l mr4\""><img :src=\""item.friendImageUrl\"" :alt=\""item.friendName\""><\/a>\n        <a :href=\""item.friendProfileUrl\"">${item.friendName}<\/a>\n        sent you a friend request${item.message.length ? \"":\"" : \"".\""}\n        <q class=\""message\"" v-if=\""item.message.length\"">${item.message}<\/q>\n      <\/p>\n      <p class=\""actions mt8\"">\n        <span v-if=\""!item.isApproved\"">\n          <span class=\""action-button accept\"" @click=\""accept\"">Accept<\/span><span class=\""action-button deny\"" @click=\""deny\"">Deny<\/span><\/span>\n        <span v-if=\""item.isApproved\"">\n          <span class=\""result\"">Accepted<\/span>\n        <\/span>\n      <\/p>\n    <\/div>\n  "",""itemFriendRequestAccept"":""    <p>\n      <a :href=\""item.friendProfileUrl\"" class=\""fl-l mr4\""><img :src=\""item.friendImageUrl\"" :alt=\""item.friendName\""><\/a>\n      <a :href=\""item.friendProfileUrl\"">${item.friendName}<\/a>\n      accepted your friend request.\n    <\/p>\n  "",""itemFriendRequestDeny"":""    <p>\n      <a :href=\""item.friendProfileUrl\"" class=\""fl-l mr4\""><img :src=\""item.friendImageUrl\"" :alt=\""item.friendName\""><\/a>\n      <a :href=\""item.friendProfileUrl\"">${item.friendName}<\/a>\n      denied your friend request.\n    <\/p>\n  "",""itemProfileComment"":""    <div>\n      <a :href=\""item.commentUserProfileUrl\"">${item.commentUserName}<\/a>\n      posted a comment on your profile${item.text.length ? \"":\"" : \"".\""}\n      <q class=\""message\"" v-if=\""item.text.length\"">${item.text}<\/q>\n    <\/div>\n  "",""itemForumQuote"":""    <div>\n      <a :href=\""item.quoteUserProfileUrl\"">${item.quoteUserName}<\/a>\n      quoted your post in the <a :href=\""item.topicUrl\"">${item.topicTitle}<\/a> topic.\n    <\/div>\n  "",""itemBlogComment"":""    <a :href=\""item.commentUserProfileUrl\"">${item.commentUserName}<\/a>\n    posted a comment on your blog.\n  "",""itemWatchedTopicMessage"":""    A forum reply was posted on your watched topic\n    <a :href=\""item.topicUrl\"">${item.topicTitle}<\/a>.\n  "",""itemClubMassMessageInForum"":""    <a :href=\""item.sharedUserProfileUrl\"">${item.sharedUserName}<\/a> shared the discussion\n    <a :href=\""item.topicUrl\"" :title=\""item.topicTitle\"">${item.topicTitle}<\/a>\n    in <a :href=\""item.clubUrl\"" :title=\""item.clubName\"">${item.clubName}<\/a>.\n  "",""itemRelatedAnimeAdd"":""    <div>\n      <a :href=\""item.url\"">${item.anime.title}<\/a>&nbsp;(${item.anime.mediaType}) has just been added to the database.\n    <\/div>\n  "",""itemUserMentions"":""    <div>\n      <a :href=\""item.senderProfileUrl\"">${item.senderName}<\/a>\n      has mentioned your name in the\n      <a :href=\""item.pageUrl\"">${item.pageTitle}<\/a>\n      <span v-text=\""{\n        user_mention_in_forum_message: 'topic',\n        user_mention_in_club_comment: 'club'\n      }[item.typeIdentifier]\""><\/span>.\n    <\/div>\n  "",""itemOnAir"":""    <div>\n      The anime you plan to watch will begin airing on ${item.date}:\n      <p class=\""ml8 mt4\"">\n        <template v-for=\""anime in item.animes\"">\n          <a :href=\""anime.url\"">${anime.title}<\/a><template v-if=\""$index < item.animes.length - 1\"">, <\/template>\n        <\/template>\n      <\/p>\n    <\/div>\n  ""};
</script><div class=""border""></div><div class=""header-menu-unit header-profile js-header-menu-unit link-bg pl8 pr8"" data-id=""profile""><a class=""header-profile-link"">Drutol<i class=""fa fa-caret-down ml4""></i></a><div class=""header-menu-dropdown header-profile-dropdown arrow_box""><ul><li><a href=""http://myanimelist.net/profile/Drutol"">Profile</a></li><li class=""clearfix""><a href=""http://myanimelist.net/myfriends.php"">Friends</a></li><li class=""clearfix""><a href=""http://myanimelist.net/clubs.php?action=myclubs"">Clubs</a></li><li><a href=""http://myanimelist.net/blog/Drutol"">Blog Posts</a></li><li><a href=""http://myanimelist.net/myreviews.php"">Reviews</a></li><li><a href=""http://myanimelist.net/myrecommendations.php"">Recommendations</a></li><li><a href=""http://myanimelist.net/editprofile.php?go=myoptions""><i class=""fa fa-cog mr4""></i>Account Settings</a></li><li><form action=""http://myanimelist.net/logout.php"" method=""post""><a href=""javascript:void(0);"" onclick=""$(this).parent().submit();""><i class=""fa fa-sign-out mr4""></i>Logout</a></form></li></ul></div></div><div class=""header-menu-unit header-profile pl0""><a href=""http://myanimelist.net/profile/Drutol"" class=""header-profile-button"" style=""background-image:url(http://cdn.myanimelist.net/images/userimages/4952914.jpg)"" title=""Drutol""></a></div></div><a href=""/panel.php"" class=""link-mal-logo"">MyAnimeList.net</a>
        </div>
        
                
          <div id=""menu"">
    <div id=""menu_right""><script type=""text/x-template"" id=""incremental-result-item-anime""><div class=""list anime"" :class=""{'focus': focus}""><a :href=""url"" class=""clearfix""><div class=""on"" v-if=""focus""><span class=""image"" :style=""{'background-image': 'url(' + item.image_url + ')'}""></span><div class=""info anime""><div class=""name"">${ item.name } <span class=""media-type"">(${ item.payload.media_type })</span></div><div class=""extra-info"">Aired: ${ item.payload.aired }<br>Score: ${ item.payload.score }<br>Status: ${ item.payload.status }</div></div></div><div class=""off"" v-else><span class=""image"" :style=""{'background-image': 'url(' + item.thumbnail_url + ')'}""></span><div class=""info anime""><div class=""name"">${ item.name }</div><div class=""media-type"">(${ mediaTypeWithStartYear })</div></div></div></a></div></script><script type=""text/x-template"" id=""incremental-result-item-manga""><div class=""list manga"" :class=""{'focus': focus}""><a :href=""url"" class=""clearfix""><div class=""on"" v-if=""focus""><span class=""image"" :style=""{'background-image': 'url(' + item.image_url + ')'}""></span><div class=""info manga""><div class=""name"">${ item.name } <span class=""media-type"">(${ item.payload.media_type })</span></div><div class=""extra-info"">Published: ${ item.payload.published }<br>Score: ${ item.payload.score }<br>Status: ${ item.payload.status }</div></div></div><div class=""off"" v-else><span class=""image"" :style=""{'background-image': 'url(' + item.thumbnail_url + ')'}""></span><div class=""info manga""><div class=""name"">${ item.name }</div><div class=""media-type"">(${ mediaTypeWithStartYear })</div></div></div></a></div></script><script type=""text/x-template"" id=""incremental-result-item-character""><div class=""list character"" :class=""{'focus': focus}""><a :href=""url"" class=""clearfix""><div class=""on"" v-if=""focus""><span class=""image"" :style=""{'background-image': 'url(' + item.image_url + ')'}""></span><div class=""info character""><div class=""name"">${ item.name }</div><div class=""extra-info""><ul class=""related-works""><li v-for=""work in item.payload.related_works"" class=""fs-i"">- ${ work }</li></ul>Favorites: ${ item.payload.favorites }</div></div></div><div class=""off"" v-else><span class=""image"" :style=""{'background-image': 'url(' + item.thumbnail_url + ')'}""></span><div class=""info character""><div class=""name"">${ item.name }</div></div></div></a></div></script><script type=""text/x-template"" id=""incremental-result-item-person""><div class=""list person"" :class=""{'focus': focus}""><a :href=""url"" class=""clearfix""><div class=""on"" v-if=""focus""><span class=""image"" :style=""{'background-image': 'url(' + item.image_url + ')'}""></span><div class=""info person""><div class=""name"">${ item.name }</div><div class=""extra-info""><span v-if=""item.payload.alternative_name"">${ item.payload.alternative_name }<br></span>Birthday: ${ item.payload.birthday }<br>Favorites: ${ item.payload.favorites }</div></div></div><div class=""off"" v-else><span class=""image"" :style=""{'background-image': 'url(' + item.thumbnail_url + ')'}""></span><div class=""info person""><div class=""name"">${ item.name }</div></div></div></a></div></script><script type=""text/x-template"" id=""incremental-result-item-club""><div class=""list club"" :class=""{'focus': focus}""><a :href=""url"" class=""clearfix""><div class=""on"" v-if=""focus""><span class=""image"" :style=""{'background-image': 'url(' + item.image_url + ')'}""></span><div class=""info club""><div class=""name"">${ item.name }</div><div class=""extra-info"">Members: ${ item.payload.members }<br>Category: ${ item.payload.category }<br>Created by: ${ item.payload.created_by }</div></div></div><div class=""off"" v-else><span class=""image"" :style=""{'background-image': 'url(' + item.thumbnail_url + ')'}""></span><div class=""info club""><div class=""name"">${ item.name }</div></div></div></a></div></script><script type=""text/x-template"" id=""incremental-result-item-user""><div class=""list user"" :class=""{'focus': focus}""><a :href=""url"" class=""clearfix""><div class=""on"" v-if=""focus""><span class=""image"" :style=""{'background-image': 'url(' + item.image_url + ')'}""></span><div class=""info user""><div class=""name"">${ item.name }</div><div class=""extra-info""><span v-if=""item.payload.authority"">${ item.payload.authority }<br></span>Joined: ${ item.payload.joined }</div></div></div><div class=""off"" v-else><span class=""image"" :style=""{'background-image': 'url(' + item.thumbnail_url + ')'}""></span><div class=""info user""><div class=""name"">${ item.name }</div></div></div></a></div></script><script type=""text/x-template"" id=""incremental-result-item-news""><div class=""list news"" :class=""{'focus': focus}""><a :href=""url"" class=""clearfix""><div class=""on"" v-if=""focus""><span class=""image"" :style=""{'background-image': 'url(' + item.image_url + ')'}""></span><div class=""info news""><div class=""name"">${ item.name }</div><div class=""extra-info"">${ item.payload.date }</div></div></div><div class=""off"" v-else><span class=""image"" :style=""{'background-image': 'url(' + item.thumbnail_url + ')'}""></span><div class=""info news""><div class=""name"">${ item.name }</div><div class=""media-type"">${ item.payload.date }</div></div></div></a></div></script><script type=""text/x-template"" id=""incremental-result-item-featured""><div class=""list featured"" :class=""{'focus': focus}""><a :href=""url"" class=""clearfix""><div class=""on"" v-if=""focus""><span class=""image"" :style=""{'background-image': 'url(' + item.image_url + ')'}""></span><div class=""info featured""><div class=""name"">${ item.name }</div><div class=""extra-info"">${ item.payload.date }</div></div></div><div class=""off"" v-else><span class=""image"" :style=""{'background-image': 'url(' + item.thumbnail_url + ')'}""></span><div class=""info featured""><div class=""name"">${ item.name }</div><div class=""media-type"">${ item.payload.date }</div></div></div></a></div></script><script type=""text/x-template"" id=""incremental-result-item-forum""><div class=""list forum"" :class=""{'focus': focus}""><a :href=""url"" class=""clearfix""><div class=""on"" v-if=""focus""><span class=""image"" :style=""{'background-image': 'url(' + item.image_url + ')'}""></span><div class=""info forum""><div class=""name""><span v-show=""item.payload.work_title"">${ item.payload.work_title}
                      <i class=""fa fa-caret-right""></i></span> ${ item.name }</div><div class=""extra-info"">${ item.payload.date }<br><span>in ${ item.payload.category }</span></div></div></div><div class=""off"" v-else><span class=""image"" :style=""{'background-image': 'url(' + item.thumbnail_url + ')'}""></span><div class=""info forum""><div class=""name""><span v-show=""item.payload.work_title"">${ item.payload.work_title}
                      <i class=""fa fa-caret-right""></i></span> ${ item.name }</div><div class=""media-type"">${ item.payload.date }</div></div></div></a></div></script><script type=""text/x-template"" id=""incremental-result-item-separator""><div class=""list separator""><div class=""separator""><span v-show=""item.name == 'anime'"">Anime</span><span v-show=""item.name == 'manga'"">Manga</span><span v-show=""item.name == 'character'"">Characters</span><span v-show=""item.name == 'person'"">People</span></div></div></script><div id=""top-search-bar""><form id=""searchBar"" method=""get"" class=""searchBar"" @submit.prevent=""jump()""><div class=""form-select-outer fl-l""><select name=""type"" id=""topSearchValue"" class=""inputtext"" v-model=""type""><option value=""all"">All</option><option value=""anime"">Anime</option><option value=""manga"">Manga</option><option value=""character"">Characters</option><option value=""person"">People</option><option value=""news"">News</option><option value=""featured"">Featured Articles</option><option value=""forum"">Forum</option><option value=""club"">Clubs</option><option value=""user"">Users</option></select></div><input v-model=""keyword"" id=""topSearchText"" type=""text"" name=""keyword"" class=""inputtext fl-l"" placeholder=""Search Anime, Manga, and more..."" size=""30"" autocomplete=""off"" @keydown.up.prevent=""moveSelection(-1)"" @keydown.down.prevent=""moveSelection(1)"" @focus=""isFocused = true"" @blur=""isFocused = false""><input id=""topSearchButon"" class=""fl-l"" :class=""{'notActive': (keyword.length < 3)}"" type=""submit"" value=""""></form><div id=""topSearchResultList"" class=""incrementalSearchResultList"" :style=""{display: (showResult ? 'block' : 'none')}"" @mousedown.prevent=""""><component v-for=""item in items"" :is=""resolveComponent(item)"" :item=""item"" :focus=""selection == $index"" :url=""generateItemPageUrl(item)"" @mouseover=""selection = $index""></component><div class=""list list-bottom"" :class=""{'focus': selection == -1}"" @mouseover=""selection = -1"" :style=""{display: (showViewAllLink ? 'block' : 'none')}""><a :href=""resultPageUrl"">
              View all results for <span class=""fw-b"">${ keyword }</span><i v-show=""currentAjaxRequest != null"" class=""fa fa-spinner fa-spin ml4""></i></a></div></div></div></div><div id=""menu_left"">
      <ul id=""nav"">
        <li class=""small""><a href=""#"" class=""non-link"">Anime</a>
          <ul class=""wider"">
            <li><a href=""http://myanimelist.net/anime.php?_location=mal_h_m"">Anime Search</a></li>
            <li><a href=""http://myanimelist.net/topanime.php?_location=mal_h_m"">Top Anime</a></li>
            <li><a href=""http://myanimelist.net/anime/season?_location=mal_h_m"">Seasonal Anime</a></li>
                        <li><a href=""http://myanimelist.net/watch/episode?_location=mal_h_m_a"">Videos</a></li>
            <li><a href=""http://myanimelist.net/reviews.php?t=anime&amp;_location=mal_h_m"">Reviews</a></li>
            <li><a href=""http://myanimelist.net/recommendations.php?s=recentrecs&amp;t=anime&amp;_location=mal_h_m"">Recommendations</a></li>
          </ul>
        </li>
        <li class=""small""><a href=""#"" class=""non-link"">Manga</a>
          <ul class=""wider"">
            <li><a href=""http://myanimelist.net/manga.php?_location=mal_h_m"">Manga Search</a></li>
            <li><a href=""http://myanimelist.net/topmanga.php?_location=mal_h_m"">Top Manga</a></li>
            <li><a href=""http://myanimelist.net/reviews.php?t=manga&amp;_location=mal_h_m"">Reviews</a></li>
            <li><a href=""http://myanimelist.net/recommendations.php?s=recentrecs&amp;t=manga&amp;_location=mal_h_m"">Recommendations</a></li>
          </ul>
        </li>
        <li><a href=""#"" class=""non-link"">Community</a>
          <ul>
            <li><a href=""http://myanimelist.net/forum/?_location=mal_h_m"">Forums</a></li>
            <li><a href=""http://myanimelist.net/clubs.php?_location=mal_h_m"">Clubs</a></li>
            <li><a href=""http://myanimelist.net/blog.php?_location=mal_h_m"">Blogs</a></li>
            <li><a href=""http://myanimelist.net/users.php?_location=mal_h_m"">Users</a></li>
          </ul>
        </li>
        <li class=""small2""><a href=""#"" class=""non-link"">Industry</a>
          <ul class=""wider"">
            <li><a href=""http://myanimelist.net/news?_location=mal_h_m"">News</a></li>
            <li><a href=""http://myanimelist.net/featured?_location=mal_h_m"">Featured Articles</a></li>
            <li><a href=""http://myanimelist.net/people.php?_location=mal_h_m"">People</a></li>
            <li><a href=""http://myanimelist.net/character.php?_location=mal_h_m"">Characters</a></li>
          </ul>
        </li>
        <li class=""small""><a href=""#"" class=""non-link"">Watch</a>
          <ul class=""wider"" style=""display: none;"">
            <li class=""""><a href=""http://myanimelist.net/watch/episode?_location=mal_h_m"">Episode Videos</a></li>
            <li class=""""><a href=""http://myanimelist.net/watch/promotion?_location=mal_h_m"">Promotional Videos</a></li>
            <li><a href=""http://myanimelist.net/watch/special?_location=mal_h_m"">Special Videos</a></li>
          </ul>
        </li>
        <li class=""smaller""><a href=""#"" class=""non-link"">Help</a>
          <ul class=""wide"" style=""display: none;"">
            <li><a href=""http://myanimelist.net/about.php?_location=mal_h_m"">About</a></li>
            <li><a href=""http://myanimelist.net/about.php?go=support&amp;_location=mal_h_m"">Support</a></li>
            <li><a href=""http://myanimelist.net/advertising?_location=mal_h_m"">Advertising</a></li>
            <li><a href=""http://myanimelist.net/forum/?topicid=515949&amp;_location=mal_h_m"">FAQ</a></li>
            <li><a href=""http://myanimelist.net/modules.php?go=report&amp;_location=mal_h_m"">Report</a></li>
            <li><a href=""http://myanimelist.net/about.php?go=team&amp;_location=mal_h_m"">Staff</a></li>
          </ul>
        </li>
              </ul>
    </div>  </div>        <div id=""contentWrapper"">
          <div>
            <a class=""header-right"" href=""http://myanimelist.net/editprofile.php?go=forumoptions""><i class=""fa fa-cog mr4""></i>Forum Settings</a><h1 class=""h1"">Forums</h1>          </div>
          <div id=""content"">

  <div class=""content-container"">

    <div class=""container-left"">
      


            <div class=""breadcrumb mb12"" itemscope="""" itemtype=""http://schema.org/BreadcrumbList""><div class=""di-ib"" itemprop=""itemListElement"" itemscope="""" itemtype=""http://schema.org/ListItem""><a href=""http://myanimelist.net/"" itemprop=""item""><span itemprop=""name"">
              Top
            </span></a><meta itemprop=""position"" content=""1""></div>&nbsp; &gt; &nbsp;<div class=""di-ib"" itemprop=""itemListElement"" itemscope="""" itemtype=""http://schema.org/ListItem""><a href=""http://myanimelist.net/forum/"" itemprop=""item""><span itemprop=""name"">
              Forum
            </span></a><meta itemprop=""position"" content=""2""></div></div>



      <div class=""forum-board-list pb16"">
          <div class=""forum-header"">MyAnimeList</div>
          <div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-bullhorn fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=5"" class=""forum-board-title"">Updates &amp; Announcements</a><br>
                                    <span class=""forum-board-description"">Updates, changes, and additions to MAL.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/SleepiBunny"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/5384341.jpg"" width=""20"" height=""25"" alt=""SleepiBunny"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535131"" class=""topic-title-link"" title=""Review Mod Team Expands: Part 1"">Review Mod Team Expands: Part 1</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">5 hours ago by <a href=""http://myanimelist.net/profile/SleepiBunny"">SleepiBunny</a> <a href=""http://myanimelist.net/forum/?topicid=1535131&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/kittyJ"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/4556671.png"" width=""20"" height=""25"" alt=""kittyJ"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1528725"" class=""topic-title-link"" title=""MyAnimeList at Anime Expo, on SmartNews App"">MyAnimeList at Anime Expo, on SmartNews App</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">Yesterday, 4:20 AM by <a href=""http://myanimelist.net/profile/kittyJ"">kittyJ</a> <a href=""http://myanimelist.net/forum/?topicid=1528725&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-gavel fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=14"" class=""forum-board-title"">MAL Guidelines &amp; FAQ</a><br>
                                    <span class=""forum-board-description"">Site rules, forum rules, database guidelines, review/recommendation guidelines, and other helpful information.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/Kineta"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/9108.png"" width=""20"" height=""25"" alt=""Kineta"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=516059"" class=""topic-title-link"" title=""Site &amp;amp; Forum Guidelines"">Site &amp; Forum Guidelines</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">May 1, 2014 10:19 PM by <a href=""http://myanimelist.net/profile/Kineta"">Kineta</a> <a href=""http://myanimelist.net/forum/?topicid=516059&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/Kineta"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/9108.png"" width=""20"" height=""25"" alt=""Kineta"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1435506"" class=""topic-title-link"" title=""How to Become a MAL Moderator"">How to Become a MAL Moderator</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">May 19, 10:39 PM by <a href=""http://myanimelist.net/profile/Kineta"">Kineta</a> <a href=""http://myanimelist.net/forum/?topicid=1435506&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-pencil-square-o fn-grey5 mr4 fs14""></i>
                                      <span class=""fs12 fw-b"">DB Modification Requests:</span><br>
                    <span class=""forum-subboards"">
                                          <a href=""http://myanimelist.net/forum/?subboard=2"">Anime DB</a>,&nbsp;                      <a href=""http://myanimelist.net/forum/?subboard=3"">Character &amp; People DB</a>,&nbsp;                      <a href=""http://myanimelist.net/forum/?subboard=5"">Manga DB</a>                    </span>
                                    <span class=""forum-board-description"">Ask questions or submit changes (that you are unable to edit on the entry page) in the applicable board.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/kuuderes_shadow"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/1048329.gif"" width=""20"" height=""25"" alt=""kuuderes_shadow"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535697"" class=""topic-title-link"" title=""100,000"">100,000</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">45 minutes ago by <a href=""http://myanimelist.net/profile/kuuderes_shadow"">kuuderes_shadow</a> <a href=""http://myanimelist.net/forum/?topicid=1535697&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/Jerkhov"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/3824613.jpg"" width=""20"" height=""25"" alt=""Jerkhov"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1510950"" class=""topic-title-link"" title=""Splitting Pokemon Special into separate entries"">Splitting Pokemon Special into separate entries</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">5 hours ago by <a href=""http://myanimelist.net/profile/Jerkhov"">Jerkhov</a> <a href=""http://myanimelist.net/forum/?topicid=1510950&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-life-ring fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=3"" class=""forum-board-title"">Support</a><br>
                                    <span class=""forum-board-description"">Have a problem using the site or think you found a bug? Post here.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/Akarin"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/477008.png"" width=""20"" height=""25"" alt=""Akarin"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535704"" class=""topic-title-link"" title=""Recommendations alignment issue"">Recommendations alignment issue</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">10 minutes ago by <a href=""http://myanimelist.net/profile/Akarin"">Akarin</a> <a href=""http://myanimelist.net/forum/?topicid=1535704&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/hpsthemaskedman"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/questionmark_50.gif"" width=""20"" height=""25"" alt=""hpsthemaskedman"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535354"" class=""topic-title-link"" title=""about the new mobile site design "">about the new mobile site design </a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">2 hours ago by <a href=""http://myanimelist.net/profile/hpsthemaskedman"">hpsthemaskedman</a> <a href=""http://myanimelist.net/forum/?topicid=1535354&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-lightbulb-o fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=4"" class=""forum-board-title"">Suggestions</a><br>
                                    <span class=""forum-board-description"">Have an idea or suggestion for the site? Share it here.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/ultravigo"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/4785069.gif"" width=""20"" height=""25"" alt=""ultravigo"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=255271"" class=""topic-title-link"" title=""Security: Add SSL/TSL/HTTPS Support"">Security: Add SSL/TSL/HTTPS Support</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">26 minutes ago by <a href=""http://myanimelist.net/profile/ultravigo"">ultravigo</a> <a href=""http://myanimelist.net/forum/?topicid=255271&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/Iizbakaokay"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/4146885.jpg"" width=""20"" height=""25"" alt=""Iizbakaokay"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535671"" class=""topic-title-link"" title=""HTTPS?"">HTTPS?</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">3 hours ago by <a href=""http://myanimelist.net/profile/Iizbakaokay"">Iizbakaokay</a> <a href=""http://myanimelist.net/forum/?topicid=1535671&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-trophy fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=13"" class=""forum-board-title"">MAL Contests</a><br>
                                    <span class=""forum-board-description"">Our season-long anime game and other user competitions can be found here.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/niilo"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/questionmark_50.gif"" width=""20"" height=""25"" alt=""niilo"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1533873"" class=""topic-title-link"" title=""Best Signature Design - July 2016 | Voting"">Best Signature Design - July 2016 | Voting</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">10 hours ago by <a href=""http://myanimelist.net/profile/niilo"">niilo</a> <a href=""http://myanimelist.net/forum/?topicid=1533873&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/FiiFO"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/18380.png"" width=""20"" height=""25"" alt=""FiiFO"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1533875"" class=""topic-title-link"" title=""GFX Signature Contest - July 2016 | Voting"">GFX Signature Contest - July 2016 | Voting</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">Jul 19, 5:02 PM by <a href=""http://myanimelist.net/profile/FiiFO"">FiiFO</a> <a href=""http://myanimelist.net/forum/?topicid=1533875&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div></div>
        </div><div class=""forum-board-list pb16"">
          <div class=""forum-header"">Anime &amp; Manga</div>
          <div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-newspaper-o fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=15"" class=""forum-board-title"">News Discussion</a><br>
                                    <span class=""forum-board-description"">Current news in anime and manga.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/Hatsuyuki"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/455244.gif"" width=""20"" height=""25"" alt=""Hatsuyuki"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535543"" class=""topic-title-link"" title=""Anime Movie 'Yowamushi Pedal: Spare Bike' Announces New Cast Members"">Anime Movie 'Yowamushi Pedal: Spare Bike' Announces New Cast Members</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">42 minutes ago by <a href=""http://myanimelist.net/profile/Hatsuyuki"">Hatsuyuki</a> <a href=""http://myanimelist.net/forum/?topicid=1535543&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/NanDemoNai_"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/5282862.jpg"" width=""20"" height=""25"" alt=""NanDemoNai_"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535507"" class=""topic-title-link"" title=""TV Anime 'Udon no Kuni no Kiniro Kemari' Adds Additional Cast Members"">TV Anime 'Udon no Kuni no Kiniro Kemari' Adds Additional Cast Members</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">43 minutes ago by <a href=""http://myanimelist.net/profile/NanDemoNai_"">NanDemoNai_</a> <a href=""http://myanimelist.net/forum/?topicid=1535507&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-gift fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=16"" class=""forum-board-title"">Anime &amp; Manga Recommendations</a><br>
                                    <span class=""forum-board-description"">Ask the community for series recommendations or help other users looking for suggestions.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/Blue_Takeru"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/5382913.jpg"" width=""20"" height=""25"" alt=""Blue_Takeru"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535615"" class=""topic-title-link"" title=""Looking for a great animes, no bad animation and for it to have hardships. "">Looking for a great animes, no bad animation and for it to have hardships. </a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">54 seconds ago by <a href=""http://myanimelist.net/profile/Blue_Takeru"">Blue_Takeru</a> <a href=""http://myanimelist.net/forum/?topicid=1535615&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/Azeraz"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/5124149.jpg"" width=""20"" height=""25"" alt=""Azeraz"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535332"" class=""topic-title-link"" title=""Anime with OP MC"">Anime with OP MC</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">13 minutes ago by <a href=""http://myanimelist.net/profile/Azeraz"">Azeraz</a> <a href=""http://myanimelist.net/forum/?topicid=1535332&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-folder-o fn-grey5 mr4 fs14""></i>
                                      <span class=""fs12 fw-b"">Series Discussion:</span><br>
                    <span class=""forum-subboards"">
                                          <a href=""http://myanimelist.net/forum/?subboard=1"">Anime Series</a>,&nbsp;                      <a href=""http://myanimelist.net/forum/?subboard=4"">Manga Series</a>                    </span>
                                    <span class=""forum-board-description"">Post in episode and chapter discussion threads or talk about specific anime and manga in their series' boards.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/ichii_1"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/3144051.jpg"" width=""20"" height=""25"" alt=""ichii_1"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535569"" class=""topic-title-link"" title=""One Piece Chapter 833 Discussion"">One Piece Chapter 833 Discussion</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">28 seconds ago by <a href=""http://myanimelist.net/profile/ichii_1"">ichii_1</a> <a href=""http://myanimelist.net/forum/?topicid=1535569&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/thebrentinator24"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/3853489.jpg"" width=""20"" height=""25"" alt=""thebrentinator24"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535532"" class=""topic-title-link"" title=""Boku no Hero Academia Chapter 100 Discussion"">Boku no Hero Academia Chapter 100 Discussion</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">3 minutes ago by <a href=""http://myanimelist.net/profile/thebrentinator24"">thebrentinator24</a> <a href=""http://myanimelist.net/forum/?topicid=1535532&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-television fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=1"" class=""forum-board-title"">Anime Discussion</a><br>
                                    <span class=""forum-board-description"">General anime discussion that is not specific to any particular series.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/Dishonest"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/5196955.jpg"" width=""20"" height=""25"" alt=""Dishonest"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535712"" class=""topic-title-link"" title=""Do you have any guilty pleasure anime?"">Do you have any guilty pleasure anime?</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">4 seconds ago by <a href=""http://myanimelist.net/profile/Dishonest"">Dishonest</a> <a href=""http://myanimelist.net/forum/?topicid=1535712&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/MidnightEarth101"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/questionmark_50.gif"" width=""20"" height=""25"" alt=""MidnightEarth101"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535700"" class=""topic-title-link"" title=""What anime has the worst fanbase ?"">What anime has the worst fanbase ?</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">13 seconds ago by <a href=""http://myanimelist.net/profile/MidnightEarth101"">MidnightEarth101</a> <a href=""http://myanimelist.net/forum/?topicid=1535700&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-book fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=2"" class=""forum-board-title"">Manga Discussion</a><br>
                                    <span class=""forum-board-description"">General manga discussion that is not specific to any particular series.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/TFO1013"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/3926591.jpg"" width=""20"" height=""25"" alt=""TFO1013"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=405937"" class=""topic-title-link"" title=""Whats the worst drawn manga you've read?"">Whats the worst drawn manga you've read?</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">11 minutes ago by <a href=""http://myanimelist.net/profile/TFO1013"">TFO1013</a> <a href=""http://myanimelist.net/forum/?topicid=405937&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/TFO1013"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/3926591.jpg"" width=""20"" height=""25"" alt=""TFO1013"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1534473"" class=""topic-title-link"" title=""Buying new or used manga volumes?"">Buying new or used manga volumes?</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">30 minutes ago by <a href=""http://myanimelist.net/profile/TFO1013"">TFO1013</a> <a href=""http://myanimelist.net/forum/?topicid=1534473&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div></div>
        </div><div class=""forum-board-list pb16"">
          <div class=""forum-header"">General</div>
          <div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-comment-o fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=8"" class=""forum-board-title"">Introductions</a><br>
                                    <span class=""forum-board-description"">New to MyAnimeList? Introduce yourself here.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/Bookerhooker"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/questionmark_50.gif"" width=""20"" height=""25"" alt=""Bookerhooker"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535407"" class=""topic-title-link"" title=""Just joined! "">Just joined! </a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">37 minutes ago by <a href=""http://myanimelist.net/profile/Bookerhooker"">Bookerhooker</a> <a href=""http://myanimelist.net/forum/?topicid=1535407&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/WolfyFruitChews"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/questionmark_50.gif"" width=""20"" height=""25"" alt=""WolfyFruitChews"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535607"" class=""topic-title-link"" title=""My Inspiration "">My Inspiration </a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">49 minutes ago by <a href=""http://myanimelist.net/profile/WolfyFruitChews"">WolfyFruitChews</a> <a href=""http://myanimelist.net/forum/?topicid=1535607&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-gamepad fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=7"" class=""forum-board-title"">Games, Computers &amp; Tech Support</a><br>
                                    <span class=""forum-board-description"">Discuss visual novels and other video games, or ask our community a computer related question.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/TsunDesu"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/5405886.png"" width=""20"" height=""25"" alt=""TsunDesu"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1525566"" class=""topic-title-link"" title=""What player do you use for watching anime?"">What player do you use for watching anime?</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">42 minutes ago by <a href=""http://myanimelist.net/profile/TsunDesu"">TsunDesu</a> <a href=""http://myanimelist.net/forum/?topicid=1525566&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/Noboru"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/131771.png"" width=""20"" height=""25"" alt=""Noboru"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1533275"" class=""topic-title-link"" title=""Which Pokemon GO team did you choose? Why?"">Which Pokemon GO team did you choose? Why?</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">1 hour ago by <a href=""http://myanimelist.net/profile/Noboru"">Noboru</a> <a href=""http://myanimelist.net/forum/?topicid=1533275&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-music fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=10"" class=""forum-board-title"">Music &amp; Entertainment</a><br>
                                    <span class=""forum-board-description"">Asian music and live-action series, Western media and artists, best-selling novels, etc.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/robiu013"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/4010353.png"" width=""20"" height=""25"" alt=""robiu013"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1534530"" class=""topic-title-link"" title=""Anyone here into progressive rock?"">Anyone here into progressive rock?</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">15 minutes ago by <a href=""http://myanimelist.net/profile/robiu013"">robiu013</a> <a href=""http://myanimelist.net/forum/?topicid=1534530&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/ASMR"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/questionmark_50.gif"" width=""20"" height=""25"" alt=""ASMR"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1531960"" class=""topic-title-link"" title=""Can't get into Disney/ Pixar"">Can't get into Disney/ Pixar</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">3 hours ago by <a href=""http://myanimelist.net/profile/ASMR"">ASMR</a> <a href=""http://myanimelist.net/forum/?topicid=1531960&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-glass fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=6"" class=""forum-board-title"">Current Events</a><br>
                                    <span class=""forum-board-description"">World headlines, the latest in science, sports competitions, and other debate topics.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/sleeplesstown"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/560459.png"" width=""20"" height=""25"" alt=""sleeplesstown"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535657"" class=""topic-title-link"" title=""Police shoot unarmed man when officer was questioned as to why he did it he said &quot;I don't know&quot;"">Police shoot unarmed man when officer was questioned as to why he did it he said ""I don't know""</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">33 seconds ago by <a href=""http://myanimelist.net/profile/sleeplesstown"">sleeplesstown</a> <a href=""http://myanimelist.net/forum/?topicid=1535657&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/SetsukoHara"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/1539843.jpg"" width=""20"" height=""25"" alt=""SetsukoHara"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535393"" class=""topic-title-link"" title=""Pokemon Go Receives Fatwa in Saudi Arabia"">Pokemon Go Receives Fatwa in Saudi Arabia</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">39 seconds ago by <a href=""http://myanimelist.net/profile/SetsukoHara"">SetsukoHara</a> <a href=""http://myanimelist.net/forum/?topicid=1535393&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-coffee fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=11"" class=""forum-board-title"">Casual Discussion</a><br>
                                    <span class=""forum-board-description"">General interest topics that don't fall into one of the sub-categories above, such as community polls.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/Erg_Orgy"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/5511480.png"" width=""20"" height=""25"" alt=""Erg_Orgy"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535708"" class=""topic-title-link"" title=""Do you have positive or negative feelings towards Black Lives Matter?"">Do you have positive or negative feelings towards Black Lives Matter?</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">5 minutes ago by <a href=""http://myanimelist.net/profile/Erg_Orgy"">Erg_Orgy</a> <a href=""http://myanimelist.net/forum/?topicid=1535708&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/traed"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/46214.jpg"" width=""20"" height=""25"" alt=""traed"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535548"" class=""topic-title-link"" title=""Donald Trump"">Donald Trump</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">12 minutes ago by <a href=""http://myanimelist.net/profile/traed"">traed</a> <a href=""http://myanimelist.net/forum/?topicid=1535548&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-picture-o fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=12"" class=""forum-board-title"">Creative Corner</a><br>
                                    <span class=""forum-board-description"">Show your creations to get help or feedback from our community. Graphics, list designs, stories; anything goes.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/Raptors0verlord"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/5518748.png"" width=""20"" height=""25"" alt=""Raptors0verlord"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535249"" class=""topic-title-link"" title=""My light novel..."">My light novel...</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">2 hours ago by <a href=""http://myanimelist.net/profile/Raptors0verlord"">Raptors0verlord</a> <a href=""http://myanimelist.net/forum/?topicid=1535249&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/Raptors0verlord"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/5518748.png"" width=""20"" height=""25"" alt=""Raptors0verlord"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535675"" class=""topic-title-link"" title=""Publisher Recommendation...!"">Publisher Recommendation...!</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">3 hours ago by <a href=""http://myanimelist.net/profile/Raptors0verlord"">Raptors0verlord</a> <a href=""http://myanimelist.net/forum/?topicid=1535675&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div><div class=""forum-board"">
                <div class=""board"">
                  <i class=""fa fa-puzzle-piece fn-grey5 mr4 fs14""></i>
                                      <a href=""http://myanimelist.net/forum/?board=9"" class=""forum-board-title"">Forum Games</a><br>
                                    <span class=""forum-board-description"">Fun forum games are contained here.</span>
                </div>
                <ul class=""topics"">                                                                                    <li class=""clearfix mb12"">
                        <a href=""http://myanimelist.net/profile/Gem"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/4741523.png"" width=""20"" height=""25"" alt=""Gem"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1528603"" class=""topic-title-link"" title=""Ask the person below you a question. v12"">Ask the person below you a question. v12</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">1 second ago by <a href=""http://myanimelist.net/profile/Gem"">Gem</a> <a href=""http://myanimelist.net/forum/?topicid=1528603&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                                                                                                        <li class=""clearfix"">
                        <a href=""http://myanimelist.net/profile/Temmie"" class=""user_thumb fl-l mr8 lh10""><img src=""http://myanimelist.net/images/useravatars/3800929.jpg"" width=""20"" height=""25"" alt=""Temmie"">
                        </a>
                        <div class=""topic-title di-b"">
                          <a href=""http://myanimelist.net/forum/?topicid=1535527"" class=""topic-title-link"" title=""Hillary vs Trump"">Hillary vs Trump</a><br>
                          <span class=""date di-ib pt4 fs10 fn-grey4"">5 seconds ago by <a href=""http://myanimelist.net/profile/Temmie"">Temmie</a> <a href=""http://myanimelist.net/forum/?topicid=1535527&amp;goto=lastpost"">»»</a></span>
                       </div>
                      </li>
                    </ul>
              </div></div>
        </div><div class=""di-b pb16 ar clearfix"">
        <a href=""http://myanimelist.net/forum/?action=setread"" class=""btn-rect-grey1 pl4 mr8""><i class=""fa fa-check mr4 fs11""></i>Mark All Read</a>
      </div>

      <div class=""forum-mods-list pb16"">
        <div class=""forum-header"">Forum Moderators</div>
        <div class=""forum-mods-container""><ul class=""forum-mods clearfix"">
                              <li class=""link-forum-mods online"">
                  <a href=""http://myanimelist.net/profile/_Ghost_"" class=""link"">
                    <span class=""name"">_Ghost_</span><span class=""thumb"" style=""background-image:url(http://cdn.myanimelist.net/r/80x100/images/useravatars/2107667.gif?s=ea8683349dede954455e858b3c7dc296)"" alt=""_Ghost_""></span>
                  </a>
                </li>                <li class=""link-forum-mods online"">
                  <a href=""http://myanimelist.net/profile/Ardanaz"" class=""link"">
                    <span class=""name"">Ardanaz</span><span class=""thumb"" style=""background-image:url(http://cdn.myanimelist.net/r/80x100/images/useravatars/4022899.png?s=88505e4afe264bc41bc3260dc8bbf5ee)"" alt=""Ardanaz""></span>
                  </a>
                </li>                <li class=""link-forum-mods offline"">
                  <a href=""http://myanimelist.net/profile/Aversa"" class=""link"">
                    <span class=""name"">Aversa</span><span class=""thumb"" style=""background-image:url(http://cdn.myanimelist.net/r/80x100/images/useravatars/317260.png?s=cbe48ad14c789188950b9d261a10be09)"" alt=""Aversa""></span>
                  </a>
                </li>                <li class=""link-forum-mods offline"">
                  <a href=""http://myanimelist.net/profile/julyan"" class=""link"">
                    <span class=""name"">julyan</span><span class=""thumb"" style=""background-image:url(http://cdn.myanimelist.net/r/80x100/images/useravatars/1480941.png?s=4ad4a5bd2205e9c46aeddb429e170ae6)"" alt=""julyan""></span>
                  </a>
                </li>                <li class=""link-forum-mods offline"">
                  <a href=""http://myanimelist.net/profile/sarroush"" class=""link"">
                    <span class=""name"">sarroush</span><span class=""thumb"" style=""background-image:url(http://cdn.myanimelist.net/r/80x100/images/useravatars/124967.jpg?s=d1c9f63c3684330d7a959413f30ae708)"" alt=""sarroush""></span>
                  </a>
                </li>                <li class=""link-forum-mods offline"">
                  <a href=""http://myanimelist.net/profile/shawnofthedeadz"" class=""link"">
                    <span class=""name"">shawnofthedeadz</span><span class=""thumb"" style=""background-image:url(http://cdn.myanimelist.net/r/80x100/images/useravatars/3304805.gif?s=c47d42e1236149877b3041e3064cc4b4)"" alt=""shawnofthedeadz""></span>
                  </a>
                </li>                <li class=""link-forum-mods offline"">
                  <a href=""http://myanimelist.net/profile/Shocked"" class=""link"">
                    <span class=""name"">Shocked</span><span class=""thumb"" style=""background-image:url(http://cdn.myanimelist.net/r/80x100/images/useravatars/280764.jpg?s=d53e80eccef2c513cb1bbf3dc52087de)"" alt=""Shocked""></span>
                  </a>
                </li>                <li class=""link-forum-mods offline"">
                  <a href=""http://myanimelist.net/profile/Tensho"" class=""link"">
                    <span class=""name"">Tensho</span><span class=""thumb"" style=""background-image:url(http://cdn.myanimelist.net/r/80x100/images/useravatars/323518.png?s=48e0a96df255735ed9af39df5c734bc8)"" alt=""Tensho""></span>
                  </a>
                </li>                <li class=""link-forum-mods offline"">
                  <a href=""http://myanimelist.net/profile/Tyrel"" class=""link"">
                    <span class=""name"">Tyrel</span><span class=""thumb"" style=""background-image:url(http://cdn.myanimelist.net/r/80x100/images/useravatars/611391.gif?s=950a647ccdfbd096cce6e9e4fde4e6f8)"" alt=""Tyrel""></span>
                  </a>
                </li>                <li class=""link-forum-mods offline"">
                  <a href=""http://myanimelist.net/profile/Zelot"" class=""link"">
                    <span class=""name"">Zelot</span><span class=""thumb"" style=""background-image:url(http://cdn.myanimelist.net/r/80x100/images/useravatars/2686267.gif?s=2bd42663e7eab2db93c7f8731590406d)"" alt=""Zelot""></span>
                  </a>
                </li></ul></div>
      </div>

              <div class=""clearfix pt24"" style=""width:720px;"">
    <div class=""fl-l"">
    

<div class=""_unit "" style=""width:336px;display: block !important;"" data-height=""280"">
  <div id=""pc_forum_top_bottom_rec_l"" class="""" style=""width: 336px;""><div id=""google_ads_iframe_/84947469/pc_forum_top_bottom_rec_l_0__container__"" style=""border: 0pt none;""><iframe id=""google_ads_iframe_/84947469/pc_forum_top_bottom_rec_l_0"" title=""3rd party ad content"" name=""google_ads_iframe_/84947469/pc_forum_top_bottom_rec_l_0"" width=""300"" height=""250"" scrolling=""no"" marginwidth=""0"" marginheight=""0"" frameborder=""0"" src=""javascript:&quot;<html><body style='background:transparent'></body></html>&quot;"" style=""border: 0px; vertical-align: bottom;""></iframe></div><iframe id=""google_ads_iframe_/84947469/pc_forum_top_bottom_rec_l_0__hidden__"" title="""" name=""google_ads_iframe_/84947469/pc_forum_top_bottom_rec_l_0__hidden__"" width=""0"" height=""0"" scrolling=""no"" marginwidth=""0"" marginheight=""0"" frameborder=""0"" src=""javascript:&quot;<html><body style='background:transparent'></body></html>&quot;"" style=""border: 0px; vertical-align: bottom; visibility: hidden; display: none;""></iframe></div>
</div>
  </div>
      <div class=""fl-r"">
    

<div class=""_unit "" style=""width:336px;display: block !important;"" data-height=""280"">
  <div id=""pc_forum_top_bottom_rec_r"" class="""" style=""width: 336px;""><div id=""google_ads_iframe_/84947469/pc_forum_top_bottom_rec_r_0__container__"" style=""border: 0pt none;""><iframe id=""google_ads_iframe_/84947469/pc_forum_top_bottom_rec_r_0"" title=""3rd party ad content"" name=""google_ads_iframe_/84947469/pc_forum_top_bottom_rec_r_0"" width=""300"" height=""250"" scrolling=""no"" marginwidth=""0"" marginheight=""0"" frameborder=""0"" src=""javascript:&quot;<html><body style='background:transparent'></body></html>&quot;"" style=""border: 0px; vertical-align: bottom;""></iframe></div><iframe id=""google_ads_iframe_/84947469/pc_forum_top_bottom_rec_r_0__hidden__"" title="""" name=""google_ads_iframe_/84947469/pc_forum_top_bottom_rec_r_0__hidden__"" width=""0"" height=""0"" scrolling=""no"" marginwidth=""0"" marginheight=""0"" frameborder=""0"" src=""javascript:&quot;<html><body style='background:transparent'></body></html>&quot;"" style=""border: 0px; vertical-align: bottom; visibility: hidden; display: none;""></iframe></div>
</div>
  </div>
  </div>

    </div>

   <div class=""container-right"">

      <div class=""mt8 mb24"">
        <a href=""http://myanimelist.net/forum/?action=recent"" class=""btn-rect-grey1 pl4 mr8""><i class=""fa fa-clock-o mr4 fs11""></i>Recent</a><a href=""http://myanimelist.net/forum/?action=viewstarred"" class=""btn-rect-grey1 pl4 mr8""><i class=""fa fa-eye mr4 fs11""></i>Watched</a><a href=""http://myanimelist.net/forum/?action=ignored"" class=""btn-rect-grey1 pl4""><i class=""fa fa-ban mr4 fs11""></i>Ignored</a>      </div>

      <div class=""mb20"">
        <form method=""get"" action=""http://myanimelist.net/forum/?action=search"" class=""forum-search-side"">
          <input type=""hidden"" name=""action"" value=""search"">
          <input type=""hidden"" name=""u"" value="""">
          <input type=""hidden"" name=""uloc"" value=""1"">
          <div class=""forum-search-select-outer di-ib fl-l"">
            <select name=""loc"" class=""forum-search-select"">
              <option value=""-1"">All Forums</option>
                              <optgroup label=""MyAnimeList"">
                                      <option value=""5"">Updates &amp; Announcements</option>
                                      <option value=""14"">MAL Guidelines &amp; FAQ</option>
                                      <option value=""17"">DB Modification Requests</option>
                                      <option value=""3"">Support</option>
                                      <option value=""4"">Suggestions</option>
                                      <option value=""13"">MAL Contests</option>
                                  </optgroup>
                              <optgroup label=""Anime &amp; Manga"">
                                      <option value=""15"">News Discussion</option>
                                      <option value=""16"">Anime &amp; Manga Recommendations</option>
                                      <option value=""19"">Series Discussion</option>
                                      <option value=""1"">Anime Discussion</option>
                                      <option value=""2"">Manga Discussion</option>
                                  </optgroup>
                              <optgroup label=""General"">
                                      <option value=""8"">Introductions</option>
                                      <option value=""7"">Games, Computers &amp; Tech Support</option>
                                      <option value=""10"">Music &amp; Entertainment</option>
                                      <option value=""6"">Current Events</option>
                                      <option value=""11"">Casual Discussion</option>
                                      <option value=""12"">Creative Corner</option>
                                      <option value=""9"">Forum Games</option>
                                  </optgroup>
                          </select>
          </div>
          <input type=""text"" name=""q"" class=""forum-search-input-text min2chars fl-l"" size=""20"" value="""" placeholder=""Search topics..."">
          <input type=""submit"" class=""forum-search-input-submit notActive"" value="""">
        </form>

        <div class=""di-b ar pt4"">
          <a href=""http://myanimelist.net/forum/?action=search"" class=""fs10 fn-blue1 ff-avenir"">Advanced Search</a>
        </div>
      </div>

      

<div class=""_unit "" style=""width:300px;display: block !important;"" data-height=""250"">
  <div id=""pc_forum_top_middle_rec_l"" class="""" style=""width: 300px;"">
    <script type=""text/javascript"">
      googletag.cmd.push(function() {
             var slot = googletag.defineSlot(""/84947469/pc_forum_top_middle_rec_l"", [[300,250],[1,1]], ""pc_forum_top_middle_rec_l"").addService(googletag.pubads())
    .setTargeting(""adult"", ""gray"")
    .setCollapseEmptyDiv(true,true);googletag.enableServices();

  googletag.display(""pc_forum_top_middle_rec_l"");
         });</script><div id=""google_ads_iframe_/84947469/pc_forum_top_middle_rec_l_0__container__"" style=""border: 0pt none;""><iframe id=""google_ads_iframe_/84947469/pc_forum_top_middle_rec_l_0"" title=""3rd party ad content"" name=""google_ads_iframe_/84947469/pc_forum_top_middle_rec_l_0"" width=""300"" height=""250"" scrolling=""no"" marginwidth=""0"" marginheight=""0"" frameborder=""0"" src=""javascript:&quot;<html><body style='background:transparent'></body></html>&quot;"" style=""border: 0px; vertical-align: bottom;""></iframe></div>
  <iframe id=""google_ads_iframe_/84947469/pc_forum_top_middle_rec_l_0__hidden__"" title="""" name=""google_ads_iframe_/84947469/pc_forum_top_middle_rec_l_0__hidden__"" width=""0"" height=""0"" scrolling=""no"" marginwidth=""0"" marginheight=""0"" frameborder=""0"" src=""javascript:&quot;<html><body style='background:transparent'></body></html>&quot;"" style=""border: 0px; vertical-align: bottom; visibility: hidden; display: none;""></iframe></div>
</div>

          
<div class=""forum-side-block"">
  <p class=""header"">Popular New Topics
  </p>
  <ul class=""forum-side-list""><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1535527"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/useravatars/1159011.jpg?s=cb1769895b2a0151f5721a1b3623a759"" alt=""Bacardi-x-Cola"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1535527"" class=""title"" title=""Hillary vs Trump"">Hillary vs Trump</a><span class=""information di-ib fs10 fn-grey4"">
                          Today, 8:42 AM by <a href=""http://myanimelist.net/profile/Bacardi-x-Cola"" class=""mr4"">Bacardi-x-Cola</a><a href=""http://myanimelist.net/forum/?topicid=1535527&amp;goto=lastpost"">»»</a><br>
              in Forum Games<span class=""comment fw-b"">662 Replies</span></span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1534987"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/useravatars/5053269.jpg?s=2924bf765cde9eef9bf0dcdbf2749c2e"" alt=""Robiiii"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1534987"" class=""title"" title=""Would you recommend a series you rated 6?"">Would you recommend a series you rated 6?</a><span class=""information di-ib fs10 fn-grey4"">
                          Jul 19, 4:10 PM by <a href=""http://myanimelist.net/profile/Robiiii"" class=""mr4"">Robiiii</a><a href=""http://myanimelist.net/forum/?topicid=1534987&amp;goto=lastpost"">»»</a><br>
              in Anime Discussion<span class=""comment fw-b"">193 Replies</span></span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1535164"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/useravatars/5265111.gif?s=a18c12bab030cd5d5a7fa1593e0c60b4"" alt=""Zeus-"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1535164"" class=""title"" title=""what was your biggest anime disappointment?"">what was your biggest anime disappointment?</a><span class=""information di-ib fs10 fn-grey4"">
                          Yesterday, 5:07 AM by <a href=""http://myanimelist.net/profile/Zeus-"" class=""mr4"">Zeus-</a><a href=""http://myanimelist.net/forum/?topicid=1535164&amp;goto=lastpost"">»»</a><br>
              in Anime Discussion<span class=""comment fw-b"">180 Replies</span></span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1535181"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/useravatars/3144051.jpg?s=b656f30a7f5b82f68ad0c01946c4a696"" alt=""ichii_1"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1535181"" class=""title"" title=""Steve King &quot;whites aided civilization more than other 'sub-groups'"">Steve King ""whites aided civilization more than other 'sub-groups'</a><span class=""information di-ib fs10 fn-grey4"">
                          Yesterday, 7:26 AM by <a href=""http://myanimelist.net/profile/ichii_1"" class=""mr4"">ichii_1</a><a href=""http://myanimelist.net/forum/?topicid=1535181&amp;goto=lastpost"">»»</a><br>
              in Current Events<span class=""comment fw-b"">99 Replies</span></span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1535311"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/useravatars/5049387.jpg?s=59eb3e18f389e67101ff68914122d603"" alt=""YayaChibi"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1535311"" class=""title"" title=""Best anime opening?"">Best anime opening?</a><span class=""information di-ib fs10 fn-grey4"">
                          Yesterday, 6:56 PM by <a href=""http://myanimelist.net/profile/YayaChibi"" class=""mr4"">YayaChibi</a><a href=""http://myanimelist.net/forum/?topicid=1535311&amp;goto=lastpost"">»»</a><br>
              in Anime Discussion<span class=""comment fw-b"">87 Replies</span></span></div></li></ul></div>

          
<div class=""forum-side-block"">
  <p class=""header""><a href=""http://myanimelist.net/forum/?action=recent"" class=""fl-r fw-n fs12"">More</a>Recent Posts
  </p>
  <ul class=""forum-side-list""><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1185313"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/useravatars/4859109.png?s=d6e4c37b2bec8ef093f1f224a59691eb"" alt=""Duskie"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1185313"" class=""title"" title=""Guess if the person above you is gay or straight"">Guess if the person above you is gay or straight</a><span class=""information di-ib fs10 fn-grey4"">
                          Now by <a href=""http://myanimelist.net/profile/Duskie"" class=""mr4"">Duskie</a><a href=""http://myanimelist.net/forum/?topicid=1185313&amp;goto=lastpost"">»»</a><br>
              in Forum Games
                      </span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1528603"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/useravatars/4741523.png?s=51cc4f9c22430aba3d3f87190cfdee3c"" alt=""Gem"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1528603"" class=""title"" title=""Ask the person below you a question. v12"">Ask the person below you a question. v12</a><span class=""information di-ib fs10 fn-grey4"">
                          1 second ago by <a href=""http://myanimelist.net/profile/Gem"" class=""mr4"">Gem</a><a href=""http://myanimelist.net/forum/?topicid=1528603&amp;goto=lastpost"">»»</a><br>
              in Forum Games
                      </span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1535712"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/useravatars/5196955.jpg?s=de1637efdc2bbe751c90085f904f87a1"" alt=""Dishonest"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1535712"" class=""title"" title=""Do you have any guilty pleasure anime?"">Do you have any guilty pleasure anime?</a><span class=""information di-ib fs10 fn-grey4"">
                          4 seconds ago by <a href=""http://myanimelist.net/profile/Dishonest"" class=""mr4"">Dishonest</a><a href=""http://myanimelist.net/forum/?topicid=1535712&amp;goto=lastpost"">»»</a><br>
              in Anime Discussion
                      </span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1535527"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/useravatars/3800929.jpg?s=cd682d42721e95b5482ce3c316a85261"" alt=""Temmie"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1535527"" class=""title"" title=""Hillary vs Trump"">Hillary vs Trump</a><span class=""information di-ib fs10 fn-grey4"">
                          5 seconds ago by <a href=""http://myanimelist.net/profile/Temmie"" class=""mr4"">Temmie</a><a href=""http://myanimelist.net/forum/?topicid=1535527&amp;goto=lastpost"">»»</a><br>
              in Forum Games
                      </span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1425796"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/useravatars/4501971.jpg?s=56eaa83b442cf3c9c9240d774ddb1495"" alt=""TShady"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1425796"" class=""title"" title=""What the above user is to you?"">What the above user is to you?</a><span class=""information di-ib fs10 fn-grey4"">
                          11 seconds ago by <a href=""http://myanimelist.net/profile/TShady"" class=""mr4"">TShady</a><a href=""http://myanimelist.net/forum/?topicid=1425796&amp;goto=lastpost"">»»</a><br>
              in Forum Games
                      </span></div></li></ul></div>

      

<div class=""_unit "" style=""width:300px;display: block !important;"" data-height=""250"">
  <div id=""pc_forum_top_middle_rec_r"" class="""" style=""width: 300px;""><div id=""google_ads_iframe_/84947469/pc_forum_top_middle_rec_r_0__container__"" style=""border: 0pt none;""><iframe id=""google_ads_iframe_/84947469/pc_forum_top_middle_rec_r_0"" title=""3rd party ad content"" name=""google_ads_iframe_/84947469/pc_forum_top_middle_rec_r_0"" width=""300"" height=""250"" scrolling=""no"" marginwidth=""0"" marginheight=""0"" frameborder=""0"" src=""javascript:&quot;<html><body style='background:transparent'></body></html>&quot;"" style=""border: 0px; vertical-align: bottom;""></iframe></div><iframe id=""google_ads_iframe_/84947469/pc_forum_top_middle_rec_r_0__hidden__"" title="""" name=""google_ads_iframe_/84947469/pc_forum_top_middle_rec_r_0__hidden__"" width=""0"" height=""0"" scrolling=""no"" marginwidth=""0"" marginheight=""0"" frameborder=""0"" src=""javascript:&quot;<html><body style='background:transparent'></body></html>&quot;"" style=""border: 0px; vertical-align: bottom; visibility: hidden; display: none;""></iframe></div>
</div>

          
<div class=""forum-side-block"">
  <p class=""header""><a href=""http://myanimelist.net/forum/?subboard=1&amp;type=series"" class=""fl-r fw-n fs12"">More</a>Anime Series Discussion
  </p>
  <ul class=""forum-side-list""><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1534934"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/anime/10/80931.jpg?s=5542d4e97700eeb220d4593052be5743"" alt=""Danganronpa 3: The End of Kibougamine Gakuen - Mirai-hen"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1534934"" class=""title"" title=""Danganronpa 3: The End of Kibougamine Gakuen - Mirai-hen, Why isn't Future Arc considered as a sequel to the Despair Arc?"">Danganronpa 3: The End of Kibougamine Gakuen - Mirai-hen <i class=""fa fa-caret-right""></i> Why isn't Future Arc considered as a sequel to the Despair Arc?</a><span class=""information di-ib fs10 fn-grey4"">
                          12 minutes ago by <a href=""http://myanimelist.net/profile/AzureAceOfficial"" class=""mr4"">AzureAceOfficial</a><a href=""http://myanimelist.net/forum/?topicid=1534934&amp;goto=lastpost"">»»</a><br></span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1534726"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/anime/11/79410.jpg?s=e94b3fd804f028316db234a500a5d851"" alt=""Re:Zero kara Hajimeru Isekai Seikatsu"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1534726"" class=""title"" title=""Re:Zero kara Hajimeru Isekai Seikatsu, Confused about Re:Zero plot? [Spoilers]"">Re:Zero kara Hajimeru Isekai Seikatsu <i class=""fa fa-caret-right""></i> Confused about Re:Zero plot? [Spoilers]</a><span class=""information di-ib fs10 fn-grey4"">
                          16 minutes ago by <a href=""http://myanimelist.net/profile/HucklePeel"" class=""mr4"">HucklePeel</a><a href=""http://myanimelist.net/forum/?topicid=1534726&amp;goto=lastpost"">»»</a><br></span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1207693"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/anime/10/6883.jpg?s=55650527420bfe4d7e183bee7dd2a906"" alt=""Elfen Lied"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1207693"" class=""title"" title=""Elfen Lied, Why is elfen lied so highly rated and popular?"">Elfen Lied <i class=""fa fa-caret-right""></i> Why is elfen lied so highly rated and popular?</a><span class=""information di-ib fs10 fn-grey4"">
                          17 minutes ago by <a href=""http://myanimelist.net/profile/HiImAnAlien"" class=""mr4"">HiImAnAlien</a><a href=""http://myanimelist.net/forum/?topicid=1207693&amp;goto=lastpost"">»»</a><br></span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1534391"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/anime/6/80109.jpg?s=ba65d076a999b717d876aaab03b9f9f2"" alt=""One Piece: Heart of Gold"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1534391"" class=""title"" title=""One Piece: Heart of Gold, When will it be subbed?"">One Piece: Heart of Gold <i class=""fa fa-caret-right""></i> When will it be subbed?</a><span class=""information di-ib fs10 fn-grey4"">
                          21 minutes ago by <a href=""http://myanimelist.net/profile/Ulquiorra"" class=""mr4"">Ulquiorra</a><a href=""http://myanimelist.net/forum/?topicid=1534391&amp;goto=lastpost"">»»</a><br></span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1507467"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/anime/11/79410.jpg?s=e94b3fd804f028316db234a500a5d851"" alt=""Re:Zero kara Hajimeru Isekai Seikatsu"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1507467"" class=""title"" title=""Re:Zero kara Hajimeru Isekai Seikatsu, General Discussion Thread, from page 30+ always tag your spoilers"">Re:Zero kara Hajimeru Isekai Seikatsu <i class=""fa fa-caret-right""></i> General Discussion Thread, from page 30+ always tag your spoilers</a><span class=""information di-ib fs10 fn-grey4"">
                          21 minutes ago by <a href=""http://myanimelist.net/profile/Smudy"" class=""mr4"">Smudy</a><a href=""http://myanimelist.net/forum/?topicid=1507467&amp;goto=lastpost"">»»</a><br></span></div></li></ul></div>

          
<div class=""forum-side-block"">
  <p class=""header""><a href=""http://myanimelist.net/forum/?subboard=4&amp;type=series"" class=""fl-r fw-n fs12"">More</a>Manga Series Discussion
  </p>
  <ul class=""forum-side-list""><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1535392"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/manga/1/66501.jpg?s=0c153bcab4a10a98be50b04ca2adcbbd"" alt=""Nisekoi"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1535392"" class=""title"" title=""Nisekoi, The reasoning the author gives for choosing chitoge is so meh "">Nisekoi <i class=""fa fa-caret-right""></i> The reasoning the author gives for choosing chitoge is so meh </a><span class=""information di-ib fs10 fn-grey4"">
                          6 minutes ago by <a href=""http://myanimelist.net/profile/Xarvyn"" class=""mr4"">Xarvyn</a><a href=""http://myanimelist.net/forum/?topicid=1535392&amp;goto=lastpost"">»»</a><br></span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1535701"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/manga/1/66501.jpg?s=0c153bcab4a10a98be50b04ca2adcbbd"" alt=""Nisekoi"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1535701"" class=""title"" title=""Nisekoi, Making the harem smaller would have been better"">Nisekoi <i class=""fa fa-caret-right""></i> Making the harem smaller would have been better</a><span class=""information di-ib fs10 fn-grey4"">
                          16 minutes ago by <a href=""http://myanimelist.net/profile/Chiibi"" class=""mr4"">Chiibi</a><a href=""http://myanimelist.net/forum/?topicid=1535701&amp;goto=lastpost"">»»</a><br></span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1533117"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/manga/3/55539.jpg?s=4ef38683f4102e1b52e53bf46844e2e4"" alt=""One Piece"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1533117"" class=""title"" title=""One Piece, Will one piecee ever break the predictability?"">One Piece <i class=""fa fa-caret-right""></i> Will one piecee ever break the predictability?</a><span class=""information di-ib fs10 fn-grey4"">
                          1 hour ago by <a href=""http://myanimelist.net/profile/ashfrliebert"" class=""mr4"">ashfrliebert</a><a href=""http://myanimelist.net/forum/?topicid=1533117&amp;goto=lastpost"">»»</a><br></span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1533235"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/manga/1/66501.jpg?s=0c153bcab4a10a98be50b04ca2adcbbd"" alt=""Nisekoi"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1533235"" class=""title"" title=""Nisekoi, Any other Kosaki fam giving this manga a 1/10?"">Nisekoi <i class=""fa fa-caret-right""></i> Any other Kosaki fam giving this manga a 1/10?</a><span class=""information di-ib fs10 fn-grey4"">
                          1 hour ago by <a href=""http://myanimelist.net/profile/crevette"" class=""mr4"">crevette</a><a href=""http://myanimelist.net/forum/?topicid=1533235&amp;goto=lastpost"">»»</a><br></span></div></li><li class=""forum-post clearfix""><a href=""http://myanimelist.net/forum/?topicid=1535199"" class=""thumb mr8 fl-l""><img src=""http://cdn.myanimelist.net/r/80x100/images/manga/3/55539.jpg?s=4ef38683f4102e1b52e53bf46844e2e4"" alt=""One Piece"" width=""40"" height=""50"" class=""thumbs""></a><div class=""information-block""><a href=""http://myanimelist.net/forum/?topicid=1535199"" class=""title"" title=""One Piece, One Piece manga 65% finished "">One Piece <i class=""fa fa-caret-right""></i> One Piece manga 65% finished </a><span class=""information di-ib fs10 fn-grey4"">
                          2 hours ago by <a href=""http://myanimelist.net/profile/Jarjaxle"" class=""mr4"">Jarjaxle</a><a href=""http://myanimelist.net/forum/?topicid=1535199&amp;goto=lastpost"">»»</a><br></span></div></li></ul></div>

      <div class=""forum-side-block"">
        <p class=""header"">
          Forum Statistics
        </p>
      </div>
      <div class=""pt4 fn-grey5 fs11 lh18"">
        Topics: 699,654<br>
        Users: 3,556,343<br>
        Users Browsing Forums: 714
      </div>

    </div>
  </div>

</div>
            </div>  <!--  control container height -->
  <div style=""clear:both;""></div>
  <!-- end rightbody -->

                      
                </div>
      
    <div id=""ad-skin-bg-right"" class=""ad-skin-side-outer ad-skin-side-bg bg-right"">
    <div id=""ad-skin-right"" class=""ad-skin-side right"" style=""display: none;"">
      <div id=""ad-skin-right-absolute-block"">
        <div id=""ad-skin-right-fixed-block""></div>
      </div>
    </div>
  </div></div>
    
    <footer>
  <div id=""footer-block"" style=""margin-top: 0px;"">
    <div class=""footer-link-block"">
      <p class=""footer-link home di-ib"">
        <a href=""http://myanimelist.net/"">Home</a>
      </p>
      <p class=""footer-link di-ib"">
        <a href=""http://myanimelist.net/about.php"">About</a>
        <a href=""http://myanimelist.net/pressroom"">Press Room</a>
        <a href=""http://myanimelist.net/about.php?go=contact"">Support</a>
        <a href=""http://myanimelist.net/advertising"">Advertising</a>
        <a href=""http://myanimelist.net/forum/?topicid=515949"">FAQ</a>
        <a href=""http://myanimelist.net/about/terms_of_use"">Terms</a>
        <a href=""http://myanimelist.net/about/privacy_policy"">Privacy</a>
        <a href=""http://myanimelist.net/about/sitemap"">Sitemap</a>
    	</p>
  		    </div>

    <div class=""footer-link-icon-block"">
            <div class=""footer-social-media ac"">
        <a target=""_blank"" class=""icon-sns icon-fb di-ib"" href=""https://www.facebook.com/OfficialMyAnimeList""></a>
        <a target=""_blank"" class=""icon-sns icon-tw di-ib"" title=""Follow @myanimelist on Twitter"" href=""https://twitter.com/myanimelist""></a>
        <a class=""icon-sns icon-gp"" href=""https://plus.google.com/105884801583962160252?prsrc=3"" rel=""publisher"" target=""_blank"" style=""text-decoration:none;"">
          <img src=""http://cdn.myanimelist.net/images/footer/icon-google_plus.png"" alt=""Google+"" style=""border:0;width:30px;height:30px;"">
        </a>
      </div>
            <div class=""footer-recommended ac"">
        <a target=""_blank"" class=""icon-recommended icon-tokyo-otaku-mode"" href=""http://otakumode.com/fb/5aO"">Tokyo Otaku Mode</a>
      </div>
    </div>

    <div id=""copyright"">
      MyAnimeList.net is a property of MyAnimeList, LLC. ©2015 All Rights Reserved.
    </div>
  </div>
</footer>

<div id=""evolve_footer""></div>

        <script type=""text/javascript"">
    window.MAL.magia = ""06410c4e6b2518e9add8f6df0ccb2da2876bb8c980aacb43a8dcaa8153c0f92c"";
  window.MAL.madoka = ""hZrDKm9k6FVRnqd3i%=K"";
</script>
  

<div id=""fancybox-tmp""></div><div id=""fancybox-loading""><div></div></div><div id=""fancybox-overlay""></div><div id=""fancybox-wrap""><div id=""fancybox-outer""><div class=""fancy-bg"" id=""fancy-bg-n""></div><div class=""fancy-bg"" id=""fancy-bg-ne""></div><div class=""fancy-bg"" id=""fancy-bg-e""></div><div class=""fancy-bg"" id=""fancy-bg-se""></div><div class=""fancy-bg"" id=""fancy-bg-s""></div><div class=""fancy-bg"" id=""fancy-bg-sw""></div><div class=""fancy-bg"" id=""fancy-bg-w""></div><div class=""fancy-bg"" id=""fancy-bg-nw""></div><div id=""fancybox-inner""></div><a id=""fancybox-close""></a><a href=""javascript:;"" id=""fancybox-left""><span class=""fancy-ico"" id=""fancybox-left-ico""></span></a><a href=""javascript:;"" id=""fancybox-right""><span class=""fancy-ico"" id=""fancybox-right-ico""></span></a></div></div><iframe id=""google_osd_static_frame_410285243812"" name=""google_osd_static_frame"" style=""display: none; width: 0px; height: 0px;""></iframe></body></html>";
#endregion
            if (string.IsNullOrEmpty(raw))
                return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);

            var list = new List<ForumBoardEntryPeekPost>(2);
            int i = 0;
            foreach (var topics in doc.WhereOfDescendantsWithClass("ul", "topics"))
            {
                foreach (var post in topics.Descendants("li"))
                {
                    i++;
                    if (i == 5 || i == 6) //skip db midifiaction board
                        continue;
                    var current = new ForumBoardEntryPeekPost();
                    current.PostTime = WebUtility.HtmlDecode(post.FirstOfDescendantsWithClass("span", "date di-ib pt4 fs10 fn-grey4").InnerText.TrimEnd('»'));
                    current.Title = WebUtility.HtmlDecode(post.FirstOfDescendantsWithClass("a", "topic-title-link").InnerText);
                    var img = post.Descendants("img").First();
                    current.User.ImgUrl = img.Attributes["src"].Value;
                    current.User.Name = img.Attributes["alt"].Value;
                    list.Add(current);
                    if (list.Count == 2) //assume we have 2 for each board
                    {
                        output.Add(list);
                        list = new List<ForumBoardEntryPeekPost>();
                    }
                }              
            }

            return output;
        }
    }
}
