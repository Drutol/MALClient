using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using MALClient.Adapters;

namespace MALClient.Android.Adapters
{
    public class MessageDialogProvider : IMessageDialogProvider
    {
        private AlertDialog _currentLoadingDialog;

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
            dialog.SetNeutralButton("OK", (sender, args) => { });
            dialog.SetTitle(title);
            dialog.SetMessage(content);
            dialog.SetCancelable(true);
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

        public void ShowChooseDialog(string content, string title,string cancelCommand, string trueCommand,string falseCommand, Action callbackOnTrue, Action callBackOnFalse = null)
        {
            var dialog = new AlertDialog.Builder(SimpleIoc.Default.GetInstance<Activity>());
            dialog.SetPositiveButton(trueCommand, (sender, args) => callbackOnTrue.Invoke());
            dialog.SetNegativeButton(falseCommand, (sender, args) => callBackOnFalse?.Invoke());
            dialog.SetNeutralButton(cancelCommand, (sender, args) => {});
            dialog.SetTitle(title);
            dialog.SetMessage(content);
            dialog.SetCancelable(true);
            dialog.Show();
        }

        public void UpdateLoadingPopup(string title, string content)
        {
            if (_currentLoadingDialog != null)
            {
                _currentLoadingDialog.SetTitle(title);
                _currentLoadingDialog.SetMessage(content);
            }
        }

        public void ShowLoadingPopup(string title, string content)
        {
            _currentLoadingDialog?.Dismiss();
            var ctx = SimpleIoc.Default.GetInstance<Activity>();

            var bottomMargin = title == null && content == null ? 16 : 32;

            var layout = new FrameLayout(ctx);
            var loadingView = new ProgressBar(ctx)
            {
                LayoutParameters = new FrameLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent)
                {
                    Gravity = GravityFlags.Center,
                    TopMargin = (int)(16 * Application.Context.Resources.DisplayMetrics.Density),
                    BottomMargin = (int)(bottomMargin * Application.Context.Resources.DisplayMetrics.Density),
                }
            };
            layout.AddView(loadingView);

            var dialog = new AlertDialog.Builder(ctx);

            dialog.SetView(layout);
            dialog.SetTitle(title);
            dialog.SetMessage(content);
            dialog.SetCancelable(false);
            _currentLoadingDialog = dialog.Show();
        }

        public void HideLoadingDialog()
        {
            _currentLoadingDialog?.Dismiss();
            _currentLoadingDialog = null;
        }
    }
}
