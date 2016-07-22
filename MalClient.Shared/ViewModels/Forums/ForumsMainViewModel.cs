using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MalClient.Shared.Delegates;
using MalClient.Shared.NavArgs;
using MalClient.Shared.Utils.Enums;

namespace MalClient.Shared.ViewModels.Forums
{
    public class ForumsMainViewModel : ViewModelBase
    {
        public event AmbiguousNavigationRequest NavigationRequested;

        public async void Init(ForumsNavigationArgs args)
        {
            args = args ?? new ForumsNavigationArgs {Page = ForumsPageIndex.PageIndex};
            NavigationRequested?.Invoke((int)args.Page, args);
        }
    }
}
