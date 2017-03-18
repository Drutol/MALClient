using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Enums;
using MALClient.XShared.Comm.Anime;

namespace MALClient.XShared.Interfaces
{
    public interface IHamburgerViewModel
    {
        Task UpdateProfileImg(bool dl = true);
        //Desktop
        void SetActiveButton(HamburgerButtons val);
        void UpdateApiDependentButtons();
        void UpdateAnimeFiltersSelectedIndex();
        void UpdateLogInLabel();
        bool MangaSectionVisbility { get; set; }
        void SetActiveButton(TopAnimeType topType);
        void UpdatePinnedProfiles();
        void UpdateBottomMargin();
    }
}
