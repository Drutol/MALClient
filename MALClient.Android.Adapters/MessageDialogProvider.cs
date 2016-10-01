using System;
using Android.App;
using GalaSoft.MvvmLight.Views;
using MALClient.Adapters;

namespace MALClient.Android.Adapters
{
    public class MessageDialogProvider : IMessageDialogProvider
    {
        public async void ShowMessageDialog(string content, string title)
        {
            //var msg = new MessageDialog(content, title);
            //await msg.ShowAsync();
            var dialog = new AlertDialog.Builder(ActivityBase.CurrentActivity);
            dialog.SetNeutralButton("OK",(sender, args) => {});
            dialog.SetTitle(title);
            dialog.SetMessage(content);
            dialog.SetCancelable(false);
            dialog.Show();
        }

        public async void ShowMessageDialogWithInput(string content, string title,string trueCommand,string falseCommand, Action callbackOnTrue,Action callBackOnFalse = null)
        {
            //bool response = false;
            //var msg = new MessageDialog(content,title);
            //msg.Commands.Add(new UICommand(trueCommand,command => response = true));
            //msg.Commands.Add(new UICommand(falseCommand));
            //await msg.ShowAsync();
            //if(response)
            //    callbackOnTrue.Invoke();
            //else
            //    callBackOnFalse?.Invoke();
        }
    }
}
