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
using Org.Zakariya.Flyoutmenu;

namespace MALClient.Android.Listeners
{
    public class MenuFlyoutSelectionListener : Java.Lang.Object, FlyoutMenuView.ISelectionListener
    {
        private readonly Action<FlyoutMenuView.MenuItem> _action;

        public MenuFlyoutSelectionListener(Action<FlyoutMenuView.MenuItem> action)
        {
            _action = action;
        }

        public void OnDismissWithoutSelection(FlyoutMenuView p0)
        {
           
        }

        public void OnItemSelected(FlyoutMenuView p0, FlyoutMenuView.MenuItem p1)
        {
            _action.Invoke(p1);
        }
    }
}