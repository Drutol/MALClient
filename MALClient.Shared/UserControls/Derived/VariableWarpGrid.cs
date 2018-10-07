using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using WinRTXamlToolkit.Controls;

namespace MALClient.UWP.Shared.UserControls.Derived
{
    /// <summary>
    ///     This panel will arrange items of various sizes
    ///     You can also sqeeze children so there's less empty space.
    /// </summary>
    public class TrueVariableWarpGrid : WrapPanel
    {
        // Using a DependencyProperty as the backing store for LockToggle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SqeezeChildrenProperty =
            DependencyProperty.Register("SqeezeChildren", typeof(bool), typeof(TrueVariableWarpGrid),
                new PropertyMetadata(false));

        public bool SqeezeChildren
        {
            get { return (bool)GetValue(SqeezeChildrenProperty); }
            set { SetValue(SqeezeChildrenProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return ArrangeInternal(availableSize, false);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return ArrangeInternal(finalSize);
        }

        private Size ArrangeInternal(Size size, bool callArrange = true)
        {
            // measure max desired item size
            foreach (var item in Children.Where(child => child.Visibility == Visibility.Visible))
                item.Measure(size);

            double x = 0.0, y = 0.0;
            var currentRow = 0;
            //we will store info about each row here
            var processedRows = new List<RowInfo> { new RowInfo() }; //1st row empty
            // position each child and get total content size
            foreach (var item in Children.Where(child => child.Visibility == Visibility.Visible))
            {
                if (x + item.DesiredSize.Width > size.Width)
                {
                    //if squeezing is enabled
                    //first we check if there's any place left in different rows
                    if (SqeezeChildren && processedRows.Count > 1)
                    {
                        //if row has enough space and item is not higher than highest in row
                        var candidate =
                            processedRows.FirstOrDefault(
                                info =>
                                    info.LeftWidth >= item.DesiredSize.Width &&
                                    item.DesiredSize.Height <= info.MaxHeight);
                        if (candidate != default(RowInfo) && candidate != processedRows[currentRow])
                        {
                            //we have row with some left space
                            item.Arrange(new Rect(candidate.TotalWidth, candidate.TopY, item.DesiredSize.Width,
                                item.DesiredSize.Height));
                            candidate.TotalWidth += item.DesiredSize.Width;
                            candidate.LeftWidth -= item.DesiredSize.Width;
                            //we won't do anything down there ninja continue
                            continue;
                        }
                    }
                    // if item won't fit, move to the next row                    
                    y = processedRows.Sum(info => info.MaxHeight);
                    processedRows[currentRow].LeftWidth = size.Width - x;
                    processedRows[currentRow].TotalWidth = x;
                    processedRows.Add(new RowInfo()); //we create new row
                    currentRow++;
                    processedRows[currentRow].TopY = y;
                    x = 0;
                }

                // implicitly, item fits in current row

                processedRows[currentRow].MaxHeight = Math.Max(processedRows[currentRow].MaxHeight,
                    item.DesiredSize.Height);
                if (callArrange)
                    item.Arrange(new Rect(x, y, item.DesiredSize.Width, item.DesiredSize.Height));
                x += item.DesiredSize.Width;
            }

            var osize = new Size(size.Width, processedRows.Sum(info => info.MaxHeight));
            return osize;
        }

        private class RowInfo
        {
            public double LeftWidth { get; set; }
            public double TotalWidth { get; set; }
            public double MaxHeight { get; set; }
            public double TopY { get; set; }
        }
    }
}