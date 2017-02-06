using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using WinRTXamlToolkit.Controls.Extensions;

namespace MALClient.UWP.Shared.UserControls
{

    static class Extensions
     {
         internal static bool IsNaN(this double d)
         {
             return Double.IsNaN(d);
         }
     }

    [TemplatePart(Name = CanvasName, Type = typeof(Canvas))]
    [TemplatePart(Name = ImageName, Type = typeof(Image))]
    public class CropControl : Control
    {
        private const string CanvasName = "PART_Canvas";
        private const string ImageName = "PART_Image";

        // Loaded controls
        private Canvas _canvas;
        private Image _image;

        // Created controls
        private Rectangle _leftMaskingRectangle;
        private Rectangle _topMaskingRectangle;
        private Rectangle _rightMaskingRectangle;
        private Rectangle _bottomMaskingRectangle;
        private Shape _topLeftDragControl;
        private Shape _topRightDragControl;
        private Shape _bottomLeftDragControl;
        private Shape _bottomRightDragControl;
        private Line _leftLine;
        private Line _topLine;
        private Line _rightLine;
        private Line _bottomLine;
        private Line _topMiddleLine;
        private Line _bottomMiddleLine;
        private Line _leftMiddleLine;
        private Line _rightMiddleLine;

        // Status
        private bool _isLoaded;
        private DragMode _dragMode;
        private bool _isManipulating;
        private bool _isTemplateApplied;
        private bool _isImageLoaded;

        // Calculated locations and values
        private double _scalingFactor;
        private double _leftImageOffset;
        private double _topImageOffset;
        private double _cropLeftAtManipulationStart;
        private double _cropTopAtManipulationStart;
        private double _cropRightAtManipulationStart;
        private double _cropBottomAtManipulationStart;
        private double _cropLeft;
        private double _cropTop;
        private double _cropRight;
        private double _cropBottom;

        #region DragMode
        private enum DragMode
        {
            None = 0,
            TopLeft,
            Top,
            TopRight,
            Right,
            BottomRight,
            Bottom,
            BottomLeft,
            Left,
            Full
        };
        #endregion

        #region ActualAspectRatio
        /// <summary>
        /// ActualAspectRatio Dependency Property
        /// </summary>
        public static readonly DependencyProperty ActualAspectRatioProperty =
            DependencyProperty.Register(
                "ActualAspectRatio",
                typeof(double),
                typeof(CropControl),
                new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the ActualAspectRatio property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public double ActualAspectRatio
        {
            get { return (double)GetValue(ActualAspectRatioProperty); }
            set { SetValue(ActualAspectRatioProperty, value); }
        }
        #endregion

        #region DesiredAspectRatio
        /// <summary>
        /// DesiredAspectRatio Dependency Property
        /// </summary>
        public static readonly DependencyProperty DesiredAspectRatioProperty =
            DependencyProperty.Register(
                "DesiredAspectRatio",
                typeof(double),
                typeof(CropControl),
                new PropertyMetadata(Double.NaN, OnDesiredAspectRatioChanged));

        /// <summary>
        /// Gets or sets the DesiredAspectRatio property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public double DesiredAspectRatio
        {
            get { return (double)GetValue(DesiredAspectRatioProperty); }
            set { SetValue(DesiredAspectRatioProperty, value); }
        }

        private bool IsFixedAspectRatio
        {
            get { return !DesiredAspectRatio.IsNaN(); }
        }
        #endregion

        #region Foreground
        /// <summary>
        /// Foreground Dependency Property
        /// </summary>
        public new static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(
                "Foreground",
                typeof(Brush),
                typeof(CropControl),
                new PropertyMetadata(new SolidColorBrush(Colors.White), OnLayoutPropertyChanged));

        /// <summary>
        /// Gets or sets the Foreground property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public new Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }
        #endregion

        #region DragControlSize
        /// <summary>
        /// DragControlSize Dependency Property
        /// </summary>
        public static readonly DependencyProperty DragControlSizeProperty =
            DependencyProperty.Register(
                "DragControlSize",
                typeof(double),
                typeof(CropControl),
                new PropertyMetadata(30.0, OnLayoutPropertyChanged));

        /// <summary>
        /// Gets or sets the DragControlSize property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public double DragControlSize
        {
            get { return (double)GetValue(DragControlSizeProperty); }
            set { SetValue(DragControlSizeProperty, value); }
        }
        #endregion

        #region DragControlStrokeThickness
        /// <summary>
        /// DragControlStrokeThickness Dependency Property
        /// </summary>
        public static readonly DependencyProperty DragControlStrokeThicknessProperty =
            DependencyProperty.Register(
                "DragControlStrokeThickness",
                typeof(double),
                typeof(CropControl),
                new PropertyMetadata(3.0, OnLayoutPropertyChanged));

        /// <summary>
        /// Gets or sets the DragControlStrokeThickness property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public double DragControlStrokeThickness
        {
            get { return (double)GetValue(DragControlStrokeThicknessProperty); }
            set { SetValue(DragControlStrokeThicknessProperty, value); }
        }
        #endregion

        #region OutsideLineThickness
        /// <summary>
        /// OutsideLineThickness Dependency Property
        /// </summary>
        public static readonly DependencyProperty OutsideLineThicknessProperty =
            DependencyProperty.Register(
                "OutsideLineThickness",
                typeof(double),
                typeof(CropControl),
                new PropertyMetadata(2.0, OnLayoutPropertyChanged));

        /// <summary>
        /// Gets or sets the OutsideLineThickness property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public double OutsideLineThickness
        {
            get { return (double)GetValue(OutsideLineThicknessProperty); }
            set { SetValue(OutsideLineThicknessProperty, value); }
        }
        #endregion

        #region InsideLineThickness
        /// <summary>
        /// InsideLineThickness Dependency Property
        /// </summary>
        public static readonly DependencyProperty InsideLineThicknessProperty =
            DependencyProperty.Register(
                "InsideLineThickness",
                typeof(double),
                typeof(CropControl),
                new PropertyMetadata(1.0, OnLayoutPropertyChanged));

        /// <summary>
        /// Gets or sets the InsideLineThickness property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public double InsideLineThickness
        {
            get { return (double)GetValue(InsideLineThicknessProperty); }
            set { SetValue(InsideLineThicknessProperty, value); }
        }
        #endregion

        #region MaskingBrush
        /// <summary>
        /// MaskingBrush Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaskingBrushProperty =
            DependencyProperty.Register(
                "MaskingBrush",
                typeof(Brush),
                typeof(CropControl),
                new PropertyMetadata(new SolidColorBrush(new Color { A = 0x44 }), OnLayoutPropertyChanged));

        /// <summary>
        /// Gets or sets the MaskingBrush property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public Brush MaskingBrush
        {
            get { return (Brush)GetValue(MaskingBrushProperty); }
            set { SetValue(MaskingBrushProperty, value); }
        }
        #endregion

        #region OriginalWidth
        /// <summary>
        /// OriginalWidth Dependency Property
        /// </summary>
        public static readonly DependencyProperty OriginalWidthProperty =
            DependencyProperty.Register(
                "OriginalWidth",
                typeof(int),
                typeof(CropControl),
                new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the OriginalWidth property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public int OriginalWidth
        {
            get { return (int)GetValue(OriginalWidthProperty); }
            set { SetValue(OriginalWidthProperty, value); }
        }
        #endregion

        #region OriginalHeight
        /// <summary>
        /// OriginalHeight Dependency Property
        /// </summary>
        public static readonly DependencyProperty OriginalHeightProperty =
            DependencyProperty.Register(
                "OriginalHeight",
                typeof(int),
                typeof(CropControl),
                new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the OriginalHeight property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public int OriginalHeight
        {
            get { return (int)GetValue(OriginalHeightProperty); }
            set { SetValue(OriginalHeightProperty, value); }
        }
        #endregion

        #region CropLeft
        /// <summary>
        /// CropLeft Dependency Property
        /// </summary>
        public static readonly DependencyProperty CropLeftProperty =
            DependencyProperty.Register(
                "CropLeft",
                typeof(int),
                typeof(CropControl),
                new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the CropLeft property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public int CropLeft
        {
            get { return (int)GetValue(CropLeftProperty); }
            set { SetValue(CropLeftProperty, value); }
        }
        #endregion

        #region CropTop
        /// <summary>
        /// CropTop Dependency Property
        /// </summary>
        public static readonly DependencyProperty CropTopProperty =
            DependencyProperty.Register(
                "CropTop",
                typeof(int),
                typeof(CropControl),
                new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the CropTop property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public int CropTop
        {
            get { return (int)GetValue(CropTopProperty); }
            set { SetValue(CropTopProperty, value); }
        }
        #endregion

        #region CropRight
        /// <summary>
        /// CropRight Dependency Property
        /// </summary>
        public static readonly DependencyProperty CropRightProperty =
            DependencyProperty.Register(
                "CropRight",
                typeof(int),
                typeof(CropControl),
                new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the CropRight property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public int CropRight
        {
            get { return (int)GetValue(CropRightProperty); }
            set { SetValue(CropRightProperty, value); }
        }
        #endregion

        #region CropBottom
        /// <summary>
        /// CropBottom Dependency Property
        /// </summary>
        public static readonly DependencyProperty CropBottomProperty =
            DependencyProperty.Register(
                "CropBottom",
                typeof(int),
                typeof(CropControl),
                new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the CropBottom property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public int CropBottom
        {
            get { return (int)GetValue(CropBottomProperty); }
            set { SetValue(CropBottomProperty, value); }
        }
        #endregion

        #region CropWidth
        /// <summary>
        /// CropWidth Dependency Property
        /// </summary>
        public static readonly DependencyProperty CropWidthProperty =
            DependencyProperty.Register(
                "CropWidth",
                typeof(int),
                typeof(CropControl),
                new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the CropWidth property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public int CropWidth
        {
            get { return (int)GetValue(CropWidthProperty); }
            set { SetValue(CropWidthProperty, value); }
        }
        #endregion

        #region CropHeight
        /// <summary>
        /// CropHeight Dependency Property
        /// </summary>
        public static readonly DependencyProperty CropHeightProperty =
            DependencyProperty.Register(
                "CropHeight",
                typeof(int),
                typeof(CropControl),
                new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the CropHeight property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public int CropHeight
        {
            get { return (int)GetValue(CropHeightProperty); }
            set { SetValue(CropHeightProperty, value); }
        }
        #endregion

        #region ImageSource
        /// <summary>
        /// ImageSource Dependency Property
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                "ImageSource",
                typeof(ImageSource),
                typeof(CropControl),
                new PropertyMetadata(null, OnImageSourceChanged));

        /// <summary>
        /// Gets or sets the ImageSource property. This dependency property 
        /// indicates the image to display in a cascade.
        /// </summary>
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ImageSource property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnImageSourceChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (CropControl)d;
            ImageSource oldImageSource = (ImageSource)e.OldValue;
            ImageSource newImageSource = target.ImageSource;
            target.OnImageSourceChanged(oldImageSource, newImageSource);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the ImageSource property.
        /// </summary>
        /// <param name="oldImageSource">The old ImageSource value</param>
        /// <param name="newImageSource">The new ImageSource value</param>
        private void OnImageSourceChanged(
            ImageSource oldImageSource, ImageSource newImageSource)
        {
            Setup();
        }
        #endregion

        #region ImageOpened
        public event RoutedEventHandler ImageOpened;
        #endregion

        public CropControl()
        {
            this.DefaultStyleKey = typeof(CropControl);
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        private void ProcessManipulationDelta(ManipulationDelta delta, bool done)
        {
            _cropLeft = _cropLeftAtManipulationStart;
            _cropTop = _cropTopAtManipulationStart;
            _cropRight = _cropRightAtManipulationStart;
            _cropBottom = _cropBottomAtManipulationStart;

            //
            // Process scaling.  Must process even if currently 1.0, because it could have been different.
            //

            // Calculate current width and height
            double selectedWidth = _cropRight - _cropLeft;
            double selectedHeight = _cropBottom - _cropTop;

            // Calculate scaled width/height;
            double newWidth = selectedWidth * delta.Scale;
            double newHeight = selectedHeight * delta.Scale;

            // Constrain width
            if (newWidth > _image.Width)
            {
                newHeight *= _image.Width / newWidth;
                newWidth = _image.Width;
            }

            // Constrain height
            if (newHeight > _image.Height)
            {
                newWidth *= _image.Height / newHeight;
                newHeight = _image.Height;
            }

            // Move crop accordingly
            _cropLeft -= (newWidth - selectedWidth) / 2;
            _cropTop -= (newHeight - selectedHeight) / 2;
            _cropRight += (newWidth - selectedWidth) / 2;
            _cropBottom += (newHeight - selectedHeight) / 2;

            // Adjust left/right borders back into range, if necessary
            if (_cropLeft < 0)
            {
                _cropRight -= _cropLeft;
                _cropLeft = 0;
            }
            else if (_cropRight > _image.Width)
            {
                _cropLeft -= _image.Width - _cropRight;
                _cropRight = _image.Width;
            }

            // Adjust top/bottom borders back into range, if necessary
            if (_cropTop < 0)
            {
                _cropBottom -= _cropTop;
                _cropTop = 0;
            }
            else if (_cropBottom > _image.Height)
            {
                _cropTop -= _image.Height - _cropBottom;
                _cropBottom = _image.Height;
            }

            //
            // Process translation
            //

            // Skip dragging if _dragMode is none (scaling still applies, however)
            if (_dragMode != DragMode.None)
            {
                // Crop X translation
                if (_dragMode == DragMode.Full)
                {
                    if (delta.Translation.X < 0)
                    {
                        if (_cropLeft + delta.Translation.X < 0)
                        {
                            delta.Translation.X -= (_cropLeft + delta.Translation.X);
                        }
                    }
                    else
                    {
                        if (_cropRight + delta.Translation.X > _image.Width)
                        {
                            delta.Translation.X -= _cropRight + delta.Translation.X - _image.Width;
                        }
                    }

                    // Crop Y translation
                    if (delta.Translation.Y < 0)
                    {
                        if (_cropTop + delta.Translation.Y < 0)
                        {
                            delta.Translation.Y -= (_cropTop + delta.Translation.Y);
                        }
                    }
                    else
                    {
                        if (_cropBottom + delta.Translation.Y > _image.Height)
                        {
                            delta.Translation.Y -= _cropBottom + delta.Translation.Y - _image.Height;
                        }
                    }
                }

                // Get initial translation amounts
                double leftTranslation = delta.Translation.X;
                double topTranslation = delta.Translation.Y;
                double rightTranslation = delta.Translation.X;
                double bottomTranslation = delta.Translation.Y;

                // Limit translation amounts depending on mode
                switch (_dragMode)
                {
                    case DragMode.Left:
                        rightTranslation = 0;
                        topTranslation = bottomTranslation = IsFixedAspectRatio ? double.NaN : 0;
                        break;

                    case DragMode.TopLeft:
                        rightTranslation = bottomTranslation = 0;
                        if (IsFixedAspectRatio)
                        {
                            if (Math.Abs(topTranslation) > Math.Abs(leftTranslation))
                            {
                                leftTranslation = double.NaN;
                            }
                            else
                            {
                                topTranslation = double.NaN;
                            }
                        }
                        break;

                    case DragMode.Top:
                        bottomTranslation = 0;
                        leftTranslation = rightTranslation = IsFixedAspectRatio ? double.NaN : 0;
                        break;

                    case DragMode.TopRight:
                        leftTranslation = bottomTranslation = 0;
                        if (IsFixedAspectRatio)
                        {
                            if (Math.Abs(topTranslation) > Math.Abs(rightTranslation))
                            {
                                rightTranslation = double.NaN;
                            }
                            else
                            {
                                topTranslation = double.NaN;
                            }
                        }
                        break;

                    case DragMode.Right:
                        leftTranslation = 0;
                        topTranslation = bottomTranslation = IsFixedAspectRatio ? double.NaN : 0;
                        break;

                    case DragMode.BottomRight:
                        leftTranslation = topTranslation = 0;
                        if (IsFixedAspectRatio)
                        {
                            if (Math.Abs(bottomTranslation) > Math.Abs(rightTranslation))
                            {
                                rightTranslation = double.NaN;
                            }
                            else
                            {
                                bottomTranslation = double.NaN;
                            }
                        }
                        break;

                    case DragMode.Bottom:
                        topTranslation = 0;
                        leftTranslation = rightTranslation = IsFixedAspectRatio ? double.NaN : 0;
                        break;

                    case DragMode.BottomLeft:
                        rightTranslation = topTranslation = 0;
                        if (IsFixedAspectRatio)
                        {
                            if (Math.Abs(bottomTranslation) > Math.Abs(leftTranslation))
                            {
                                leftTranslation = double.NaN;
                            }
                            else
                            {
                                bottomTranslation = double.NaN;
                            }
                        }
                        break;
                }

                double minSize = Math.Max(_scalingFactor, 1);

                // One of these means we need to adjust the height to match
                // the aspect ratio.
                if (topTranslation.IsNaN() || bottomTranslation.IsNaN())
                {
                    newWidth = Math.Max(minSize, Math.Min(_cropRight + rightTranslation, _image.Width) - Math.Max(0, _cropLeft + leftTranslation));
                    newHeight = Math.Max(minSize, newWidth / DesiredAspectRatio);
                    double adjustment = _cropBottom - _cropTop - newHeight;
                    if (topTranslation.IsNaN())
                    {
                        if (bottomTranslation.IsNaN())
                        {
                            topTranslation = adjustment / 2;
                            bottomTranslation = -(adjustment / 2);
                        }
                        else
                        {
                            topTranslation = adjustment;
                        }
                    }
                    else
                    {
                        bottomTranslation = -adjustment;
                    }

                    topTranslation = Math.Max(topTranslation, -_cropTop);
                    bottomTranslation = Math.Min(bottomTranslation, _image.Height - _cropBottom);
                    newHeight = _cropBottom - _cropTop - topTranslation + bottomTranslation;
                    double desiredWidth = newHeight * DesiredAspectRatio;
                    adjustment = newWidth - desiredWidth;
                    if (leftTranslation != 0)
                    {
                        leftTranslation += adjustment;
                    }
                    else
                    {
                        rightTranslation -= adjustment;
                    }
                }

                // One of these means we need to adjust the width to match
                // the aspect ratio.
                if (leftTranslation.IsNaN() || rightTranslation.IsNaN())
                {
                    newHeight = Math.Max(minSize, Math.Min(_cropBottom + bottomTranslation, _image.Height) - Math.Max(0, _cropTop + topTranslation));
                    newWidth = Math.Max(minSize, newHeight * DesiredAspectRatio);
                    double adjustment = _cropRight - _cropLeft - newWidth;
                    if (leftTranslation.IsNaN())
                    {
                        if (rightTranslation.IsNaN())
                        {
                            leftTranslation = adjustment / 2;
                            rightTranslation = -(adjustment / 2);
                        }
                        else
                        {
                            leftTranslation = adjustment;
                        }
                    }
                    else
                    {
                        rightTranslation = -adjustment;
                    }

                    leftTranslation = Math.Max(leftTranslation, -_cropLeft);
                    rightTranslation = Math.Min(rightTranslation, _image.Width - _cropRight);
                    newWidth = _cropRight - _cropLeft - leftTranslation + rightTranslation;
                    double desiredHeight = newWidth / DesiredAspectRatio;
                    adjustment = newHeight - desiredHeight;
                    if (topTranslation != 0)
                    {
                        topTranslation += adjustment;
                    }
                    else
                    {
                        bottomTranslation -= adjustment;
                    }
                }

                // Apply X and Y translations, limiting minsize
                // TODO: round to pixel amounts!
                _cropLeft = Math.Max(0, _cropLeft + leftTranslation);
                _cropTop = Math.Max(0, _cropTop + topTranslation);
                _cropRight = Math.Min(_image.Width, _cropRight + rightTranslation);
                _cropBottom = Math.Min(_image.Height, _cropBottom + bottomTranslation);

                if (leftTranslation != 0)
                {
                    _cropLeft = Math.Floor(Math.Min(_cropRight - minSize, _cropLeft));
                }

                if (topTranslation != 0)
                {
                    _cropTop = Math.Floor(Math.Min(_cropBottom - minSize, _cropTop));
                }

                if (rightTranslation != 0)
                {
                    _cropRight = Math.Ceiling(Math.Max(_cropLeft + minSize, _cropRight));
                }

                if (bottomTranslation != 0)
                {
                    _cropBottom = Math.Ceiling(Math.Max(_cropTop + minSize, _cropBottom));
                }
            }

            _isManipulating = !done;

            SetCropProperties();

            AdjustLayout();
        }

        private void SetCropProperties()
        {
            CropLeft = (int)(_cropLeft / _scalingFactor);
            CropTop = (int)(_cropTop / _scalingFactor);

            // Internally, we keep _cropRight and _cropBottom as +1 beyond where the crop should be
            // which is pretty necessary because of scaling.  We don't want to expose that back
            // to our customers, however.
            CropRight = CropLeft + (int)Math.Ceiling((_cropRight - _cropLeft) / _scalingFactor) - 1;
            CropBottom = CropTop + (int)Math.Ceiling((_cropBottom - _cropTop) / _scalingFactor) - 1;

            // Now, here, we need to add the one back in
            CropWidth = CropRight - CropLeft + 1;
            CropHeight = CropBottom - CropTop + 1;

            // And compute our actual aspect ratio
            ActualAspectRatio = (_cropRight - _cropLeft) / (_cropBottom - _cropTop);
        }

        private T GetTemplateChild<T>(string name) where T : class
        {
            T item = GetTemplateChild(name) as T;

            if (item == null)
            {
                throw new InvalidOperationException("CropControl requires an " + typeof(T) + " called " + name + " in its template.");
            }

            return item;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _canvas = GetTemplateChild<Canvas>(CanvasName);
            _image = GetTemplateChild<Image>(ImageName);

            // Add manipulation event handlers
            _canvas.ManipulationMode =
                ManipulationModes.Scale | ManipulationModes.ScaleInertia |
                ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
            _canvas.ManipulationStarting += OnManipulationStarting;
            _canvas.ManipulationStarted += OnManipulationStarted;
            _canvas.ManipulationDelta += OnManipulationDelta;
            _canvas.ManipulationCompleted += OnManipulationCompleted;
            _canvas.ManipulationInertiaStarting += OnManipulationInertiaStarting;

            // CAUTION: this needs to not happen in design mode, as it causes a problem there
            // Window.Current.CoreWindow.PointerWheelChanged += OnPointerWheelChanged;

            FrameworkElementExtensions.SetCursor(this, new CoreCursor(CoreCursorType.Arrow, 0));
            _canvas.PointerMoved += OnPointerMoved;

            _cropLeft = _cropRight = _cropTop = _cropBottom = 50;

            // Set image to a zero size until it's properly loaded
            // This handily avoids some crashes, and some visual issues
            SetWidthHeight(_image, 0, 0);

            _isTemplateApplied = true;

            Setup();
        }

        private void OnPointerWheelChanged(CoreWindow sender, PointerEventArgs e)
        {
            Debug.WriteLine("!!!! wheel delta: " + e.CurrentPoint.Properties.MouseWheelDelta);
            e.Handled = true;
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isImageLoaded || _isManipulating)
            {
                return;
            }

            Point position = e.GetCurrentPoint(this).Position;
            position = SetCursorForPosition(position);
        }

        private Point SetCursorForPosition(Point position)
        {
            DragMode dragMode = CalculateDragMode(position);

            CoreCursorType cursorType;
            if (dragMode == DragMode.Full)
            {
                cursorType = CoreCursorType.Hand;
            }
            else if (dragMode == DragMode.TopLeft || dragMode == DragMode.BottomRight)
            {
                cursorType = CoreCursorType.SizeNorthwestSoutheast;
            }
            else if (dragMode == DragMode.TopRight || dragMode == DragMode.BottomLeft)
            {
                cursorType = CoreCursorType.SizeNortheastSouthwest;
            }
            else if (dragMode == DragMode.Top || dragMode == DragMode.Bottom)
            {
                cursorType = CoreCursorType.SizeNorthSouth;
            }
            else if (dragMode == DragMode.Left || dragMode == DragMode.Right)
            {
                cursorType = CoreCursorType.SizeWestEast;
            }
            else
            {
                cursorType = CoreCursorType.Arrow;
            }

            FrameworkElementExtensions.SetCursor(this, new CoreCursor(cursorType, (uint)cursorType));
            return position;
        }

        private DragMode CalculateDragMode(Point point)
        {
            DragMode mode;

            var _topLeftTransform = this.TransformToVisual(_topLeftDragControl);
            var _bottomRightTransform = this.TransformToVisual(_bottomRightDragControl);
            double dragControlSize = _topLeftDragControl.ActualWidth;
            var pointInTopLeft = _topLeftTransform.TransformPoint(point);
            var pointInBottomRight = _bottomRightTransform.TransformPoint(point);

            // Check for outside
            if (pointInTopLeft.X < 0 || pointInBottomRight.X > dragControlSize
                || pointInTopLeft.Y < 0 || pointInBottomRight.Y > dragControlSize)
            {
                mode = DragMode.None;
            }

            // Check for top possibilities
            else if (pointInTopLeft.Y <= dragControlSize)
            {
                if (pointInTopLeft.X <= dragControlSize)
                {
                    mode = DragMode.TopLeft;
                }
                else
                {
                    mode = pointInBottomRight.X < 0 ? DragMode.Top : DragMode.TopRight;
                }
            }

            // Check for bottom possibilities
            else if (pointInBottomRight.Y >= 0)
            {
                if (pointInTopLeft.X <= dragControlSize)
                {
                    mode = DragMode.BottomLeft;
                }
                else
                {
                    mode = pointInBottomRight.X < 0 ? DragMode.Bottom : DragMode.BottomRight;
                }
            }

            // Check for right
            else if (pointInBottomRight.X >= 0)
            {
                mode = DragMode.Right;
            }

            // Check for left
            else if (pointInTopLeft.X <= dragControlSize)
            {
                mode = DragMode.Left;
            }

            // Guess that means the middle!
            else
            {
                mode = DragMode.Full;
            }

            return mode;
        }

        private void OnManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            if (!_isImageLoaded)
            {
                return;
            }

            _cropLeftAtManipulationStart = _cropLeft;
            _cropTopAtManipulationStart = _cropTop;
            _cropRightAtManipulationStart = _cropRight;
            _cropBottomAtManipulationStart = _cropBottom;
            e.Handled = true;
        }

        private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (!_isImageLoaded)
            {
                return;
            }

            _dragMode = CalculateDragMode(e.Position);
            ProcessManipulationDelta(e.Cumulative, false);
            e.Handled = true;
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (!_isImageLoaded)
            {
                return;
            }

            ProcessManipulationDelta(e.Cumulative, false);
            e.Handled = true;
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (!_isImageLoaded)
            {
                return;
            }

            ProcessManipulationDelta(e.Cumulative, true);
            SetCursorForPosition(e.Position);
            e.Handled = true;
        }

        private void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
            if (!_isImageLoaded)
            {
                return;
            }

            ProcessManipulationDelta(e.Cumulative, false);
            e.Handled = true;
        }

        private static void OnDesiredAspectRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CropControl crop = d as CropControl;
            if (crop != null)
            {
                crop.SetCropForDesiredAspectRatio();
            }
        }

        private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CropControl crop = d as CropControl;
            if (crop != null)
            {
                crop.DoFullLayout();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
            Setup();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_isTemplateApplied)
            {
                //Window.Current.CoreWindow.PointerWheelChanged -= OnPointerWheelChanged;
            }
        }

        private void SetWidthHeight(FrameworkElement element, double width, double height)
        {
            element.Width = width;
            element.Height = height;
        }

        private void SetCanvasXYWithOffset(UIElement element, double x, double y)
        {
            Canvas.SetLeft(element, x + _leftImageOffset);
            Canvas.SetTop(element, y + _topImageOffset);
        }

        private void SetPositionWithOffset(Line line, double x1, double y1, double x2, double y2)
        {
            line.X1 = x1 + _leftImageOffset;
            line.Y1 = y1 + _topImageOffset;

            if (x2.IsNaN())
            {
                line.X2 = line.X1;
            }
            else
            {
                line.X2 = Math.Max(line.X1, x2 + _leftImageOffset);
            }

            if (y2.IsNaN())
            {
                line.Y2 = line.Y1;
            }
            else
            {
                line.Y2 = Math.Max(line.Y1, y2 + _topImageOffset);
            }
        }


        private void AdjustLayout()
        {
            if (!_isImageLoaded)
            {
                return;
            }

            double halfDragControl = DragControlSize / 2;

            // Move drag controls
            SetCanvasXYWithOffset(_topLeftDragControl, _cropLeft - halfDragControl, _cropTop - halfDragControl);
            SetCanvasXYWithOffset(_topRightDragControl, _cropRight - halfDragControl, _cropTop - halfDragControl);
            SetCanvasXYWithOffset(_bottomLeftDragControl, _cropLeft - halfDragControl, _cropBottom - halfDragControl);
            SetCanvasXYWithOffset(_bottomRightDragControl, _cropRight - halfDragControl, _cropBottom - halfDragControl);

            // Move outside lines
            SetPositionWithOffset(
                _topLine,
                _cropLeft + halfDragControl, _cropTop,
                _cropRight - halfDragControl, Double.NaN);
            SetPositionWithOffset(
                _leftLine,
                _cropLeft, _cropTop + halfDragControl,
                Double.NaN, _cropBottom - halfDragControl);
            SetPositionWithOffset(
                _bottomLine,
                _cropLeft + halfDragControl, _cropBottom,
                _cropRight - halfDragControl, Double.NaN);
            SetPositionWithOffset(
                _rightLine,
                _cropRight, _cropTop + halfDragControl,
                Double.NaN, _cropBottom - halfDragControl);

            // Move inside lines
            SetPositionWithOffset(
                _topMiddleLine,
                _cropLeft, _cropTop + (_cropBottom - _cropTop) / 3,
                _cropRight, Double.NaN);
            SetPositionWithOffset(
                _leftMiddleLine,
                _cropLeft + (_cropRight - _cropLeft) / 3, _cropTop,
                Double.NaN, _cropBottom);
            SetPositionWithOffset(
                _bottomMiddleLine,
                _cropLeft, _cropBottom - (_cropBottom - _cropTop) / 3,
                _cropRight, Double.NaN);
            SetPositionWithOffset(
                _rightMiddleLine,
                _cropRight - (_cropRight - _cropLeft) / 3, _cropTop,
                Double.NaN, _cropBottom);

            // Adjust the masking rectangles
            _leftMaskingRectangle.Width = _cropLeft;
            Canvas.SetLeft(_topMaskingRectangle, _cropLeft + _leftImageOffset);
            SetWidthHeight(_topMaskingRectangle, _cropRight - _cropLeft, _cropTop);
            _rightMaskingRectangle.Width = _image.Width - _cropRight;
            Canvas.SetLeft(_rightMaskingRectangle, _cropRight + _leftImageOffset);
            SetCanvasXYWithOffset(_bottomMaskingRectangle, _cropLeft, _cropBottom);
            SetWidthHeight(_bottomMaskingRectangle, _cropRight - _cropLeft, _image.Height - _cropBottom);
        }

        public void Setup()
        {
            // Check for loaded and template succesfully applied
            if (!_isLoaded ||
                !_isTemplateApplied)
            {
                return;
            }

            _image.Source = ImageSource;
            var bi = _image.Source as BitmapImage;
            if (bi != null)
            {
                bi.ImageOpened += (sender, e) =>
                    {
                        DoFullLayout();
                        if (this.ImageOpened != null)
                        {
                            this.ImageOpened(this, e);
                        }
                    };
            }
        }

        private void DoFullLayout()
        {
            if (!_isTemplateApplied)
            {
                return;
            }

            var bi = _image.Source as BitmapImage;
            if (bi == null)
            {
                return;
            }

            // Create UI controls
            MaybeCreateMaskingRectangle(ref _leftMaskingRectangle);
            MaybeCreateMaskingRectangle(ref _topMaskingRectangle);
            MaybeCreateMaskingRectangle(ref _rightMaskingRectangle);
            MaybeCreateMaskingRectangle(ref _bottomMaskingRectangle);
            MaybeCreateDragControl(ref _topLeftDragControl);
            MaybeCreateDragControl(ref _topRightDragControl);
            MaybeCreateDragControl(ref _bottomLeftDragControl);
            MaybeCreateDragControl(ref _bottomRightDragControl);
            MaybeCreateLine(ref _leftLine, OutsideLineThickness);
            MaybeCreateLine(ref _topLine, OutsideLineThickness);
            MaybeCreateLine(ref _rightLine, OutsideLineThickness);
            MaybeCreateLine(ref _bottomLine, OutsideLineThickness);
            MaybeCreateLine(ref _leftMiddleLine, InsideLineThickness);
            MaybeCreateLine(ref _topMiddleLine, InsideLineThickness);
            MaybeCreateLine(ref _rightMiddleLine, InsideLineThickness);
            MaybeCreateLine(ref _bottomMiddleLine, InsideLineThickness);

            double scalingX = (_canvas.ActualWidth - DragControlSize) / bi.PixelWidth;
            double scalingY = (_canvas.ActualHeight - DragControlSize) / bi.PixelHeight;

            _scalingFactor = Math.Min(scalingX, scalingY);

            // Calculate desired dimensions and necessary image offset
            double desiredWidth = Math.Floor(bi.PixelWidth * _scalingFactor);
            double desiredHeight = Math.Floor(bi.PixelHeight * _scalingFactor);
            _leftImageOffset = (_canvas.ActualWidth - desiredWidth) / 2;
            _topImageOffset = (_canvas.ActualHeight - desiredHeight) / 2;

            // Set public values
            OriginalWidth = (int)bi.PixelWidth;
            OriginalHeight = (int)bi.PixelHeight;

            // Move and size image
            SetCanvasXYWithOffset(_image, 0, 0);
            SetWidthHeight(_image, desiredWidth, desiredHeight);

            // Set initial crop amounts
            SetCropForDesiredAspectRatio();
            SetCropProperties();

            // Size the lines
            SetWidthHeight(_leftLine, _canvas.ActualWidth, _canvas.ActualHeight);
            SetWidthHeight(_topLine, _canvas.ActualWidth, _canvas.ActualHeight);
            SetWidthHeight(_rightLine, _canvas.ActualWidth, _canvas.ActualHeight);
            SetWidthHeight(_bottomLine, _canvas.ActualWidth, _canvas.ActualHeight);
            SetWidthHeight(_leftMiddleLine, _canvas.ActualWidth, _canvas.ActualHeight);
            SetWidthHeight(_topMiddleLine, _canvas.ActualWidth, _canvas.ActualHeight);
            SetWidthHeight(_rightMiddleLine, _canvas.ActualWidth, _canvas.ActualHeight);
            SetWidthHeight(_bottomMiddleLine, _canvas.ActualWidth, _canvas.ActualHeight);

            // Set the parts of the masking rectangles that remain in place
            SetCanvasXYWithOffset(_leftMaskingRectangle, 0, 0);
            _leftMaskingRectangle.Height = _image.Height;
            Canvas.SetTop(_topMaskingRectangle, _topImageOffset);
            Canvas.SetTop(_rightMaskingRectangle, _topImageOffset);
            _rightMaskingRectangle.Height = _image.Height;

            _isImageLoaded = true;

            // Adjust the parts of the layout that adjust on crop changes
            AdjustLayout();
        }

        private void SetCropForDesiredAspectRatio()
        {
            // Do nothing if the template isn't applied, or the image isn't loaded.
            // Note that we can't use _isImageLoaded here because that's set after
            // this is called during the loading process.
            if (!_isTemplateApplied || _image.Width == 0)
            {
                return;
            }

            _cropLeft = _cropTop = 0;

            if (IsFixedAspectRatio)
            {
                double imageAspectRatio = _image.Width / _image.Height;

                if (DesiredAspectRatio <= 0)
                {
                    DesiredAspectRatio = imageAspectRatio;
                }
                else if (DesiredAspectRatio > imageAspectRatio)
                {
                    // Handle case of desired aspect ratio being wider than image
                    double desiredCropHeight = _image.Width / DesiredAspectRatio;
                    _cropTop = (_image.Height - desiredCropHeight) / 2;
                }
                else
                {
                    // Handle case of desired aspect ratio being taller than image
                    double desiredCropWidth = _image.Height * DesiredAspectRatio;
                    _cropLeft = (_image.Width - desiredCropWidth) / 2;
                }
            }

            _cropRight = _image.Width - _cropLeft;
            _cropBottom = _image.Height - _cropTop;
        }

        private Rectangle MaybeCreateMaskingRectangle(ref Rectangle rectangle)
        {
            if (rectangle == null)
            {
                rectangle = new Rectangle();
                _canvas.Children.Add(rectangle);
            }

            //// rectangle.Fill = new SolidColorBrush(new Color { A = 0x44 });
            rectangle.Fill = MaskingBrush;

            return rectangle;
        }

        private Shape MaybeCreateDragControl(ref Shape dragControl)
        {
            if (dragControl == null)
            {
                dragControl = new Ellipse();
                _canvas.Children.Add(dragControl);
            }

            SetWidthHeight(dragControl, DragControlSize, DragControlSize);
            dragControl.Stroke = this.Foreground;
            dragControl.StrokeThickness = DragControlStrokeThickness;

            return dragControl;
        }

        private Line MaybeCreateLine(ref Line line, double thickness)
        {
            if (line == null)
            {
                line = new Line();
                _canvas.Children.Add(line);
            }

            line.Stroke = this.Foreground;
            line.StrokeThickness = thickness;

            return line;
        }
    }
}
