using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Daimajia.Swipe;

namespace MALClient.Android.Listeners
{
    public class SwipeLayoutListener : Java.Lang.Object , SwipeLayout.ISwipeListener
    {
        public Action<SwipeLayout, float, float> OnUpdateAction { get; set; }
        public Action<SwipeLayout,float,float> OnReleaseAction { get; set; }
        public Action<SwipeLayout> OnOpenAction { get; set; }
        public bool IsSwiping { get; set; }

        public void OnClose(SwipeLayout p0)
        {
            IsSwiping = false;
        }

        public void OnHandRelease(SwipeLayout p0, float p1, float p2)
        {
            OnReleaseAction?.Invoke(p0,p1,p2);
        }

        public void OnOpen(SwipeLayout p0)
        {
            OnOpenAction?.Invoke(p0);
        }

        public void OnStartClose(SwipeLayout p0)
        {
            
        }

        public void OnStartOpen(SwipeLayout p0)
        {
            IsSwiping = true;
        }

        public void OnUpdate(SwipeLayout p0, int p1, int p2)
        {
            IsSwiping = true;
            OnUpdateAction?.Invoke(p0, p1, p2);
        }
    }
}