using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MalClient.Shared.Models.Forums;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.Comm.Forums
{
    public class ForumBoardTopicsQuery : Query
    {
        public ForumBoardTopicsQuery(ForumBoards board)
        {
            Request =
                WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/forum/{GetEndpoint(board)}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        public async Task<List<ForumTopicEntry>> GetTopicPosts()
        {
            var output = new List<ForumTopicEntry>();

            #region testString

            var raw = @"<html><head>
    <link rel=""alternate"" type=""application/rss+xml"" title=""Forum - MyAnimeList.net RSS Feed"" href=""/rss.php?type=forum""><link rel=""alternate"" type=""application/rss+xml"" title=""Recent Posts - MyAnimeList Forum"" href=""/rss.php?type=board&amp;id=4"">

<title>
Suggestions - Forums - MyAnimeList.net
</title>


  
<meta name=""keywords"" content=""anime, myanimelist, anime news, manga"">
  

        
          
          
  <link rel=""next"" href=""http://myanimelist.net/forum/?board=4&amp;show=50"">




<meta property=""og:locale"" content=""en_US""><meta property=""fb:app_id"" content=""360769957454434""><meta property=""og:site_name"" content=""MyAnimeList.net""><meta name=""twitter:card"" content=""summary""><meta name=""twitter:site"" content=""@myanimelist""><meta property=""og:title"" content="" Suggestions - Forums - MyAnimeList.net ""><meta property=""og:image"" content=""http://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png""><meta name=""twitter:image:src"" content=""http://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png""><meta property=""og:url"" content=""http://myanimelist.net/forum/?board=4"">
<meta name=""csrf_token"" content=""af7719f00a074da2480e610b1d1f3e1007beae26"">
<link rel=""stylesheet"" type=""text/css"" href=""http://cdn.myanimelist.net/static/assets/css/pc/style-2455f8dda3.css"">

<script src=""http://pagead2.googlesyndication.com/pagead/osd.js""></script><script type=""text/javascript"" async="""" src=""http://www.google-analytics.com/plugins/ua/linkid.js""></script><script type=""text/javascript"" async="""" src=""https://www.gstatic.com/recaptcha/api2/r20160718175036/recaptcha__en.js""></script><script async="""" type=""text/javascript"" src=""http://www.googletagservices.com/tag/js/gpt.js""></script><script async="""" src=""//www.google-analytics.com/analytics.js""></script><script type=""text/javascript"" src=""http://cdn.myanimelist.net/static/assets/js/pc/header-1147b3497b.js""></script>
<script type=""text/javascript"" src=""http://cdn.myanimelist.net/static/assets/js/pc/all-49f169a5c6.js"" id=""alljs"" data-params=""{&quot;origin_url&quot;:&quot;http:\/\/myanimelist.net&quot;,&quot;is_request_bot_filter_log&quot;:false}"" defer=""""></script>



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

<body onload="" "" class=""page-forum page-common"">
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
  </div>
    <div class=""wrapper"">

        <div id=""headerSmall""><a href=""/panel.php"" class=""link-mal-logo"">MyAnimeList.net</a>

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
</script><div class=""border""></div><div class=""header-menu-unit header-profile js-header-menu-unit link-bg pl8 pr8"" data-id=""profile""><a class=""header-profile-link"">Drutol<i class=""fa fa-caret-down ml4""></i></a><div class=""header-menu-dropdown header-profile-dropdown arrow_box""><ul><li><a href=""http://myanimelist.net/profile/Drutol"">Profile</a></li><li class=""clearfix""><a href=""http://myanimelist.net/myfriends.php"">Friends</a></li><li class=""clearfix""><a href=""http://myanimelist.net/clubs.php?action=myclubs"">Clubs</a></li><li><a href=""http://myanimelist.net/blog/Drutol"">Blog Posts</a></li><li><a href=""http://myanimelist.net/myreviews.php"">Reviews</a></li><li><a href=""http://myanimelist.net/myrecommendations.php"">Recommendations</a></li><li><a href=""http://myanimelist.net/editprofile.php?go=myoptions""><i class=""fa fa-cog mr4""></i>Account Settings</a></li><li><form action=""http://myanimelist.net/logout.php"" method=""post""><a href=""javascript:void(0);"" onclick=""$(this).parent().submit();""><i class=""fa fa-sign-out mr4""></i>Logout</a></form></li></ul></div></div><div class=""header-menu-unit header-profile pl0""><a href=""http://myanimelist.net/profile/Drutol"" class=""header-profile-button"" style=""background-image:url(http://cdn.myanimelist.net/images/userimages/4952914.jpg)"" title=""Drutol""></a></div></div></div>  <div id=""menu"">
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
          <ul class=""wider"">
            <li><a href=""http://myanimelist.net/watch/episode?_location=mal_h_m"">Episode Videos</a></li>
            <li><a href=""http://myanimelist.net/watch/promotion?_location=mal_h_m"">Promotional Videos</a></li>
            <li><a href=""http://myanimelist.net/watch/special?_location=mal_h_m"">Special Videos</a></li>
          </ul>
        </li>
        <li class=""smaller""><a href=""#"" class=""non-link"">Help</a>
          <ul class=""wide"">
            <li><a href=""http://myanimelist.net/about.php?_location=mal_h_m"">About</a></li>
            <li><a href=""http://myanimelist.net/about.php?go=support&amp;_location=mal_h_m"">Support</a></li>
            <li><a href=""http://myanimelist.net/advertising?_location=mal_h_m"">Advertising</a></li>
            <li><a href=""http://myanimelist.net/forum/?topicid=515949&amp;_location=mal_h_m"">FAQ</a></li>
            <li><a href=""http://myanimelist.net/modules.php?go=report&amp;_location=mal_h_m"">Report</a></li>
            <li><a href=""http://myanimelist.net/about.php?go=team&amp;_location=mal_h_m"">Staff</a></li>
          </ul>
        </li>
              </ul>
    </div>  </div><div id=""contentWrapper""><div><a class=""header-right"" href=""/editprofile.php?go=forumoptions""><i class=""fa fa-cog mr4""></i>Forum Settings</a><div class=""h1"">Forums</div></div><div id=""content""><div class=""clearfix mt4""><div class=""fl-l"">


                  <div class=""breadcrumb "" itemscope="""" itemtype=""http://schema.org/BreadcrumbList""><div class=""di-ib"" itemprop=""itemListElement"" itemscope="""" itemtype=""http://schema.org/ListItem""><a href=""http://myanimelist.net/"" itemprop=""item""><span itemprop=""name"">
              Top
            </span></a><meta itemprop=""position"" content=""1""></div>&nbsp; &gt; &nbsp;<div class=""di-ib"" itemprop=""itemListElement"" itemscope="""" itemtype=""http://schema.org/ListItem""><a href=""http://myanimelist.net/forum/"" itemprop=""item""><span itemprop=""name"">
              Forum
            </span></a><meta itemprop=""position"" content=""2""></div>&nbsp; &gt; &nbsp;<div class=""di-ib"" itemprop=""itemListElement"" itemscope="""" itemtype=""http://schema.org/ListItem""><a href=""http://myanimelist.net/forum/?board=4"" itemprop=""item""><span itemprop=""name"">
              Suggestions
            </span></a><meta itemprop=""position"" content=""3""></div></div>



</div>
<a name=""top""></a>

<div class=""fl-r ar""><a href=""http://myanimelist.net/forum/?action=recent"" class=""btn-rect-grey1 pl4 ml8""><i class=""fa fa-clock-o mr4 fs11""></i>Recent</a><a href=""http://myanimelist.net/forum/?action=viewstarred"" class=""btn-rect-grey1 pl4 ml8""><i class=""fa fa-eye mr4 fs11""></i>Watched</a><a href=""http://myanimelist.net/forum/?action=ignored"" class=""btn-rect-grey1 pl4 ml8""><i class=""fa fa-ban mr4 fs11""></i>Ignored</a><a href=""http://myanimelist.net/forum/?action=search"" class=""btn-rect-grey1 pl4 ml8""><i class=""fa fa-search mr4 fs11""></i>Search Forum</a></div><div class=""fl-l mt8 cl-l"">
    <h1 class=""forum_locheader"">Suggestions</h1><br><span>Have an idea or suggestion for the site? Share it here.</span></div></div>
    <div class=""borderClass pl0 pr0 pt16 pb0 cl-b"">
      <div class=""fl-l pb8""><a href=""?action=post&amp;boardid=4"" class=""btn-forum"">Create New Topic</a><span id=""iBoardId"" class=""di-id"" style=""margin-left:6px;""><a href=""javascript:void(0);"" data-id=""4"" data-mode=""1"" class=""js-ignore-board-button btn-forum"">Ignore Board</a></span></div>
      <div class=""fl-r pb8""><span class=""di-ib"">Pages (105) <a href=""?board=4&amp;show=-100""></a> <a href=""?board=4&amp;show=-50""></a> [1] <a href=""?board=4&amp;show=50"">2</a> <a href=""?board=4&amp;show=100"">3</a> <a href=""?board=4&amp;show=50"">»</a> ... <a href=""?board=4&amp;show=5200"">Last »</a></span></div>
      <div class=""forum-topic-sort""><p class=""pl8"">Sorted by: <a href=""?board=4&amp;sort=post"">Newest Topic</a>&nbsp;·&nbsp;<span class=""fw-b"">Last Post</span></p></div>
    </div>
	<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" id=""forumTopics""><tbody><tr class=""forum-table-header""><td align=""center"" class=""normal_header"" colspan=""2"">Topic</td>
		  	<td width=""75"" align=""center"" class=""normal_header"">Replies</td><td width=""130"" align=""center"" class=""normal_header"">Last Post</td>
		</tr><tr id=""topicRow1"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow1""><a href=""javascript:void(0);"" data-id=""4555"" class=""js-toggle-watching-topic-button""><span id=""wt4555""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""4555"" data-row=""topicRow1"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span><strong>Sticky:</strong>  <a href=""/forum/?topicid=4555"" class=""icon-forum-locked"">When Posting a Site Suggestion...</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Xinil"">Xinil</a></span> - <span class=""lightLink"">Sep 5, 2007</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">2</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Chavez"">Chavez</a> <a href=""/forum/?topicid=4555&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Apr 24, 2011 10:04 PM</td>
			</tr>
			<tr id=""topicRow2"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow2""><a href=""javascript:void(0);"" data-id=""1527162"" class=""js-toggle-watching-topic-button""><span id=""wt1527162""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1527162"" data-row=""topicRow2"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span><strong><a href=""/forum/?topicid=1527162&amp;goto=newpost"" title=""Go to Newest Post"">»</a> <a href=""/forum/?topicid=1527162"">Manga &amp; Anime language</a></strong> <small></small><br><span class=""forum_postusername""><a href=""/profile/Timarda"">Timarda</a></span> - <span class=""lightLink"">Jun 28</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">3</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Timarda"">Timarda</a> <a href=""/forum/?topicid=1527162&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>2 hours ago</td>
			</tr>
			<tr id=""topicRow3"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow3""><a href=""javascript:void(0);"" data-id=""1440396"" class=""js-toggle-watching-topic-button""><span id=""wt1440396""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1440396"" data-row=""topicRow3"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span><strong><a href=""/forum/?topicid=1440396&amp;goto=newpost"" title=""Go to Newest Post"">»</a> <a href=""/forum/?topicid=1440396"">Add source material filter to the anime search page</a></strong> <small></small><br><span class=""forum_postusername""><a href=""/profile/Cnon"">Cnon</a></span> - <span class=""lightLink"">Oct 19, 2015</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">22</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/KingRequiem"">KingRequiem</a> <a href=""/forum/?topicid=1440396&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>8 hours ago</td>
			</tr>
			<tr id=""topicRow4"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow4""><a href=""javascript:void(0);"" data-id=""255271"" class=""js-toggle-watching-topic-button""><span id=""wt255271""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""255271"" data-row=""topicRow4"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span><strong><a href=""/forum/?topicid=255271&amp;goto=newpost"" title=""Go to Newest Post"">»</a> <a href=""/forum/?topicid=255271"">Security: Add SSL/TSL/HTTPS Support</a></strong> <small> ( <a href=""/forum/?topicid=255271&amp;show=0"">1</a> <a href=""/forum/?topicid=255271&amp;show=50"">2</a> <a href=""/forum/?topicid=255271&amp;show=100"">3</a>  ) </small><br><span class=""forum_postusername""><a href=""/profile/packet"">packet</a></span> - <span class=""lightLink"">Aug 18, 2010</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">115</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/epatryk"">epatryk</a> <a href=""/forum/?topicid=255271&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Yesterday, 11:15 AM</td>
			</tr>
			<tr id=""topicRow5"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow5""><a href=""javascript:void(0);"" data-id=""1535841"" class=""js-toggle-watching-topic-button""><span id=""wt1535841""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1535841"" data-row=""topicRow5"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span><strong><a href=""/forum/?topicid=1535841&amp;goto=newpost"" title=""Go to Newest Post"">»</a> <a href=""/forum/?topicid=1535841"">Category headers on the All Anime page (modern list)</a></strong> <small></small><br><span class=""forum_postusername""><a href=""/profile/Shishio-kun"">Shishio-kun</a></span> - <span class=""lightLink"">Jul 22</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">0</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Shishio-kun"">Shishio-kun</a> <a href=""/forum/?topicid=1535841&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Yesterday, 8:55 AM</td>
			</tr>
			<tr id=""topicRow6"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow6""><a href=""javascript:void(0);"" data-id=""1535826"" class=""js-toggle-watching-topic-button""><span id=""wt1535826""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1535826"" data-row=""topicRow6"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span><strong><a href=""/forum/?topicid=1535826&amp;goto=newpost"" title=""Go to Newest Post"">»</a> <a href=""/forum/?topicid=1535826"">Regarding the helpful button on reviews.</a></strong> <small></small><br><span class=""forum_postusername""><a href=""/profile/Touniouk"">Touniouk</a></span> - <span class=""lightLink"">Jul 22</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">1</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/ChrissyAtSea"">ChrissyAtSea</a> <a href=""/forum/?topicid=1535826&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Yesterday, 7:26 AM</td>
			</tr>
			<tr id=""topicRow7"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow7""><a href=""javascript:void(0);"" data-id=""1535827"" class=""js-toggle-watching-topic-button""><span id=""wt1535827""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1535827"" data-row=""topicRow7"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span><strong><a href=""/forum/?topicid=1535827&amp;goto=newpost"" title=""Go to Newest Post"">»</a> <a href=""/forum/?topicid=1535827"">Was there a use for status=5 for profiles?</a></strong> <small></small><br><span class=""forum_postusername""><a href=""/profile/senpaizuri3"">senpaizuri3</a></span> - <span class=""lightLink"">Jul 22</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">0</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/senpaizuri3"">senpaizuri3</a> <a href=""/forum/?topicid=1535827&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Yesterday, 7:19 AM</td>
			</tr>
			<tr id=""topicRow8"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow8""><a href=""javascript:void(0);"" data-id=""1512614"" class=""js-toggle-watching-topic-button""><span id=""wt1512614""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1512614"" data-row=""topicRow8"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span>Poll:  <a href=""/forum/?topicid=1512614"">A Suggestion for the Anime and Manga Lists</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/KingUmarBashir"">KingUmarBashir</a></span> - <span class=""lightLink"">May 20</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">14</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Shishio-kun"">Shishio-kun</a> <a href=""/forum/?topicid=1512614&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Yesterday, 4:26 AM</td>
			</tr>
			<tr id=""topicRow9"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow9""><a href=""javascript:void(0);"" data-id=""1529426"" class=""js-toggle-watching-topic-button""><span id=""wt1529426""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1529426"" data-row=""topicRow9"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span>Poll:  <a href=""/forum/?topicid=1529426"">Option to sort manga and anime lists by ""date added""</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Souphead"">Souphead</a></span> - <span class=""lightLink"">Jul 3</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">8</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Fexell"">Fexell</a> <a href=""/forum/?topicid=1529426&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 21, 7:20 PM</td>
			</tr>
			<tr id=""topicRow10"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow10""><a href=""javascript:void(0);"" data-id=""1535653"" class=""js-toggle-watching-topic-button""><span id=""wt1535653""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1535653"" data-row=""topicRow10"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1535653"">Content advisory</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/DecimoXX"">DecimoXX</a></span> - <span class=""lightLink"">Jul 21</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">0</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/DecimoXX"">DecimoXX</a> <a href=""/forum/?topicid=1535653&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 21, 5:51 PM</td>
			</tr>
			<tr id=""topicRow11"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow11""><a href=""javascript:void(0);"" data-id=""1535614"" class=""js-toggle-watching-topic-button""><span id=""wt1535614""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1535614"" data-row=""topicRow11"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1535614"">Searching for Similiar Anime that Multiple Seiyuus are in</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/brownlegion"">brownlegion</a></span> - <span class=""lightLink"">Jul 21</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">0</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/brownlegion"">brownlegion</a> <a href=""/forum/?topicid=1535614&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 21, 3:09 PM</td>
			</tr>
			<tr id=""topicRow12"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow12""><a href=""javascript:void(0);"" data-id=""1535515"" class=""js-toggle-watching-topic-button""><span id=""wt1535515""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1535515"" data-row=""topicRow12"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1535515"">Mobile site: Go to newest post button</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Fasolt"">Fasolt</a></span> - <span class=""lightLink"">Jul 21</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">0</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Fasolt"">Fasolt</a> <a href=""/forum/?topicid=1535515&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 21, 7:52 AM</td>
			</tr>
			<tr id=""topicRow13"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow13""><a href=""javascript:void(0);"" data-id=""1535514"" class=""js-toggle-watching-topic-button""><span id=""wt1535514""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1535514"" data-row=""topicRow13"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1535514"">add histogram for review scores?</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/reherd"">reherd</a></span> - <span class=""lightLink"">Jul 21</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">0</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/reherd"">reherd</a> <a href=""/forum/?topicid=1535514&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 21, 7:49 AM</td>
			</tr>
			<tr id=""topicRow14"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow14""><a href=""javascript:void(0);"" data-id=""1526063"" class=""js-toggle-watching-topic-button""><span id=""wt1526063""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1526063"" data-row=""topicRow14"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1526063"">Make quoting on mobile linked to the post. </a> <small></small><br><span class=""forum_postusername""><a href=""/profile/ExTemplar"">ExTemplar</a></span> - <span class=""lightLink"">Jun 24</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">2</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Fasolt"">Fasolt</a> <a href=""/forum/?topicid=1526063&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 21, 7:41 AM</td>
			</tr>
			<tr id=""topicRow15"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow15""><a href=""javascript:void(0);"" data-id=""1535475"" class=""js-toggle-watching-topic-button""><span id=""wt1535475""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1535475"" data-row=""topicRow15"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1535475"">Source Field For Editing Anime Information</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/BigOnAnime"">BigOnAnime</a></span> - <span class=""lightLink"">Jul 21</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">0</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/BigOnAnime"">BigOnAnime</a> <a href=""/forum/?topicid=1535475&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 21, 5:16 AM</td>
			</tr>
			<tr id=""topicRow16"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow16""><a href=""javascript:void(0);"" data-id=""1535247"" class=""js-toggle-watching-topic-button""><span id=""wt1535247""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1535247"" data-row=""topicRow16"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1535247"">Ability to customize the Seasonal Anime page</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/vigorousjammer"">vigorousjammer</a></span> - <span class=""lightLink"">Jul 20</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">2</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/vigorousjammer"">vigorousjammer</a> <a href=""/forum/?topicid=1535247&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 21, 5:03 AM</td>
			</tr>
			<tr id=""topicRow17"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow17""><a href=""javascript:void(0);"" data-id=""1526600"" class=""js-toggle-watching-topic-button""><span id=""wt1526600""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1526600"" data-row=""topicRow17"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1526600"">Remove ""Forum Games""</a> <small> ( <a href=""/forum/?topicid=1526600&amp;show=0"">1</a> <a href=""/forum/?topicid=1526600&amp;show=50"">2</a>  ) </small><br><span class=""forum_postusername""><a href=""/profile/Luthandorius"">Luthandorius</a></span> - <span class=""lightLink"">Jun 26</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">53</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Giotto"">Giotto</a> <a href=""/forum/?topicid=1526600&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 21, 3:58 AM</td>
			</tr>
			<tr id=""topicRow18"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow18""><a href=""javascript:void(0);"" data-id=""1529105"" class=""js-toggle-watching-topic-button""><span id=""wt1529105""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1529105"" data-row=""topicRow18"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1529105"">For all the equations the system uses, it seems weak...</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Seyfert"">Seyfert</a></span> - <span class=""lightLink"">Jul 2</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">3</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Giotto"">Giotto</a> <a href=""/forum/?topicid=1529105&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 21, 3:55 AM</td>
			</tr>
			<tr id=""topicRow19"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow19""><a href=""javascript:void(0);"" data-id=""1427870"" class=""js-toggle-watching-topic-button""><span id=""wt1427870""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1427870"" data-row=""topicRow19"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1427870"">Animated (Gif) Icons for Profile?</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Saiko"">Saiko</a></span> - <span class=""lightLink"">Sep 14, 2015</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">6</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Giotto"">Giotto</a> <a href=""/forum/?topicid=1427870&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 21, 2:29 AM</td>
			</tr>
			<tr id=""topicRow20"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow20""><a href=""javascript:void(0);"" data-id=""1533270"" class=""js-toggle-watching-topic-button""><span id=""wt1533270""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1533270"" data-row=""topicRow20"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1533270"">Update the ""Preview"" feature for forum threads.</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Doomcat55"">Doomcat55</a></span> - <span class=""lightLink"">Jul 14</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">2</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Wintovisky"">Wintovisky</a> <a href=""/forum/?topicid=1533270&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 20, 10:40 PM</td>
			</tr>
			<tr id=""topicRow21"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow21""><a href=""javascript:void(0);"" data-id=""1535119"" class=""js-toggle-watching-topic-button""><span id=""wt1535119""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1535119"" data-row=""topicRow21"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span>Poll:  <a href=""/forum/?topicid=1535119"">Ecchi Rating/A Secondary Rating System?</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Kyou85Kirino96"">Kyou85Kirino96</a></span> - <span class=""lightLink"">Jul 20</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">10</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Kyou85Kirino96"">Kyou85Kirino96</a> <a href=""/forum/?topicid=1535119&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 20, 7:36 PM</td>
			</tr>
			<tr id=""topicRow22"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow22""><a href=""javascript:void(0);"" data-id=""1421879"" class=""js-toggle-watching-topic-button""><span id=""wt1421879""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1421879"" data-row=""topicRow22"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1421879"">Review: Take back/Un-click/Undo accidental Helpful vote [In Discussion]</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Zagafon"">Zagafon</a></span> - <span class=""lightLink"">Aug 27, 2015</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">3</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/konfou"">konfou</a> <a href=""/forum/?topicid=1421879&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 20, 7:29 PM</td>
			</tr>
			<tr id=""topicRow23"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow23""><a href=""javascript:void(0);"" data-id=""1535299"" class=""js-toggle-watching-topic-button""><span id=""wt1535299""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1535299"" data-row=""topicRow23"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1535299"">Create a section where you can sort seasons by title</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Raccoon_Citizen"">Raccoon_Citizen</a></span> - <span class=""lightLink"">Jul 20</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">0</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Raccoon_Citizen"">Raccoon_Citizen</a> <a href=""/forum/?topicid=1535299&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 20, 5:37 PM</td>
			</tr>
			<tr id=""topicRow24"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow24""><a href=""javascript:void(0);"" data-id=""1535291"" class=""js-toggle-watching-topic-button""><span id=""wt1535291""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1535291"" data-row=""topicRow24"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1535291"">Sort creator/staff's works by year it released</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/nokitron"">nokitron</a></span> - <span class=""lightLink"">Jul 20</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">0</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/nokitron"">nokitron</a> <a href=""/forum/?topicid=1535291&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 20, 5:12 PM</td>
			</tr>
			<tr id=""topicRow25"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow25""><a href=""javascript:void(0);"" data-id=""1517848"" class=""js-toggle-watching-topic-button""><span id=""wt1517848""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1517848"" data-row=""topicRow25"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1517848"">Need a ""+"" button for episode count on mobile site</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/precurejunkie"">precurejunkie</a></span> - <span class=""lightLink"">Jun 3</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">5</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/precurejunkie"">precurejunkie</a> <a href=""/forum/?topicid=1517848&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 20, 4:41 PM</td>
			</tr>
			<tr id=""topicRow26"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow26""><a href=""javascript:void(0);"" data-id=""1464777"" class=""js-toggle-watching-topic-button""><span id=""wt1464777""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1464777"" data-row=""topicRow26"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1464777"">In Seasonal Anime, create tab that has the year in review</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Maka"">Maka</a></span> - <span class=""lightLink"">Dec 23, 2015</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">5</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Iizbakaokay"">Iizbakaokay</a> <a href=""/forum/?topicid=1464777&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 20, 3:05 PM</td>
			</tr>
			<tr id=""topicRow27"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow27""><a href=""javascript:void(0);"" data-id=""1535102"" class=""js-toggle-watching-topic-button""><span id=""wt1535102""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1535102"" data-row=""topicRow27"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1535102"">Any way to hide Episode Videos tab ??</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/moodie"">moodie</a></span> - <span class=""lightLink"">Jul 20</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">1</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Iizbakaokay"">Iizbakaokay</a> <a href=""/forum/?topicid=1535102&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 20, 7:56 AM</td>
			</tr>
			<tr id=""topicRow28"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow28""><a href=""javascript:void(0);"" data-id=""1535163"" class=""js-toggle-watching-topic-button""><span id=""wt1535163""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1535163"" data-row=""topicRow28"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span>Poll:  <a href=""/forum/?topicid=1535163"">Character Traits or Tags?</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Flyboy66"">Flyboy66</a></span> - <span class=""lightLink"">Jul 20</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">1</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Iizbakaokay"">Iizbakaokay</a> <a href=""/forum/?topicid=1535163&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 20, 7:55 AM</td>
			</tr>
			<tr id=""topicRow29"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow29""><a href=""javascript:void(0);"" data-id=""1498500"" class=""js-toggle-watching-topic-button""><span id=""wt1498500""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1498500"" data-row=""topicRow29"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1498500"">DB: Remove Potential Spoilers from Video Thumbnails</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/ProgrammerJimmy"">ProgrammerJimmy</a></span> - <span class=""lightLink"">Apr 6</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">30</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Iizbakaokay"">Iizbakaokay</a> <a href=""/forum/?topicid=1498500&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 20, 7:49 AM</td>
			</tr>
			<tr id=""topicRow30"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow30""><a href=""javascript:void(0);"" data-id=""1527683"" class=""js-toggle-watching-topic-button""><span id=""wt1527683""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1527683"" data-row=""topicRow30"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1527683"">Why do we don't have response section in reviews, recommendation, articles etc..  </a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Reydulcetgambol"">Reydulcetgambol</a></span> - <span class=""lightLink"">Jun 29</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">20</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Reydulcetgambol"">Reydulcetgambol</a> <a href=""/forum/?topicid=1527683&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 20, 5:14 AM</td>
			</tr>
			<tr id=""topicRow31"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow31""><a href=""javascript:void(0);"" data-id=""1534929"" class=""js-toggle-watching-topic-button""><span id=""wt1534929""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1534929"" data-row=""topicRow31"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1534929"">Can't reviews have an advanced search/filtering?</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Seyfert"">Seyfert</a></span> - <span class=""lightLink"">Jul 19</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">1</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Subpyro"">Subpyro</a> <a href=""/forum/?topicid=1534929&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 19, 3:11 PM</td>
			</tr>
			<tr id=""topicRow32"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow32""><a href=""javascript:void(0);"" data-id=""1534686"" class=""js-toggle-watching-topic-button""><span id=""wt1534686""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1534686"" data-row=""topicRow32"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1534686"">Make characters searchable with popular yet inaccurate spellings</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/linaxt"">linaxt</a></span> - <span class=""lightLink"">Jul 18</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">2</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Luthandorius"">Luthandorius</a> <a href=""/forum/?topicid=1534686&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 19, 11:27 AM</td>
			</tr>
			<tr id=""topicRow33"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow33""><a href=""javascript:void(0);"" data-id=""1529390"" class=""js-toggle-watching-topic-button""><span id=""wt1529390""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1529390"" data-row=""topicRow33"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1529390"">Can we please please please somehow exclude all of the airing titles on the top anime page and only allow them to be displayed after finished in a period of time?</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Takana_no_Hana"">Takana_no_Hana</a></span> - <span class=""lightLink"">Jul 3</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">12</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Aneji"">Aneji</a> <a href=""/forum/?topicid=1529390&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 19, 12:22 AM</td>
			</tr>
			<tr id=""topicRow34"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow34""><a href=""javascript:void(0);"" data-id=""16816"" class=""js-toggle-watching-topic-button""><span id=""wt16816""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""16816"" data-row=""topicRow34"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=16816"">Anime Rating (G-PG-PG13-R) Clarification?</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/zvuc"">zvuc</a></span> - <span class=""lightLink"">Mar 18, 2008</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">34</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Ailithir"">Ailithir</a> <a href=""/forum/?topicid=16816&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 18, 4:25 PM</td>
			</tr>
			<tr id=""topicRow35"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow35""><a href=""javascript:void(0);"" data-id=""1529602"" class=""js-toggle-watching-topic-button""><span id=""wt1529602""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1529602"" data-row=""topicRow35"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1529602"">Rule against bigoted generalizations</a> <small> ( <a href=""/forum/?topicid=1529602&amp;show=0"">1</a> <a href=""/forum/?topicid=1529602&amp;show=50"">2</a> <a href=""/forum/?topicid=1529602&amp;show=100"">3</a>  ) </small><br><span class=""forum_postusername""><a href=""/profile/aikaflip"">aikaflip</a></span> - <span class=""lightLink"">Jul 3</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">119</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Clebardman"">Clebardman</a> <a href=""/forum/?topicid=1529602&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 18, 1:52 PM</td>
			</tr>
			<tr id=""topicRow36"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow36""><a href=""javascript:void(0);"" data-id=""1534597"" class=""js-toggle-watching-topic-button""><span id=""wt1534597""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1534597"" data-row=""topicRow36"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1534597"">Entertainment rating system</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/smilyface58"">smilyface58</a></span> - <span class=""lightLink"">Jul 18</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">1</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Iizbakaokay"">Iizbakaokay</a> <a href=""/forum/?topicid=1534597&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 18, 1:32 PM</td>
			</tr>
			<tr id=""topicRow37"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow37""><a href=""javascript:void(0);"" data-id=""1473012"" class=""js-toggle-watching-topic-button""><span id=""wt1473012""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1473012"" data-row=""topicRow37"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1473012"">sort by episode duration</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Fuseteam"">Fuseteam</a></span> - <span class=""lightLink"">Jan 16</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">5</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Seyfert"">Seyfert</a> <a href=""/forum/?topicid=1473012&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 18, 1:48 AM</td>
			</tr>
			<tr id=""topicRow38"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow38""><a href=""javascript:void(0);"" data-id=""1534025"" class=""js-toggle-watching-topic-button""><span id=""wt1534025""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1534025"" data-row=""topicRow38"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1534025"">An awaiting database approval list feature</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/K-Anime"">K-Anime</a></span> - <span class=""lightLink"">Jul 16</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">3</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/BigOnAnime"">BigOnAnime</a> <a href=""/forum/?topicid=1534025&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 17, 10:35 PM</td>
			</tr>
			<tr id=""topicRow39"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow39""><a href=""javascript:void(0);"" data-id=""1533258"" class=""js-toggle-watching-topic-button""><span id=""wt1533258""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1533258"" data-row=""topicRow39"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1533258"">Disable small and colored text on forums?</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/kuchitsu"">kuchitsu</a></span> - <span class=""lightLink"">Jul 14</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">10</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/sunnysummerday"">sunnysummerday</a> <a href=""/forum/?topicid=1533258&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 16, 11:48 PM</td>
			</tr>
			<tr id=""topicRow40"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow40""><a href=""javascript:void(0);"" data-id=""1533916"" class=""js-toggle-watching-topic-button""><span id=""wt1533916""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1533916"" data-row=""topicRow40"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1533916"" class=""icon-forum-locked"">Online Chat</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/SasoriUchiha"">SasoriUchiha</a></span> - <span class=""lightLink"">Jul 16</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">3</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Tensho"">Tensho</a> <a href=""/forum/?topicid=1533916&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 16, 5:07 AM</td>
			</tr>
			<tr id=""topicRow41"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow41""><a href=""javascript:void(0);"" data-id=""1533830"" class=""js-toggle-watching-topic-button""><span id=""wt1533830""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1533830"" data-row=""topicRow41"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1533830"">MAL Episode Poll Statistics</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/koluffy"">koluffy</a></span> - <span class=""lightLink"">Jul 15</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">8</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/koluffy"">koluffy</a> <a href=""/forum/?topicid=1533830&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 15, 11:36 PM</td>
			</tr>
			<tr id=""topicRow42"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow42""><a href=""javascript:void(0);"" data-id=""1486715"" class=""js-toggle-watching-topic-button""><span id=""wt1486715""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1486715"" data-row=""topicRow42"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1486715"">Search: Add ""My List"" Filter </a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Scorch94"">Scorch94</a></span> - <span class=""lightLink"">Feb 29</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">1</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/rachelcp"">rachelcp</a> <a href=""/forum/?topicid=1486715&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 15, 4:18 AM</td>
			</tr>
			<tr id=""topicRow43"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow43""><a href=""javascript:void(0);"" data-id=""1437925"" class=""js-toggle-watching-topic-button""><span id=""wt1437925""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1437925"" data-row=""topicRow43"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1437925"">Light novel Section</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Yamakki"">Yamakki</a></span> - <span class=""lightLink"">Oct 12, 2015</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">20</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/worldeditor11"">worldeditor11</a> <a href=""/forum/?topicid=1437925&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 14, 2:30 PM</td>
			</tr>
			<tr id=""topicRow44"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow44""><a href=""javascript:void(0);"" data-id=""1533297"" class=""js-toggle-watching-topic-button""><span id=""wt1533297""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1533297"" data-row=""topicRow44"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1533297"">Give series discussion an ignore button</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Protaku"">Protaku</a></span> - <span class=""lightLink"">Jul 14</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">1</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Cnon"">Cnon</a> <a href=""/forum/?topicid=1533297&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 14, 1:45 PM</td>
			</tr>
			<tr id=""topicRow45"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow45""><a href=""javascript:void(0);"" data-id=""1533301"" class=""js-toggle-watching-topic-button""><span id=""wt1533301""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1533301"" data-row=""topicRow45"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1533301"">Detailed search with sorting</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Horodep"">Horodep</a></span> - <span class=""lightLink"">Jul 14</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">3</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Iizbakaokay"">Iizbakaokay</a> <a href=""/forum/?topicid=1533301&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 14, 11:16 AM</td>
			</tr>
			<tr id=""topicRow46"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow46""><a href=""javascript:void(0);"" data-id=""583071"" class=""js-toggle-watching-topic-button""><span id=""wt583071""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""583071"" data-row=""topicRow46"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=583071"">Not Yet Aired Anime Moved to 'Plan to Watch' List</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/ataraxial"">ataraxial</a></span> - <span class=""lightLink"">Apr 9, 2013</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">14</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Sidoen"">Sidoen</a> <a href=""/forum/?topicid=583071&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 14, 8:34 AM</td>
			</tr>
			<tr id=""topicRow47"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow47""><a href=""javascript:void(0);"" data-id=""1532165"" class=""js-toggle-watching-topic-button""><span id=""wt1532165""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1532165"" data-row=""topicRow47"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1532165"">Why Hasn't There Been A ManhwaManhua Section Added To The Forums?</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/TomDay"">TomDay</a></span> - <span class=""lightLink"">Jul 10</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">5</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/TomDay"">TomDay</a> <a href=""/forum/?topicid=1532165&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 14, 7:55 AM</td>
			</tr>
			<tr id=""topicRow48"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow48""><a href=""javascript:void(0);"" data-id=""1532472"" class=""js-toggle-watching-topic-button""><span id=""wt1532472""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1532472"" data-row=""topicRow48"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1532472"">Could you change the popularity system?</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Kenna_Pyralis"">Kenna_Pyralis</a></span> - <span class=""lightLink"">Jul 11</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">14</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/vigorousjammer"">vigorousjammer</a> <a href=""/forum/?topicid=1532472&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 14, 4:53 AM</td>
			</tr>
			<tr id=""topicRow49"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow49""><a href=""javascript:void(0);"" data-id=""1533190"" class=""js-toggle-watching-topic-button""><span id=""wt1533190""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1533190"" data-row=""topicRow49"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span>Poll:  <a href=""/forum/?topicid=1533190"">A suggestion for MAL profiles.</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/KingUmarBashir"">KingUmarBashir</a></span> - <span class=""lightLink"">Jul 13</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">2</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/KingUmarBashir"">KingUmarBashir</a> <a href=""/forum/?topicid=1533190&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 13, 9:00 PM</td>
			</tr>
			<tr id=""topicRow50"">
    	         <td class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 1px;"" width=""25"" align=""center"" id=""topicrow50""><a href=""javascript:void(0);"" data-id=""1532709"" class=""js-toggle-watching-topic-button""><span id=""wt1532709""><img src=""http://cdn.myanimelist.net/images/watch_n.gif"" title=""You are not watching this topic""></span></a></td>
                 <td class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;""><span style=""float: right;""><a href=""javascript:void(0);"" class=""js-ignore-topic-button"" data-id=""1532709"" data-row=""topicRow50"" data-mode=""1""><img src=""http://cdn.myanimelist.net/images/ignorethread.gif"" alt=""Ignore/Unignore this topic"" title=""Ignore/Unignore this topic"" border=""0""></a></span> <a href=""/forum/?topicid=1532709"">Date vs Alphabetical Searches</a> <small></small><br><span class=""forum_postusername""><a href=""/profile/Gymkata"">Gymkata</a></span> - <span class=""lightLink"">Jul 12</span></td>
			  <td align=""center"" width=""75"" class=""forum_boardrow2"" style=""border-width: 0px 1px 1px 0px;"">1</td>
				<td align=""right"" width=""130"" class=""forum_boardrow1"" style=""border-width: 0px 1px 1px 0px;"" nowrap="""">by <a href=""/profile/Iizbakaokay"">Iizbakaokay</a> <a href=""/forum/?topicid=1532709&amp;goto=lastpost"" title=""Go to the Last Post"">»»</a><br>Jul 12, 5:44 PM</td>
			</tr>
			</tbody></table>
    <div class=""spaceit"" style=""text-align: right;""><span class=""bgColor1"" style=""padding: 2px;"">Pages (105) <a href=""?board=4&amp;show=-100""></a> <a href=""?board=4&amp;show=-50""></a> [1] <a href=""?board=4&amp;show=50"">2</a> <a href=""?board=4&amp;show=100"">3</a> <a href=""?board=4&amp;show=50"">»</a> ... <a href=""?board=4&amp;show=5200"">Last »</a></span></div>
    </div><!-- end of contentHome -->

  <div class=""mauto clearfix pt24"" style=""width:760px;"">
    <div class=""fl-l"">
    

<div class=""_unit "" style=""width:336px;display: block !important;"" data-height=""280"">
  <div id=""pc_forum_detail_bottom_rec_l"" class="""" style=""width: 336px;""><div id=""google_ads_iframe_/84947469/pc_forum_detail_bottom_rec_l_0__container__"" style=""border: 0pt none;""><iframe id=""google_ads_iframe_/84947469/pc_forum_detail_bottom_rec_l_0"" title=""3rd party ad content"" name=""google_ads_iframe_/84947469/pc_forum_detail_bottom_rec_l_0"" width=""336"" height=""280"" scrolling=""no"" marginwidth=""0"" marginheight=""0"" frameborder=""0"" src=""javascript:&quot;<html><body style='background:transparent'></body></html>&quot;"" style=""border: 0px; vertical-align: bottom;""></iframe></div><iframe id=""google_ads_iframe_/84947469/pc_forum_detail_bottom_rec_l_0__hidden__"" title="""" name=""google_ads_iframe_/84947469/pc_forum_detail_bottom_rec_l_0__hidden__"" width=""0"" height=""0"" scrolling=""no"" marginwidth=""0"" marginheight=""0"" frameborder=""0"" src=""javascript:&quot;<html><body style='background:transparent'></body></html>&quot;"" style=""border: 0px; vertical-align: bottom; visibility: hidden; display: none;""></iframe></div>
</div>
  </div>
      <div class=""fl-r"">
    

<div class=""_unit "" style=""width:336px;display: block !important;"" data-height=""280"">
  <div id=""pc_forum_detail_bottom_rec_r"" class="""" style=""width: 336px;""><div id=""google_ads_iframe_/84947469/pc_forum_detail_bottom_rec_r_0__container__"" style=""border: 0pt none;""><iframe id=""google_ads_iframe_/84947469/pc_forum_detail_bottom_rec_r_0"" title=""3rd party ad content"" name=""google_ads_iframe_/84947469/pc_forum_detail_bottom_rec_r_0"" width=""300"" height=""250"" scrolling=""no"" marginwidth=""0"" marginheight=""0"" frameborder=""0"" src=""javascript:&quot;<html><body style='background:transparent'></body></html>&quot;"" style=""border: 0px; vertical-align: bottom;""></iframe></div><iframe id=""google_ads_iframe_/84947469/pc_forum_detail_bottom_rec_r_0__hidden__"" title="""" name=""google_ads_iframe_/84947469/pc_forum_detail_bottom_rec_r_0__hidden__"" width=""0"" height=""0"" scrolling=""no"" marginwidth=""0"" marginheight=""0"" frameborder=""0"" src=""javascript:&quot;<html><body style='background:transparent'></body></html>&quot;"" style=""border: 0px; vertical-align: bottom; visibility: hidden; display: none;""></iframe></div>
</div>
  </div>
  </div>
  </div>    <div class=""side-ad side-ad--l"">
    

<div class=""_unit "" style=""width:160px;display: block !important;"" data-height=""600"">
  <div id=""pc_forum_detail_side_sky_l"" class="""" style=""width:160px;"">
    <script type=""text/javascript"">
      googletag.cmd.push(function() {
             var slot = googletag.defineSlot(""/84947469/pc_forum_detail_side_sky_l"", [[160,600],[1,1]], ""pc_forum_detail_side_sky_l"").addService(googletag.pubads())
    .setTargeting(""adult"", ""gray"")
        .setTargeting(""utm_campaign"", ""ad_mal_forum_board_4"")
        .setCollapseEmptyDiv(true,true);googletag.enableServices();

    window.MAL.SkinAd.pushSKTag(""pc_forum_detail_side_sky_l"");    window.MAL.SkinAd.pushRefreshSkSlot(""pc_forum_detail_side_sky_l"", slot);
         });</script>
  </div>
</div>
  </div>

  <div class=""side-ad side-ad--r"">
    

<div class=""_unit "" style=""width:160px;display: block !important;"" data-height=""600"">
  <div id=""pc_forum_detail_side_sky_r"" class="""" style=""width:160px;"">
    <script type=""text/javascript"">
      googletag.cmd.push(function() {
             var slot = googletag.defineSlot(""/84947469/pc_forum_detail_side_sky_r"", [[160,600],[1,1]], ""pc_forum_detail_side_sky_r"").addService(googletag.pubads())
    .setTargeting(""adult"", ""gray"")
        .setCollapseEmptyDiv(true,true);googletag.enableServices();

    window.MAL.SkinAd.pushSKTag(""pc_forum_detail_side_sky_r"");    window.MAL.SkinAd.pushRefreshSkSlot(""pc_forum_detail_side_sky_r"", slot);
         });</script>
  </div>
</div>
  </div>


<!--  control container height -->
<div style=""clear:both;""></div>

<!-- end rightbody -->

</div><!-- wrapper -->


    <div id=""ad-skin-bg-right"" class=""ad-skin-side-outer ad-skin-side-bg bg-right"">
    <div id=""ad-skin-right"" class=""ad-skin-side right"" style=""display: none;"">
      <div id=""ad-skin-right-absolute-block"">
        <div id=""ad-skin-right-fixed-block""></div>
      </div>
    </div>
  </div>
</div><!-- #myanimelist -->

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


<div id=""fancybox-tmp""></div><div id=""fancybox-loading""><div></div></div><div id=""fancybox-overlay""></div><div id=""fancybox-wrap""><div id=""fancybox-outer""><div class=""fancy-bg"" id=""fancy-bg-n""></div><div class=""fancy-bg"" id=""fancy-bg-ne""></div><div class=""fancy-bg"" id=""fancy-bg-e""></div><div class=""fancy-bg"" id=""fancy-bg-se""></div><div class=""fancy-bg"" id=""fancy-bg-s""></div><div class=""fancy-bg"" id=""fancy-bg-sw""></div><div class=""fancy-bg"" id=""fancy-bg-w""></div><div class=""fancy-bg"" id=""fancy-bg-nw""></div><div id=""fancybox-inner""></div><a id=""fancybox-close""></a><a href=""javascript:;"" id=""fancybox-left""><span class=""fancy-ico"" id=""fancybox-left-ico""></span></a><a href=""javascript:;"" id=""fancybox-right""><span class=""fancy-ico"" id=""fancybox-right-ico""></span></a></div></div><iframe id=""google_osd_static_frame_3828419403306"" name=""google_osd_static_frame"" style=""display: none; width: 0px; height: 0px;""></iframe></body></html>";

#endregion

            if (string.IsNullOrEmpty(raw))
                return null;
            var doc = new HtmlDocument();
            doc.LoadHtml(raw);

            var topicContainer =
                doc.DocumentNode.Descendants("table")
                    .First(node => node.Attributes.Contains("id") && node.Attributes["id"].Value == "forumTopics");
            foreach (var topicRow in topicContainer.Descendants("tr").Skip(1)) //skip forum table header
            {
                var current = new ForumTopicEntry();
                var tds = topicRow.Descendants("td").ToList();

                current.Type = tds[1].ChildNodes[1].InnerText;

                var titleLinks = tds[1].Descendants("a").ToList();
                var titleLink = titleLinks[titleLinks.Count - 2];

                current.Title = titleLink.InnerText;
                current.Url = "http://myanimelist.net" + titleLink.Attributes["href"].Value;
                current.Id = titleLink.Attributes["href"].Value.Split('=').Last();

                var spans = tds[1].Descendants("span").ToList();
                current.Op = spans[1].InnerText;
                current.Created = spans[2].InnerText;

                current.Replies = tds[2].InnerText;

                current.LastPoster = tds[3].Descendants("a").First().InnerText;
                current.LastPostDate = tds[3].ChildNodes.Last().InnerText;

                output.Add(current);
            }

            return output;
        }

        private static string GetEndpoint(ForumBoards board)
        {
            if (board == ForumBoards.AnimeDisc || board == ForumBoards.MangaDisc)
                return $"?subboard={(int) board}";
            return $"?board={(int) board}";
        }
    }
}
