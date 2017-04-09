using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;

namespace MALClient.Android.Listeners
{
    public class DrawerListener : Java.Lang.Object, DrawerLayout.IDrawerListener
    {
        private readonly Action _onClose;
        private readonly Action _onOpen;

        public DrawerListener(Action onClose,Action onOpen)
        {
            _onClose = onClose;
            _onOpen = onOpen;
        }

        public void OnDrawerClosed(View drawerView)
        {
            _onClose?.Invoke();
        }

        public void OnDrawerOpened(View drawerView)
        {
            _onOpen?.Invoke();
        }

        public void OnDrawerSlide(View drawerView, float slideOffset)
        {

        }

        public void OnDrawerStateChanged(int newState)
        {

        }
    }
}