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
using Com.Mikepenz.Materialdrawer.Interfaces;
using Com.Mikepenz.Materialdrawer.Model.Interfaces;

namespace MALClient.Android.Listeners
{
    public class DrawerCheckedChangeListener : Java.Lang.Object, IOnCheckedChangeListener
    {
        private readonly Action<IDrawerItem,bool> _onChanged;

        public DrawerCheckedChangeListener(Action<IDrawerItem,bool> onChanged)
        {
            _onChanged = onChanged;
        }

        public void OnCheckedChanged(IDrawerItem p0, CompoundButton p1, bool p2)
        {
            _onChanged.Invoke(p0,p2);
        }
    }
}