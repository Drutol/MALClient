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
using Com.Mikepenz.Materialdrawer;
using Com.Mikepenz.Materialdrawer.Model.Interfaces;

namespace MALClient.Android.Listeners
{
    public class HamburgerItemClickListener : Java.Lang.Object, Drawer.IOnDrawerItemClickListener
    {
        private readonly Action<View, int, IDrawerItem> _action;

        public HamburgerItemClickListener(Action<View, int, IDrawerItem> action)
        {
            _action = action;
        }

        public bool OnItemClick(View p0, int p1, IDrawerItem p2)
        {
            _action.Invoke(p0,p1,p2);
            return true;
        }
    }
}