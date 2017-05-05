using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace MALClient.Android.Listeners
{
    public class OnTextEnterListener : Java.Lang.Object, ITextWatcher
    {
        private readonly Action _onEnter;

        public OnTextEnterListener(Action onEnter)
        {
            _onEnter = onEnter;
        }

        public void AfterTextChanged(IEditable s)
        {

        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
 
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            if (!string.IsNullOrWhiteSpace(s.ToString()) && s.ToString().EndsWith("\n"))
                _onEnter.Invoke();
        }
    }
}