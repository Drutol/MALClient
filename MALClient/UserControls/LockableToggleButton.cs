using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace MALClient.UserControls
{
    public class LockableToggleButton : ToggleButton
    {
        // Using a DependencyProperty as the backing store for LockToggle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LockToggleProperty =
            DependencyProperty.Register("LockToggle", typeof (bool), typeof (LockableToggleButton),
                new PropertyMetadata(false));

        public bool LockToggle
        {
            get { return (bool) GetValue(LockToggleProperty); }
            set { SetValue(LockToggleProperty, value); }
        }

        protected override void OnToggle()
        {
            if (!LockToggle)
            {
                base.OnToggle();
            }
        }
    }
}