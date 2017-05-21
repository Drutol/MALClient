using System;
using Android.App;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using MALClient.Adapters;

namespace MALClient.Android.Adapters
{
    public class MessageDialogProvider : IMessageDialogProvider
    {
        public void ShowMessageDialog(string content, string title)
        {
            //var msg = new MessageDialog(content, title);
            //await msg.ShowAsync();
            var dialog = new AlertDialog.Builder(SimpleIoc.Default.GetInstance<Activity>());
            dialog.SetNeutralButton("OK",(sender, args) => {});
            dialog.SetTitle(title);
            dialog.SetMessage(content);
            dialog.SetCancelable(true);
            dialog.Show();
        }

        public void ShowMessageDialogWithInput(string content, string title,string trueCommand,string falseCommand, Action callbackOnTrue,Action callBackOnFalse = null)
        {
            var dialog = new AlertDialog.Builder(SimpleIoc.Default.GetInstance<Activity>());
            dialog.SetPositiveButton(trueCommand, (sender, args) => callbackOnTrue.Invoke());
            dialog.SetNegativeButton(falseCommand, (sender, args) => callBackOnFalse?.Invoke());
            dialog.SetTitle(title);
            dialog.SetMessage(content);
            dialog.SetCancelable(false);
            dialog.Show();
        }
    }
}
