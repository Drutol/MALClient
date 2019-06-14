using System;
using System.Diagnostics;
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

    public interface IBugFixingGridViewAdapter
    {
        bool HandledGridViewBug { get; set; }
    }

    public class ObservableGridViewAdapterWithFooter<T> : ObservableAdapter<T> , IBugFixingGridViewAdapter
    {
        public bool HandledGridViewBug { get; set; }
        private int _returnPasses;
        private View _cachedFirstView;

        public View Footer { get; set; }
       
        public override int Count => base.Count + 1;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (position == base.Count)
            {
                if(Footer == null)
                    Debugger.Break();
                return Footer;
            }
            else
            {
                try
                {
                    if (!HandledGridViewBug)
                        if (position == 0)
                        {
                            if (_cachedFirstView != null)
                            {
                                if (_returnPasses++ == 3)
                                    HandledGridViewBug = true;
                                return _cachedFirstView;
                            }
                            var v = base.GetView(position, convertView == Footer ? null : convertView, parent);
                            _cachedFirstView = v;
                            if (v == null)
                                Debugger.Break();
                            return v;
                        }
                    return base.GetView(position, convertView == Footer ? null : convertView, parent);
                }
                catch (Exception e)
                {
                    if (convertView == null)
                        Debugger.Break();
                    return convertView;
                }

            }
        }
    }
}