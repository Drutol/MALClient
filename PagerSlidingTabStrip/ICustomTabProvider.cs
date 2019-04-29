using System;
using Android.Views;

namespace com.refractored
{
    public interface ICustomTabProvider
    {
        View GetCustomTabView(ViewGroup parent, int position);
        void TabSelected(View p0);
        void TabUnselected(View p0);
    }
}

