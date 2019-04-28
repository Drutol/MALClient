using System;
using Android.Views;

namespace com.refractored
{
    public interface ICustomTabProvider
    {
        View GetCustomTabView(ViewGroup parent, int position);
    }
}

