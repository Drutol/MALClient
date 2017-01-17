using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.Models.Enums;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Utils.Enums;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.ViewModels
{
    public class HamburgerControlViewModel : IHamburgerViewModel
    {
        public Task UpdateProfileImg(bool dl = true)
        {
            return Task.CompletedTask;
        }

        public void SetActiveButton(HamburgerButtons val)
        {
           
        }

        public void UpdateApiDependentButtons()
        {
            
        }

        public void UpdateAnimeFiltersSelectedIndex()
        {
           
        }

        public void UpdateLogInLabel()
        {
           
        }

        public bool MangaSectionVisbility { get; set; }

        public void SetActiveButton(TopAnimeType topType)
        {
            
        }

        public void UpdatePinnedProfiles()
        {
            
        }

        public void UpdateBottomMargin()
        {
            
        }
    }
}