using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using MALClient.XShared.Utils;

namespace MALClient.UWP.Shared.Managers
{
    public static class CssManager
    {
        private static string ReplacedCss { get;  }
        private static string ReplacedCssHtmlBodyScrollEnabled { get;  }
//        document.addEventListener('click', function(e)
//        {
//            e = e || window.event;
//        var target = e.target || e.srcElement;
//                                    if(target.nodeName === 'A'){
//                                        target.className += ' font-bold';
//                                        setTimeout(function() { target.className -= ' font-bold'; }, 200);
//                                    }
//}, false);
//                                function loadLink(x, y)
//{
//    var el = document.elementFromPoint(x, y);
//    el && el.click();
//};
        private const string Begin = @"<html><head>
                            <meta name=""viewport"" content=""width=device-width, initial-scale=1, user-scalable=no"" />
                            <script type=""text/javascript"">

                                function getDocHeight(id) {
                                    var D = document;
                                    return Math.max(
                                        Math.max(document.getElementById(id).scrollHeight, document.getElementById(id).scrollHeight),
                                        Math.max(document.getElementById(id).offsetHeight, document.getElementById(id).offsetHeight),
                                        Math.max(document.getElementById(id).clientHeight, document.getElementById(id).clientHeight)
                                    );
                                };
                                function notifyDocumentHeightChanged(id){
                                    window.external.notify(getDocHeight(id).toString());
                                };                                
                                function bindButtons(){
                                    var classname = document.getElementsByTagName(""input"")
                                    for (var i = 0; i < classname.length; i++) {
                                        classname[i].addEventListener('click', function() {notifyDocumentHeightChanged(""content"");this.nextSibling.childNodes[0].style.display = 'none';}, false);
                                    }
                                };
   
                            </script>
                       </head><body id='root' onload='notifyDocumentHeightChanged(""content"");bindButtons();'><div id='content'>";
        private const string End = @"</div></body></html>";

        static CssManager()
        {
            var uiSettings = new UISettings();
            var color = uiSettings.GetColorValue(UIColorType.Accent);
            var color1 = uiSettings.GetColorValue(UIColorType.AccentDark2);
            var color2 = uiSettings.GetColorValue(UIColorType.AccentLight2);
            string css = Css;
            string bodyCss = CssHtmlBodyScrollEnabled;

            css = css.Replace("AccentColourBase", "#" + color.ToString().Substring(3)).
                Replace("AccentColourLight", "#" + color2.ToString().Substring(3)).
                Replace("AccentColourDark", "#" + color1.ToString().Substring(3))
                .Replace("BodyBackgroundThemeColor",
                    Settings.SelectedTheme == (int)ApplicationTheme.Dark ? "#2d2d2d" : "#e6e6e6")
                .Replace("BodyForegroundThemeColor",
                    Settings.SelectedTheme == (int)ApplicationTheme.Dark ? "white" : "black").Replace(
                    "HorizontalSeparatorColor",
                    Settings.SelectedTheme == (int)ApplicationTheme.Dark ? "#0d0d0d" : "#b3b3b3")
                .Replace("BodyBackgroundThemeDarkerColor",
                    Settings.SelectedTheme == (int)ApplicationTheme.Dark ? "#212121" : "#dadada");

            bodyCss = bodyCss.Replace("AccentColourBase", "#" + color.ToString().Substring(3)).
                Replace("AccentColourLight", "#" + color2.ToString().Substring(3)).
                Replace("AccentColourDark", "#" + color1.ToString().Substring(3))
                .Replace("BodyBackgroundThemeColor",
                    Settings.SelectedTheme == (int)ApplicationTheme.Dark ? "#2d2d2d" : "#e6e6e6")
                .Replace("BodyForegroundThemeColor",
                    Settings.SelectedTheme == (int)ApplicationTheme.Dark ? "white" : "black").Replace(
                    "HorizontalSeparatorColor",
                    Settings.SelectedTheme == (int)ApplicationTheme.Dark ? "#0d0d0d" : "#b3b3b3")
                .Replace("BodyBackgroundThemeDarkerColor",
                    Settings.SelectedTheme == (int)ApplicationTheme.Dark ? "#212121" : "#dadada");

            ReplacedCssHtmlBodyScrollEnabled = bodyCss;
            ReplacedCss = css;
        }

        public static string WrapWithCss(string html,bool disableScroll = false)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            var css = ReplacedCss.Insert(0, ReplacedCssHtmlBodyScrollEnabled);

            if (!Settings.ArticlesDisplayScrollBar)
                css += CssRemoveScrollbar;
            css += "</style>";
            return css + Begin + html + End;
        }

        #region Css

        public const string CssRemoveScrollbar = @"#root{
            height: 100%;
            width: 100%;
            overflow: hidden;
            position: relative;
        }

        #content{
            position: absolute;
            top: 0;
            bottom: 0;
            left: 0;
            right: -17px; /* Increase/Decrease this value for cross-browser compatibility */
            overflow-y: scroll;
            padding-right: 30px;
            padding-left: 5px;
            margin-bottom: 15px;
        }";

        private const string CssHtmlBodyScrollDisabled =
            @"<style type=""text/css"">@charset ""UTF-8"";
            html, body
	        {
		        background-color: BodyBackgroundThemeColor;
		        color: BodyForegroundThemeColor;
                font-family: 'Segoe UI';
                margin: 0; 
                height: 100%; 
                overflow: hidden;
	        }";
        private const string CssHtmlBodyScrollEnabled =
            @"<style type=""text/css"">@charset ""UTF-8"";
            html, body
	        {
		        background-color: BodyBackgroundThemeColor;
		        color: BodyForegroundThemeColor;
                font-family: 'Segoe UI';
	        }";
        private const string Css =
            @"
	        .userimg
	        {
		        display: block;
		        margin: 10px auto;
		        max-width: 100%;
		        height: auto;
		        -webkit-box-shadow: 0px 0px 67px 5px rgba(0,0,0,0.58);
		        -moz-box-shadow: 0px 0px 67px 5px rgba(0,0,0,0.58);
		        box-shadow: 0px 0px 67px 5px rgba(0,0,0,0.58);
	        }
	        a
	        {
		        font-weight: bold;
		        text-decoration: none;
	        }
            a:link{color:AccentColourBase}
            a:active{color:AccentColourBase}
            a:visited{color:AccentColourDark}
            a:hover{color:AccentColourLight}

        h1 {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 24px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 500;
	        line-height: 26.4px;
        }
        h2 {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 24px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 500;
	        line-height: 26.4px;
        }
        h3 {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 18px;
	        font-style: normal;
	        font-variant: normal;
	        position: relative;
	        text-align: center;
	        font-weight: 500;
	        line-height: 15.4px;
        }
        h4 {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 14px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 500;
	        line-height: 15.4px;
        }
        hr {
            display: block;
            height: 2px;
	        background-color: HorizontalSeparatorColor;
	        border-radius: 10px 10px 10px 10px;
	        -moz-border-radius: 10px 10px 10px 10px;
	        -webkit-border-radius: 10px 10px 10px 10px;
	        border: 1px solid #1f1f1f;
            margin: 1em 0;
            margin-right: 20px;
            padding-right: 0;
        }
        p {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 14px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 400;
	        line-height: 20px;
        }
        blockquote {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 21px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 400;
	        line-height: 30px;
        }
        pre {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 13px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 400;
	        line-height: 18.5714px;
        }

        .tags
        {
	        position: absolute;
            left: -9999px;
        }
        .js-sns-icon-container
        {
	        position: absolute;
            left: -9999px;
        }

        .news-info-block
        {
	        width: 100%;
	        border-style: solid;
            border-width: 0px 0px 2px 0px;
            border-color: rgba(0, 0, 0, 0);
        }

        .information
        {
	        font-family: 'Segoe UI', Frutiger, 'Frutiger Linotype', 'Dejavu Sans', 'Helvetica Neue', Arial, sans-serif;
	        font-size: 12px;
	        font-style: normal;
	        font-variant: normal;
	        font-weight: 500;
        }

        .quotetext
        {
        	border-style: solid;
    		border-width: 0px 0px 0px 2px;
    		padding: 10px;
    		border-color: AccentColourBase;
    		background: BodyBackgroundThemeDarkerColor;
        }

        .codetext
        {
    		padding: 7px;
    		background: BodyBackgroundThemeDarkerColor;
    		font-family: 'Consolas';
        }";

        #endregion
    }
}
