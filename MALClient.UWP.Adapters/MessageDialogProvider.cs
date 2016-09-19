using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using MALClient.Adapters;

namespace MALClient.UWP.Adapters
{
    public class MessageDialogProvider : IMessageDialogProvider
    {
        public async void ShowMessageDialog(string content, string title)
        {
            try
            {
                var msg = new MessageDialog(content, title);
                await msg.ShowAsync();
            }
            catch (Exception)
            {
                //core window blah blah blah
            }

        }

        public async void ShowMessageDialogWithInput(string content, string title, string trueCommand,
            string falseCommand, Action callbackOnTrue, Action callBackOnFalse = null)
        {
            try
            {
                bool response = false;
                var msg = new MessageDialog(content, title);
                msg.Commands.Add(new UICommand(trueCommand, command => response = true));
                msg.Commands.Add(new UICommand(falseCommand));
                await msg.ShowAsync();
                if (response)
                    callbackOnTrue.Invoke();
                else
                    callBackOnFalse?.Invoke();
            }
            catch (Exception)
            {
                //core window something something
            }

        }
    }
}
