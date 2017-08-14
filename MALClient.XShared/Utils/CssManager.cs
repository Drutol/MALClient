using MALClient.Adapters;

namespace MALClient.XShared.Utils
{
    public abstract class CssManagerBase : ICssManager
    {
        private string ReplacedCss { get; set; }
        private string ReplacedCssHtmlBodyScrollEnabled { get; set; }
        private string ReplacedCssHtmlBodyMinWidth { get; set; }
        private string ReplacedBegin { get; set; }

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
                                    $notifyFunction$(getDocHeight(id).toString());
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

        protected abstract string AccentColour { get; }
        protected abstract string AccentColourLight { get; }
        protected abstract string AccentColourDark { get; }
        protected abstract string NotifyFunction { get; }
        protected abstract string ShadowsDefinition { get; }

        private void PrepareCss()
        {
            string css = Css;
            string bodyCss = CssHtmlBodyScrollEnabled;
            string bodyCssMinWidth = CssHtmlBodyMinWidth;

            css = css.Replace("AccentColourBase", AccentColour)
                .Replace("AccentColourLight", AccentColourLight)
                .Replace("AccentColourDark", AccentColourDark)
                .Replace("BodyBackgroundThemeColor",
                    Settings.SelectedTheme == 1 ? "#2d2d2d" : "#e6e6e6")
                .Replace("BodyForegroundThemeColor",
                    Settings.SelectedTheme == 1 ? "white" : "black")
                .Replace(
                    "HorizontalSeparatorColor",
                    Settings.SelectedTheme == 1 ? "#0d0d0d" : "#b3b3b3")
                .Replace("BodyBackgroundThemeDarkerColor",
                    Settings.SelectedTheme == 1 ? "#212121" : "#dadada")
                .Replace("ShadowDefinition", ShadowsDefinition);

            bodyCss = bodyCss.Replace("AccentColourBase", AccentColour).
                Replace("AccentColourLight", AccentColourLight).
                Replace("AccentColourDark", AccentColourDark)
                .Replace("BodyBackgroundThemeColor",
                    Settings.SelectedTheme == 1 ? "#2d2d2d" : "#e6e6e6")
                .Replace("BodyForegroundThemeColor",
                    Settings.SelectedTheme == 1 ? "white" : "black").Replace(
                    "HorizontalSeparatorColor",
                    Settings.SelectedTheme == 1 ? "#0d0d0d" : "#b3b3b3")
                .Replace("BodyBackgroundThemeDarkerColor",
                    Settings.SelectedTheme == 1 ? "#212121" : "#dadada");

            bodyCssMinWidth = bodyCssMinWidth.Replace("AccentColourBase", AccentColour).
                Replace("AccentColourLight", AccentColourLight).
                Replace("AccentColourDark", AccentColourDark)
                .Replace("BodyBackgroundThemeColor",
                    Settings.SelectedTheme == 1 ? "#2d2d2d" : "#e6e6e6")
                .Replace("BodyForegroundThemeColor",
                    Settings.SelectedTheme == 1 ? "white" : "black").Replace(
                    "HorizontalSeparatorColor",
                    Settings.SelectedTheme == 1 ? "#0d0d0d" : "#b3b3b3")
                .Replace("BodyBackgroundThemeDarkerColor",
                    Settings.SelectedTheme == 1 ? "#212121" : "#dadada");

            ReplacedBegin = Begin.Replace("$notifyFunction$", NotifyFunction);
            ReplacedCssHtmlBodyScrollEnabled = bodyCss;
            ReplacedCssHtmlBodyMinWidth = bodyCssMinWidth;
            ReplacedCss = css;
        }

        public string WrapWithCss(string html,bool withImgCss = true,int? minWidth = null)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            if(string.IsNullOrEmpty(ReplacedCss))
                PrepareCss();

            var css = withImgCss
                ? ReplacedCss.Insert(0,
                    minWidth == null
                        ? ReplacedCssHtmlBodyScrollEnabled
                        : ReplacedCssHtmlBodyMinWidth.Replace("MinimalWidth", minWidth.Value.ToString()))
                : ReplacedCss.Substring(320).Insert(0, minWidth == null
                    ? ReplacedCssHtmlBodyScrollEnabled
                    : ReplacedCssHtmlBodyMinWidth.Replace("MinimalWidth", minWidth.Value.ToString()));

            if (!Settings.ArticlesDisplayScrollBar)
                css += CssRemoveScrollbar;
            css += "</style>";
            return css + ReplacedBegin + html + End;
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

        private const string CssHtmlBodyMinWidth =
            @"
            <meta name=""viewport"" content=""width=device-width, initial-scale=1, minimum-scale=1"">
            <style type=""text/css"">@charset ""UTF-8"";
            html, body
	        {
		        background-color: BodyBackgroundThemeColor;
		        color: BodyForegroundThemeColor;
                font-family: 'Segoe UI';
                -webkit-align-content: center;
                min-width: MinimalWidthpx;
                width: MinimalWidthpx;
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
		        -webkit-box-shadow: ShadowDefinition rgba(0,0,0,0.58);
		        -moz-box-shadow: ShadowDefinition rgba(0,0,0,0.58);
		        box-shadow: ShadowDefinition rgba(0,0,0,0.58);
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
