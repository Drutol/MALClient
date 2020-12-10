using System;
using Android.Graphics;
using Android.Webkit;
using MALClient.XShared.Delegates;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Web
{
    public class ListenableWebClient : WebViewClient
    {
        public EmptyEventHander PageReady;
        public NavigationInterceptPossible NavigationInterceptOpportunity;

        private bool _loading;
        private bool _redirect;

        public bool NavigateIfNoInterception { get; set; }

        public override WebResourceResponse? ShouldInterceptRequest(WebView? view, IWebResourceRequest? request)
        {
            return base.ShouldInterceptRequest(view, request);
        }

        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            

            if (_loading)
                _redirect = true;

            InterceptUrl(view,url);
            return true;
        }

        private async void InterceptUrl(WebView view, string url)
        {
            var invoke = NavigationInterceptOpportunity?.Invoke(url);
            if (invoke != null)
            {
                var result = await invoke;
                if (!string.IsNullOrEmpty(result))
                    view.LoadUrl(result);
                else if (NavigateIfNoInterception)
                {
                    view.LoadUrl(url);
                }
            }
            else
            {
                if (NavigateIfNoInterception)
                {
                    view.LoadUrl(url);
                }
            }
        }

        public override void OnPageStarted(WebView view, string url, Bitmap favicon)
        {
            _loading = true;
            base.OnPageStarted(view, url, favicon);
        }

        public override void OnPageFinished(WebView view, string url)
        {
            if (!_redirect)
            {
                _loading = false;
            }

            if (!_loading && !_redirect)
            {
                PageReady?.Invoke();
            }
            else
            {
                _redirect = false;
            }
            base.OnPageFinished(view, url);
        }
    }
}