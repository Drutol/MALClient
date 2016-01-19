using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UserControls
{
    public sealed partial class StackedBarChartControl : UserControl
    {
        private readonly List<Color>  _colorsOrder = new List<Color> {Colors.ForestGreen , Colors.DodgerBlue , Colors.Gold , Colors.Crimson , Colors.Gray };
        private readonly int _lineThickness = 30;


        public StackedBarChartControl()
        {
            this.InitializeComponent();
        }

        public void PopulateData(List<int> values,int margin = 10)
        {
            int nonZeroValuesCount = values.Count(i => i != 0);
            var maxWidth = (ChartCanvas.ActualWidth - (_lineThickness*3/4)*nonZeroValuesCount) - margin*2;
            var height = 0;//(ChartCanvas.ActualHeight / 2) ;
            float all = values.Aggregate(0.0f, (current, value) => current + value);
            double currX = _lineThickness / 2 + margin/2, offset;
            int currColor = 0;
            Dictionary<double,float> labels = new Dictionary<double, float>();
            float totalPercentage = 0;
            float percentage;

            var txt0 = new TextBlock
            {
                Text = "0%",
                TextAlignment = TextAlignment.Center,
            };
            txt0.SetValue(Canvas.TopProperty, height + 30);
            txt0.SetValue(Canvas.LeftProperty, currX/2);
            ChartCanvas.Children.Add(txt0);

            ChartCanvas.Children.Add(new Line
            {
                X1 = currX,
                X2 = ChartCanvas.ActualWidth - margin*2,
                Y1 = height,
                Y2 = height,
                Stroke = new SolidColorBrush(Colors.Gainsboro),
                Fill = new SolidColorBrush(Colors.Gainsboro),
                StrokeThickness = 40,
                StrokeDashCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round

            });

            foreach (var value in values)
            {
                if(value == 0)
                    continue;
                percentage = value * 100 / all;
                totalPercentage += percentage;
                offset = percentage*maxWidth/100;
                labels.Add(currX + offset, totalPercentage);
                var line = new Line
                {
                    X1 = currX,
                    X2 = currX + offset,
                    Y1= height,
                    Y2= height,
                    Stroke = new SolidColorBrush(_colorsOrder[currColor]),
                    Fill = new SolidColorBrush(Colors.Gainsboro),
                    StrokeThickness = _lineThickness,
                    StrokeDashCap = PenLineCap.Round,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round                   
                };
                line.PointerEntered += LineOnPointerEntered;
                line.PointerExited += LineOnPointerExited;
                ChartCanvas.Children.Add(line);
                currX += offset + _lineThickness*3/4;
                currColor++;
            }

            int? prevLabel = null;
            foreach (var label in labels)
            {
                if ((int)Math.Ceiling(label.Value) == 100)
                    break;
                var txt = new TextBlock
                {
                    Text = Math.Ceiling(label.Value) + "%",
                    TextAlignment = TextAlignment.Center,
                };
                int lblHeight = height + 30;

                if (prevLabel == null || Math.Abs((int) label.Key - (int) prevLabel) > 30)
                    prevLabel = (int) label.Key;
                else
                    lblHeight -= 80;
                txt.SetValue(Canvas.TopProperty,lblHeight);
                txt.SetValue(Canvas.LeftProperty, label.Key);
                ChartCanvas.Children.Add(txt);
            }

            var txt100 = new TextBlock
            {
                Text = "100%",
                TextAlignment = TextAlignment.Center,
            };
            txt100.SetValue(Canvas.TopProperty, height + 30);
            txt100.SetValue(Canvas.LeftProperty, ChartCanvas.ActualWidth-40);
            ChartCanvas.Children.Add(txt100);


        }

        private void LineOnPointerExited(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            var line = sender as Line;
            line.StrokeThickness = _lineThickness;
        }

        private void LineOnPointerEntered(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            var line = sender as Line;
            line.StrokeThickness = _lineThickness + 5;
        }
    }
}
