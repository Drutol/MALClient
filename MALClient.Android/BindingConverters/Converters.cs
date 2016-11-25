using Android.Views;

namespace MALClient.Android.BindingConverters
{
    public static class Converters
    {
        public static ViewStates BoolToVisibility(bool arg)
        {
            return arg ? ViewStates.Visible : ViewStates.Gone;
        }

        public static ViewStates BoolToVisibilityInverted(bool arg)
        {
            return arg ? ViewStates.Gone : ViewStates.Visible;
        }

        public static ViewStates VisibilityInverterConverter(ViewStates arg)
        {
            return arg == ViewStates.Visible ? ViewStates.Gone : ViewStates.Visible;
        }

        public static ViewStates IsStringEmptyToVisibility(string arg)
        {
            return string.IsNullOrEmpty(arg) ? ViewStates.Gone : ViewStates.Visible;
        }
    }
}