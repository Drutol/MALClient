using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Adapters
{
    public interface IMessageDialogProvider
    {
        void ShowMessageDialog(string content, string title);
        Task ShowMessageDialogAsync(string content, string title);

        void ShowMessageDialogWithInput(string content, string title, string trueCommand, string falseCommand,
            Action callbackOnTrue, Action callBackOnFalse = null);

        void ShowLoadingPopup(string title, string content);
        void HideLoadingDialog();
        void UpdateLoadingPopup(string title, string content);
    }
}
