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

namespace Com.Daimajia.Swipe
{
    public partial class SwipeLayout
    {
        private ISwipeListener _swipeListener;

        public ISwipeListener SwipeListener
        {
            get { return _swipeListener; }
            set
            {
                if (_swipeListener != null)
                    RemoveSwipeListener(_swipeListener);

                _swipeListener = value;
                AddSwipeListener(_swipeListener);
            }
        }
    }
}