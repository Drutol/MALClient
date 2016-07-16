using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MalClient.Shared.UserControls
{
    public sealed partial class StackedBarChartControl : UserControl
    {
        private static readonly Brush B2 =
            new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Dark
                ? Color.FromArgb(255, 88, 88, 88)
                : Colors.Gainsboro);

        private static readonly List<Color> ColorsOrder = new List<Color>
        {
            Colors.ForestGreen,
            Colors.DodgerBlue,
            Colors.Gold,
            Colors.Crimson,
            Colors.Gray
        };

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register(
                "DataSource",
                typeof(List<int>),
                typeof(StackedBarChartControl),
                new PropertyMetadata(
                    new List<int>(), PropertyChangedCallback));

        private readonly int _lineThickness = 30;

        private bool _loaded;

        public StackedBarChartControl()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                _loaded = true;
                PopulateData();
            };
        }

        public List<int> DataSource
        {
            get { return (List<int>) GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var chart = dependencyObject as StackedBarChartControl;
            if (chart._loaded)
                chart.PopulateData();
        }

        private async void PopulateData()
        {
            if (DataSource == null)
                return;
            ChartCanvas.Children.Clear();
            await Task.Delay(50); // raceeee... wrooom!
            var margin = 10;
            var values = DataSource;
            if (values.Count == 0)
                return;
            var nonZeroValuesCount = values.Count(i => i != 0);
            var maxWidth = ChartCanvas.ActualWidth - _lineThickness*3/4*nonZeroValuesCount - margin*2;
            var height = 0; //(ChartCanvas.ActualHeight / 2) ;
            var all = values.Aggregate(0.0f, (current, value) => current + value);
            double currX = _lineThickness/2 + margin/2, offset;
            var currColor = 0;
            var labels = new Dictionary<double, float>();
            float totalPercentage = 0;
            float percentage;

            ChartCanvas.Children.Add(new Line
            {
                X1 = currX,
                X2 = ChartCanvas.ActualWidth - margin*2,
                Y1 = height,
                Y2 = height,
                Stroke = B2,
                Fill = B2,
                StrokeThickness = 40,
                StrokeDashCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            });

            foreach (var value in values)
            {
                if (value == 0)
                {
                    currColor++;
                    continue;
                }
                percentage = value*100/all;
                totalPercentage += percentage;
                offset = percentage*maxWidth/100;
                labels.Add(currX + offset, totalPercentage);
                var line = new Line
                {
                    X1 = currX,
                    X2 = currX + offset,
                    Y1 = height,
                    Y2 = height,
                    Stroke = new SolidColorBrush(ColorsOrder[currColor]),
                    Fill = B2,
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
                if ((int) Math.Ceiling(label.Value) == 100)
                    break;
                var txt = new TextBlock
                {
                    Text = Math.Floor(label.Value) + "%",
                    TextAlignment = TextAlignment.Center
                };
                var lblHeight = height + 30;

                if (prevLabel == null || Math.Abs((int) label.Key - (int) prevLabel) > 30)
                    prevLabel = (int) label.Key;
                else
                    lblHeight -= 80;
                txt.SetValue(Canvas.TopProperty, lblHeight);
                txt.SetValue(Canvas.LeftProperty, label.Key);
                ChartCanvas.Children.Add(txt);
            }
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