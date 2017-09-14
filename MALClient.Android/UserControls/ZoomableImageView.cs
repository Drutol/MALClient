using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using Java.Lang;
using MALClient.Android.Listeners;
using Math = System.Math;

namespace MALClient.Android.UserControls
{
    /// <summary>
    /// https://stackoverflow.com/questions/6650398/android-imageview-zoom-in-and-zoom-out
    /// </summary>
    public class ZoomableImageView : ImageViewAsync
    {
        Matrix matrix = new Matrix();

        static int NONE = 0;
        static int DRAG = 1;
        static int ZOOM = 2;
        static int CLICK = 3;
        int mode = NONE;

        PointF last = new PointF();
        PointF start = new PointF();
        float minScale = 1f;
        float maxScale = 4f;
        float[] m;

        float redundantXSpace, redundantYSpace;
        float width, height;
        float saveScale = 1f;
        public float right, bottom, origWidth, origHeight, bmWidth = DimensionsHelper.DpToPx(168), bmHeight = DimensionsHelper.DpToPx(300);

        ScaleGestureDetector mScaleDetector;
        Context context;


        public ZoomableImageView(Context context, IAttributeSet attr): base(context,attr)
        {

            Clickable = true;
            this.context = context;
            mScaleDetector = new ScaleGestureDetector(context, new ScaleListener(this));
            matrix.SetTranslate(1f, 1f);
            m = new float[9];
            ImageMatrix = matrix;
            SetScaleType(ScaleType.Matrix);

            SetOnTouchListener(new TouchListener(this));
        }

        private class ScaleListener : ScaleGestureDetector.SimpleOnScaleGestureListener
        {
            private ZoomableImageView _parent;

            public ScaleListener(ZoomableImageView parent)
            {
                _parent = parent;
            }

            public override bool OnScaleBegin(ScaleGestureDetector detector)
            {
                _parent.mode = ZOOM;
                return true;
            }


            public override bool OnScale(ScaleGestureDetector detector)
            {
                float mScaleFactor = detector.ScaleFactor;
                float origScale = _parent.saveScale;
                _parent.saveScale *= mScaleFactor;
                if (_parent.saveScale > _parent.maxScale)
                {
                    _parent.saveScale = _parent.maxScale;
                    mScaleFactor = _parent.maxScale / origScale;
                }
                else if (_parent.saveScale < _parent.minScale)
                {
                    _parent.saveScale = _parent.minScale;
                    mScaleFactor = _parent.minScale / origScale;
                }
                _parent.right = _parent.width * _parent.saveScale - _parent.width -
                                (2 * _parent.redundantXSpace * _parent.saveScale);
                _parent.bottom = _parent.height * _parent.saveScale - _parent.height -
                                 (2 * _parent.redundantYSpace * _parent.saveScale);
                if (_parent.origWidth * _parent.saveScale <= _parent.width ||
                    _parent.origHeight * _parent.saveScale <= _parent.height)
                {
                    _parent.matrix.PostScale(mScaleFactor, mScaleFactor, _parent.width / 2, _parent.height / 2);
                    if (mScaleFactor < 1)
                    {
                        _parent.matrix.GetValues(_parent.m);
                        float x = _parent.m[Matrix.MtransX];
                        float y = _parent.m[Matrix.MtransY];
                        if (mScaleFactor < 1)
                        {
                            if (Java.Lang.Math.Round(_parent.origWidth * _parent.saveScale) < _parent.width)
                            {
                                if (y < -_parent.bottom)
                                    _parent.matrix.PostTranslate(0, -(y + _parent.bottom));
                                else if (y > 0)
                                    _parent.matrix.PostTranslate(0, -y);
                            }
                            else
                            {
                                if (x < -_parent.right)
                                    _parent.matrix.PostTranslate(-(x + _parent.right), 0);
                                else if (x > 0)
                                    _parent.matrix.PostTranslate(-x, 0);
                            }
                        }
                    }
                }
                else
                {
                    _parent.matrix.PostScale(mScaleFactor, mScaleFactor, detector.FocusX, detector.FocusY);
                    _parent.matrix.GetValues(_parent.m);
                    float x = _parent.m[Matrix.MtransX];
                    float y = _parent.m[Matrix.MtransY];
                    if (mScaleFactor < 1)
                    {
                        if (x < -_parent.right)
                            _parent.matrix.PostTranslate(-(x + _parent.right), 0);
                        else if (x > 0)
                            _parent.matrix.PostTranslate(-x, 0);
                        if (y < -_parent.bottom)
                            _parent.matrix.PostTranslate(0, -(y + _parent.bottom));
                        else if (y > 0)
                            _parent.matrix.PostTranslate(0, -y);
                    }
                }
                return true;
            }
        }

        class TouchListener : Java.Lang.Object, IOnTouchListener
        {
            private ZoomableImageView _parent;

            public TouchListener(ZoomableImageView parent)
            {
                _parent = parent;
            }

            public bool OnTouch(View v, MotionEvent e)
            {
                _parent.mScaleDetector.OnTouchEvent(e);

                _parent.matrix.GetValues(_parent.m);
                float x = _parent.m[Matrix.MtransX];
                float y = _parent.m[Matrix.MtransY];
                PointF curr = new PointF(e.GetX(), e.GetY());

                switch (e.Action)
                {
                    //when one finger is touching
                    //set the mode to DRAG
                    case MotionEventActions.Down:
                        _parent.last.Set(e.GetX(), e.GetY());
                        _parent.start.Set(_parent.last);
                        _parent.mode = DRAG;
                        break;
                    //when two fingers are touching
                    //set the mode to ZOOM
                    case MotionEventActions.PointerDown:
                        _parent.last.Set(e.GetX(), e.GetY());
                        _parent.start.Set(_parent.last);
                        _parent.mode = ZOOM;
                        break;
                    //when a finger moves
                    //If mode is applicable move image
                    case MotionEventActions.Move:
                        //if the mode is ZOOM or
                        //if the mode is DRAG and already zoomed
                        if (_parent.mode == ZOOM || (_parent.mode == DRAG && _parent.saveScale > _parent.minScale))
                        {
                            float deltaX = curr.X - _parent.last.X; // x difference
                            float deltaY = curr.Y - _parent.last.Y; // y difference
                            float scaleWidth =
                                Java.Lang.Math.Round(_parent.origWidth *
                                                     _parent.saveScale); // _parent.width after applying current scale
                            float scaleHeight =
                                Java.Lang.Math.Round(_parent.origHeight *
                                                     _parent.saveScale); // _parent.height after applying current scale
                            //if scale_parent.width is smaller than the views _parent.width
                            //in other words if the image _parent.width fits in the view
                            //limit left and _parent.right movement
                            if (scaleWidth < _parent.width)
                            {
                                deltaX = 0;
                                if (y + deltaY > 0)
                                    deltaY = -y;
                                else if (y + deltaY < -_parent.bottom)
                                    deltaY = -(y + _parent.bottom);
                            }
                            //if scale_parent.height is smaller than the views _parent.height
                            //in other words if the image _parent.height fits in the view
                            //limit up and down movement
                            else if (scaleHeight < _parent.height)
                            {
                                deltaY = 0;
                                if (x + deltaX > 0)
                                    deltaX = -x;
                                else if (x + deltaX < -_parent.right)
                                    deltaX = -(x + _parent.right);
                            }
                            //if the image doesnt fit in the _parent.width or height
                            //limit both up and down and left and _parent.right
                            else
                            {
                                if (x + deltaX > 0)
                                    deltaX = -x;
                                else if (x + deltaX < -_parent.right)
                                    deltaX = -(x + _parent.right);

                                if (y + deltaY > 0)
                                    deltaY = -y;
                                else if (y + deltaY < -_parent.bottom)
                                    deltaY = -(y + _parent.bottom);
                            }
                            //move the image with the matrix
                            _parent.matrix.PostTranslate(deltaX, deltaY);
                            //set the last touch location to the current
                            _parent.last.Set(curr.X, curr.Y);
                        }
                        break;
                    //first finger is lifted
                    case MotionEventActions.Up:
                        _parent.mode = NONE;
                        int xDiff = (int) Java.Lang.Math.Abs(curr.X - _parent.start.X);
                        int yDiff = (int) Java.Lang.Math.Abs(curr.Y - _parent.start.Y);
                        if (xDiff < CLICK && yDiff < CLICK)
                            _parent.PerformClick();
                        break;
                    // second finger is lifted
                    case MotionEventActions.PointerUp:
                        _parent.mode = NONE;
                        break;
                }
                _parent.ImageMatrix = _parent.matrix;
                _parent.Invalidate();
                return true;
            }

        }

        

        public override void SetImageBitmap(Bitmap bm)
        {
            base.SetImageBitmap(bm);
            bmWidth = bm.Width;
            bmHeight = bm.Height;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            width = MeasureSpec.GetSize(widthMeasureSpec);
            height = MeasureSpec.GetSize(heightMeasureSpec);
            //Fit to screen.
            float scale;
            float scaleX = width / bmWidth;
            float scaleY = height / bmHeight;
            scale = Math.Min(scaleX, scaleY);
            matrix.SetScale(scale, scale);
            ImageMatrix = matrix;
            saveScale = 1f;

            // Center the image
            redundantYSpace = height - (scale * bmHeight);
            redundantXSpace = width - (scale * bmWidth);
            redundantYSpace /= 2;
            redundantXSpace /= 2;

            matrix.PostTranslate(redundantXSpace, redundantYSpace);

            origWidth = width - 2 * redundantXSpace;
            origHeight = height - 2 * redundantYSpace;
            right = width * saveScale - width - (2 * redundantXSpace * saveScale);
            bottom = height * saveScale - height - (2 * redundantYSpace * saveScale);
            ImageMatrix = matrix;
        }
    }
}
