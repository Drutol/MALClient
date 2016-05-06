using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MALClient.Comm;
using MALClient.Models.ApiResponses;

namespace MALClient.ViewModels
{
    public class HummingbirdProfilePageViewModel : ViewModelBase
    {
        public HumProfileData CurrentData { get; set; } = new HumProfileData();

        private bool _loaded;
        public async void Init()
        {
            if (!_loaded)
            {
                CurrentData = await new ProfileQuery().GetHumProfileData();
                RaisePropertyChanged(() => CurrentData);
            }    
        }
    }
}
