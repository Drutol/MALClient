using System;
using System.Windows.Input;

namespace Com.Shehabic.Droppy
{ 
    public partial class DroppyMenuPopup
    {
        public static event EventHandler<Action> OverrideRequested;
        public static event EventHandler ResetOverrideRequested;

        class FlyoutDismissListener : Java.Lang.Object, IOnDismissCallback
        {
            private readonly Action _onDismiss;

            public FlyoutDismissListener(Action onDismiss)
            {
                _onDismiss = onDismiss;
            }

            public void Call()
            {
                _onDismiss.Invoke();
            }
        }

        public static float RequestedElevation { get; set; } = 0;

        public void Show()
        {
            ShowBase();
            MenuView.Elevation = RequestedElevation;
            OverrideRequested.Invoke(this,() => Dismiss(true));
            MOnDismissCallback = new FlyoutDismissListener(() => ResetOverrideRequested.Invoke(this,EventArgs.Empty));
        }

        public void Dismiss(bool animated)
        {
            ResetOverrideRequested.Invoke(this,EventArgs.Empty);
            DismissBase(animated);
        }

        
    }
}