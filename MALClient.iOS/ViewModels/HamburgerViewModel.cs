using System;
using System.Threading.Tasks;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.iOS
{
	public class HamburgerViewModel : IHamburgerViewModel
	{
		public bool MangaSectionVisbility
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public void SetActiveButton(TopAnimeType topType)
		{
			throw new NotImplementedException();
		}

		public void SetActiveButton(HamburgerButtons val)
		{
			throw new NotImplementedException();
		}

		public void UpdateAnimeFiltersSelectedIndex()
		{
			throw new NotImplementedException();
		}

		public void UpdateApiDependentButtons()
		{
			throw new NotImplementedException();
		}

		public void UpdateLogInLabel()
		{
			//throw new NotImplementedException();
		}

		public void UpdatePinnedProfiles()
		{
			throw new NotImplementedException();
		}

		public Task UpdateProfileImg(bool dl = true)
		{
			throw new NotImplementedException();
		}
	}
}

