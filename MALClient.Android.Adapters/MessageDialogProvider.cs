using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using MALClient.Adapters;

namespace MALClient.Android.Adapters
{
    public class MessageDialogProvider : IMessageDialogProvider
    {
        class DialogDissmissListener : Java.Lang.Object, IDialogInterfaceOnDismissListener
        {
            private readonly Action _action;

            public DialogDissmissListener(Action action)
            {
                _action = action;
            }

            public void OnDismiss(IDialogInterface dialog)
            {
                _action.Invoke();
            }
        }

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

        public async Task ShowMessageDialogAsync(string content, string title)
        {
            var sem = new SemaphoreSlim(0);
            var dialog = new AlertDialog.Builder(SimpleIoc.Default.GetInstance<Activity>());
            dialog.SetNeutralButton("Go to MAL support", (sender, args) => { sem.Release(); });
            dialog.SetTitle(title);
            dialog.SetMessage(content);
            dialog.SetCancelable(false);
            dialog.Show();
            dialog.SetOnDismissListener(new DialogDissmissListener(() => sem.Release()));
            await sem.WaitAsync();
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
