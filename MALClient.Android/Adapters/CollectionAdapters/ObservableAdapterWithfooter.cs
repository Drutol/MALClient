using System;
using Android.Views;
using GalaSoft.MvvmLight.Helpers;

namespace MALClient.Android.CollectionAdapters
{
    public class ObservableAdapterWithFooter<T> : ObservableAdapter<T>
    {
        public View Footer { get; set; }

        public override int Count => base.Count + 1;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (position == base.Count)
            {
                return Footer;
            }
            else
            {
                try
                {
                    return base.GetView(position, convertView == Footer ? null : convertView, parent);
                }
                catch (Exception)
                {
                    return convertView;
                }

            }
        }
    }
}