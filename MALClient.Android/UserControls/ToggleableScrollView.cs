using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Hardware.Input;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MALClient.Android.Listeners;
using Object = Java.Lang.Object;
using String = System.String;

namespace MALClient.Android.UserControls
{
    public class ToggleableScrollView : ScrollView
    {
        private OverscrollingListView _bottomScrollingView;
        private bool _childSetUp;
        private MotionEvent _lastEvent;
        private bool _interceptTouchEvents = true;

        #region Constructors

        public ToggleableScrollView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ToggleableScrollView(Context context) : base(context)
        {
        }

        public ToggleableScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public ToggleableScrollView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public ToggleableScrollView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        #endregion

        private bool InterceptTouchEvents
        {
            get { return _interceptTouchEvents; }
            set
            {
                _interceptTouchEvents = value;
                _bottomScrollingView.InterceptTouchEvents = !value;
            }
        }

        public OverscrollingListView BottomScrollingView
        {
            get { return _bottomScrollingView; }
            set
            {
                _childSetUp = false;
                _bottomScrollingView = value;
                _bottomScrollingView.ReachedTop += OnReachedTop;
                _bottomScrollingView.LayoutParameters.Height = Height;
                _bottomScrollingView.OverScrollMode = OverScrollMode.Never;
            }
        }

        private void OnReachedTop(object sender, EventArgs eventArgs)
        {
            InterceptTouchEvents = true;
        }

        protected override  void OnOverScrolled(int scrollX, int scrollY, bool clampedX, bool clampedY)
        {
            if (scrollY > 0 && clampedY)
            {
                InterceptTouchEvents = false;

               DelayTouchEvent();
            }
            base.OnOverScrolled(scrollX, scrollY, clampedX, clampedY);
        }

        private async void DelayTouchEvent()
        {
            await Task.Delay(200);
            //_bottomScrollingView.DispatchTouchEvent(_lastEvent);
            //_bottomScrollingView.OnInterceptTouchEvent(_lastEvent);
            //_bottomScrollingView.OnTouchEvent(_lastEvent);
            String methodName = "getInstance";
            Object[] objArr = new Object[0];
            var im = (InputManager)Class.FromType(typeof(InputManager)).GetDeclaredMethod(methodName, new Class[0])
                .Invoke(null, objArr);


            //Get the reference to injectInputEvent method
            methodName = "injectInputEvent";
            var injectInputEventMethod = Class.FromType(typeof(InputManager)).GetMethod(methodName, new Class[]
            {
                Class.FromType(typeof(InputEvent)),Integer.Type
            });

            injectInputEventMethod.Invoke(im, new Object[] { _lastEvent, Integer.ValueOf(0) });
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return InterceptTouchEvents;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if(e.Action == MotionEventActions.Move)
                _lastEvent = MotionEvent.Obtain(e);
            if (InterceptTouchEvents)
                return base.OnTouchEvent(e);

            _bottomScrollingView.OnTouchEvent(e);
            return true;
        }
    }
}