using System;
using System.Windows.Input;

namespace Com.Shehabic.Droppy
{ 
    public partial class DroppyMenuPopup
    {
        public static event EventHandler<Action> OverrideRequested;
        public static event EventHandler ResetOverrideRequested;
        public event EventHandler OnHiddenWithoutSelection;

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

        public object Tag { get; set; }

        public void Show()
        {
            ShowBase();
            MenuView.Elevation = RequestedElevation;
            OverrideRequested.Invoke(this, () =>
            {
                Dismiss(true);
                OnHiddenWithoutSelection?.Invoke(this,EventArgs.Empty);
            });
            MOnDismissCallback = new FlyoutDismissListener(() => ResetOverrideRequested.Invoke(this,EventArgs.Empty));
        }

        public void Dismiss(bool animated)
        {
            if(OnHiddenWithoutSelection == null)
                ResetOverrideRequested.Invoke(this,EventArgs.Empty);
            DismissBase(animated);
        }

        
    }
}