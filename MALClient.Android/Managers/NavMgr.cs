using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Managers
{
    public class NavMgr : INavMgr
    {
        public void RegisterBackNav(PageIndex page, object args, PageIndex source = PageIndex.PageAbout)
        {
            
        }

        public void RegisterOneTimeOverride(ICommand command)
        {
            
        }

        public void DeregisterBackNav()
        {
            
        }

        public void ResetOffBackNav()
        {
            
        }

        public void RegisterBackNav(ProfilePageNavigationArgs args)
        {
            
        }

        public void CurrentMainViewOnBackRequested()
        {
            
        }

        public void CurrentOffViewOnBackRequested()
        {
            
        }

        public void ResetMainBackNav()
        {
            
        }

        public void RegisterBackNav(AnimeDetailsPageNavigationArgs args)
        {
            
        }

        public void RegisterOneTimeMainOverride(ICommand command)
        {
            
        }

        public void ResetOneTimeOverride()
        {
            
        }

        public void ResetOneTimeMainOverride()
        {
            
        }
    }
}