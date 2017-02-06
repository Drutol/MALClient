namespace Com.Daimajia.Swipe
{
    public partial class SwipeLayout
    {
        private Com.Daimajia.Swipe.SwipeLayout.ISwipeListener _swipeListener;

        public Com.Daimajia.Swipe.SwipeLayout.ISwipeListener SwipeListener
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