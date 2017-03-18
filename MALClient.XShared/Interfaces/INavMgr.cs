using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MALClient.Models.Enums;
using MALClient.XShared.NavArgs;

namespace MALClient.XShared.Interfaces
{
    public interface INavMgr
    {
        void RegisterBackNav(PageIndex page, object args, PageIndex source = PageIndex.PageAbout);
        void RegisterOneTimeOverride(ICommand command);
        void DeregisterBackNav();
        void ResetOffBackNav();
        //Desktop
        void RegisterBackNav(ProfilePageNavigationArgs args);
        void CurrentMainViewOnBackRequested();
        void CurrentOffViewOnBackRequested();
        void ResetMainBackNav();
        void RegisterBackNav(AnimeDetailsPageNavigationArgs args);
        void RegisterUnmonitoredMainBackNav(PageIndex page, object args);
        void RegisterOneTimeMainOverride(ICommand command);
        void ResetOneTimeOverride();
        void ResetOneTimeMainOverride();
    }
}
