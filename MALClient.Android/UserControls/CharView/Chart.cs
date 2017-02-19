using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using MALClient.XShared.Utils;
using Android.Animation;

namespace MALClient.Android.UserControls
{
    public partial class Chart
    {
        private event EventHandler<MotionEvent> OnInnerCircleTouch;
        private event EventHandler<MotionEvent> OnOutterCircleTouch;
        private List<Arc> ArcsList = new List<Arc>();


        private float _sum;
        private float Sum
        {
            get { return _sum; }
            set
            {
                _sum = value;
                foreach (var arc in ArcsList) arc.RescaleToSum(value);
                OnChartUpdated?.Invoke(null, null);
            }
        }

        private int _selectedArc = -1;

        private Circle _innerCircle = new Circle(0, 0, 200);
        public float InnerCircleRadius
        {
            get { return _innerCircle.Radius; }
            set
            {
                _innerCircle.Radius = value;
                calculateStrokeWidth(value, OutterCircleRadius);
                calculateDrawingRadius(value, OutterCircleRadius);
            }
        }

        private Circle _outterCircle = new Circle(0, 0, 400);
        public float OutterCircleRadius
        {
            get { return _outterCircle.Radius; }
            set
            {
                _outterCircle.Radius = value;
                calculateStrokeWidth(value, OutterCircleRadius);
                calculateDrawingRadius(value, OutterCircleRadius);
            }
        }
        public Vector2D Position { get; set; } = new Vector2D(200, 400);
        public event EventHandler OnChartUpdated;

        public Chart(ChartView view)
        {
            InnerCircleRadius = 200;
            OutterCircleRadius = 400;
            view.OnViewInitialized += (sender, args) => InitAnimationStart();
            view.OnTouch += (sender, args) => checkTouch(args);
            Init();
        }

        private void Init()
        {
            
        }
        
        public void Draw(Canvas canvas)
        {
            canvas.Save();
            canvas.Translate(Position.X, Position.Y);
            foreach(var arc in ArcsList)
            {
                arc.Draw(canvas);
                canvas.Rotate((arc.CurrentValue * arc.LengthFraction / Sum) * 360.0f);
            }
            canvas.Restore();
        }

        public void Add(float value, Color color)
        {
            var strokeWidth = calculateStrokeWidth(InnerCircleRadius, OutterCircleRadius);
            var drawingRadius = calculateDrawingRadius(InnerCircleRadius, OutterCircleRadius);
            Arc temp = new Arc(drawingRadius, strokeWidth, color);
            temp.OnValueChanged += (sender, args) => updateSum();
            temp.OnValueSet += (arc, toValue) => SetAnimation(arc as Arc, toValue);
            ArcsList.Add(temp);
            temp.Value = value;
        }

        private void SetAnimation(Arc arc, float end)
        {
            ValueAnimator animator = ValueAnimator.OfFloat(new float[] { (arc as Arc).CurrentValue, end });
            animator.SetDuration(1000);
            animator.Update += (sender, args) =>
            {
                (arc as Arc).CurrentValue = (float)animator.AnimatedValue;
            };
            animator.Start();
        }

        private void SetInstant(Arc arc, float end)
        {
            arc.CurrentValue = end;
        }

        public void InitAnimationStart()
        {
            foreach(var arc in ArcsList)
            {
                ValueAnimator animator = ValueAnimator.OfFloat(new float[] { 0, 1 });
                animator.SetDuration(1000);
                animator.Update += (sender, args) =>
                {
                    arc.LengthFraction = (float)animator.AnimatedValue;
                };
                animator.Start();
            }
        }

        private void updateSum()
        {
            Sum = ArcsList.Select(val => val.CurrentValue).Sum(); //LINQ! :D Sam zrobi³em!
        }

        private float calculateStrokeWidth(float innerRadius, float outterRadius)
        {
            return outterRadius - innerRadius;
        }

        private float calculateDrawingRadius(float innerRadius, float outterRadius)
        {
            return calculateStrokeWidth(innerRadius, outterRadius) / 2.0f + innerRadius;
        }

        //DEBUG
        public void incVal()
        {
            ArcsList[0].Value += 10;
        }
        //-----//

        private void checkTouch(MotionEvent args)
        {
            float X = args.GetX();
            float Y = args.GetY();
            Vector2D relativePosition = new Vector2D(X - Position.X, Y - Position.Y);

            if (!_outterCircle.CheckCollision( new XShared.Utils.Point(relativePosition) ) ) return;

            if (_innerCircle.CheckCollision(new XShared.Utils.Point(relativePosition)))
                OnInnerCircleTouch?.Invoke(this, args);
            else
                OnOutterCircleTouch?.Invoke(this, args);
        }
    }
}