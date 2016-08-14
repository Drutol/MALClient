using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.ViewModels;

namespace MalClient.Shared.Delegates
{
    public delegate void OffContentPaneStateChanged();
    public delegate void AnimeItemListInitialized();
    public delegate void ScrollIntoViewRequest(AnimeItemViewModel item,bool select = false);
    public delegate void SelectionResetRequest(AnimeListDisplayModes mode);
    public delegate void SortingSettingChange(SortOptions option, bool descencing);
    public delegate void NavigationRequest(Type page, object args = null);
    public delegate void AmbiguousNavigationRequest(int enumId,object args);

    public delegate void WebViewNavigationRequest(string content, bool arg);
    public delegate void PivotItemSelectionRequest(int index);
}
