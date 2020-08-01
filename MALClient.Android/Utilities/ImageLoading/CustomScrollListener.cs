using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;

namespace MALClient.Android.Utilities.ImageLoading
{
    class CustomScrollListener : RecyclerView.OnScrollListener
    {
        public CustomScrollListener()
        {

        }

        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);

            switch (newState)
            {
                case RecyclerView.ScrollStateDragging:
                    ImageService.Instance.SetPauseWork(true);
                    //_onPause();
                    break;

                case RecyclerView.ScrollStateIdle:
                    ImageService.Instance.SetPauseWork(false);
                    //_onResume();
                    break;
            }
        }
    }
}