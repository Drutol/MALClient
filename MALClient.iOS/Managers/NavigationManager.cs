using System;
using System.Windows.Input;
using MALClient.XShared.NavArgs;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.iOS.Managers
{
	public class NavigationManager : INavMgr
	{
		public void CurrentMainViewOnBackRequested()
		{
			//throw new NotImplementedException();
		}

		public void CurrentOffViewOnBackRequested()
		{
			//throw new NotImplementedException();
		}

		public void DeregisterBackNav()
		{
			//throw new NotImplementedException();
		}

		public void RegisterBackNav(AnimeDetailsPageNavigationArgs args)
		{
			//throw new NotImplementedException();
		}

		public void RegisterBackNav(ProfilePageNavigationArgs args)
		{
			//throw new NotImplementedException();
		}

		public void RegisterBackNav(PageIndex page, object args, PageIndex source = PageIndex.PageAbout)
		{
			//throw new NotImplementedException();
		}

		public void RegisterOneTimeMainOverride(ICommand command)
		{
			//throw new NotImplementedException();
		}

		public void RegisterOneTimeOverride(ICommand command)
		{
			//throw new NotImplementedException();
		}

		public void ResetMainBackNav()
		{
			//throw new NotImplementedException();
		}

		public void ResetOffBackNav()
		{
			//throw new NotImplementedException();
		}

		public void ResetOneTimeMainOverride()
		{
			//throw new NotImplementedException();
		}

		public void ResetOneTimeOverride()
		{
			//throw new NotImplementedException();
		}
	}
}

