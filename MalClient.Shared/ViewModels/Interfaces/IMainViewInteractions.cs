using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace MALClient.Shared.ViewModels.Interfaces
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
