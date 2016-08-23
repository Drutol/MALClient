using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace MalClient.Shared.ViewModels
{
    public interface IMainViewInteractions
    {
        Storyboard CurrentStatusStoryboard { get; }
        Storyboard CurrentOffStatusStoryboard { get; }
        Storyboard CurrentOffSubStatusStoryboard { get; }
        Storyboard PinDialogStoryboard { get; }
        Storyboard HidePinDialogStoryboard { get; }
        SplitViewDisplayMode CurrentDisplayMode { get; }
        void SearchInputFocus(FocusState state);
        void InitSplitter();
    }
}
