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
        private event EventHandler< Tuple<int, int> > SegmentSelected;
        private event EventHandler OnLoaded;
        private List<Arc> ArcsList = new List<Arc>();
        public float SelectedTargetAngle { get; set; }

        private float _angle = 0;
        public float Angle
        {
            get => _angle;
            set => _angle = (value + 360.0f) % 360.0f;
        }

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
        public int SelectedArc
        {
            get { return _selectedArc; }
            set
            {
                if (_selectedArc == value) return;
                SegmentSelected?.Invoke(this, new Tuple<int, int>(_selectedArc, value));
                _selectedArc = value;
            }
        }
        private Circle _innerCircle = new Circle(0, 0, 200);
        public float InnerCircleRadius
        {
            get { return _innerCircle.Radius; }
            set
            {
                _innerCircle.Radius = value;
                CalculateStrokeWidth(value, OutterCircleRadius);
                CalculateDrawingRadius(value, OutterCircleRadius);
            }
        }

        private Circle _outterCircle = new Circle(0, 0, 400);
        public float OutterCircleRadius
        {
            get { return _outterCircle.Radius; }
            set
            {
                _outterCircle.Radius = value;
                CalculateStrokeWidth(value, OutterCircleRadius);
                CalculateDrawingRadius(value, OutterCircleRadius);
            }
        }
        public Vector2D Position { get; set; } = new Vector2D(200, 400);
        public event EventHandler OnChartUpdated;

        public Chart(ChartView view)
        {
            InnerCircleRadius = 200;
            OutterCircleRadius = 400;
            view.OnViewInitialized += (sender, args) =>
            {
                Position = new Vector2D(view.MeasuredWidth / 2.0f, view.MeasuredHeight / 2.0f);
            };
            view.OnViewInitialized += (sender, args) => InitAnimationStart();
            OnLoaded += (sender, args) =>
            {
                view.OnTouch += (ob, action) => CheckTouch(action);
            };
            SegmentSelected += (sender, args) => ShrinkStrokeWidth(args.Item1);
            SegmentSelected += (sender, args) => EnlargeStrokeWidth(args.Item2);
            SegmentSelected += (sender, args) => TurnToSelectedSegment(args.Item2);
            SelectedTargetAngle = 90;
            Init();
        }

        private void Init()
        {
            
        }
        
        public void Draw(Canvas canvas)
        {
            canvas.Save();
                canvas.Translate(Position.X, Position.Y);
                canvas.Rotate(Angle);
                foreach(var arc in ArcsList)
                {
                    arc.Draw(canvas);
                    canvas.Rotate((arc.CurrentValue * arc.LengthFraction / Sum) * 360.0f);
                }
            canvas.Restore();
        }

        public void Add(float value, Color color)
        {
            var strokeWidth = CalculateStrokeWidth(InnerCircleRadius, OutterCircleRadius);
            var drawingRadius = CalculateDrawingRadius(InnerCircleRadius, OutterCircleRadius);
            Arc temp = new Arc(drawingRadius, strokeWidth, color);
            temp.OnValueChanged += (sender, args) => updateSum();
            temp.OnValueSet += (arc, toValue) => SetAnimation(arc as Arc, toValue);
            temp.OnNeedRefresh += (sender, args) => OnChartUpdated?.Invoke(sender, args);
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
                animator.AnimationEnd += (sender, args) =>
                {
                    OnLoaded?.Invoke(this, null);
                };
                animator.Start();
            }
        }

        private void updateSum()
        {
            Sum = ArcsList.Select(val => val.CurrentValue).Sum(); //LINQ! :D Sam zrobi³em!
        }

        private float CalculateStrokeWidth(float innerRadius, float outterRadius)
        {
            return outterRadius - innerRadius;
        }

        private float CalculateDrawingRadius(float innerRadius, float outterRadius)
        {
            return CalculateStrokeWidth(innerRadius, outterRadius) / 2.0f + innerRadius;
        }

        //DEBUG
        public void incVal()
        {
            ArcsList[0].Value += 10;
        }
        //-----//

        private void CheckTouch(MotionEvent args)
        {
            float X = args.GetX();
            float Y = args.GetY();
            Vector2D relativePosition = new Vector2D(X - Position.X, Y - Position.Y);

            if (!_outterCircle.CheckCollision( new XShared.Utils.Point(relativePosition) ) ) return;

            if (_innerCircle.CheckCollision(new XShared.Utils.Point(relativePosition)))
                OnInnerCircleTouch?.Invoke(this, args);
            else
                OutterCircleTouch(args);
        }

        private void InnerCircleTouch(MotionEvent args)
        {
            switch(args.Action)
            {
                case MotionEventActions.Up:
                    _selectedArc = -1;
                    break;
            }
        }

        private void OutterCircleTouch(MotionEvent args)
        {
            switch(args.Action)
            {
                case MotionEventActions.Up:
                    SelectArc(args.GetX(), args.GetY());
                    break;
            }
        }

        private void SelectArc(float X, float Y)
        {
            Vector2D centerRelativePoint = new Vector2D(X - Position.X, Y - Position.Y);
            float relativePointAngle = centerRelativePoint.GetAngle();
            float circleFraction = 0;
            for (int i = 0; i < ArcsList.Count; i++)
            {
                circleFraction += ArcsList[i].ChartFraction;
                if ( ((relativePointAngle - Angle + 360.0f) % 360)/360.0f < circleFraction )
                {
                    SelectedArc = i;
                    return;
                }
            }
        }

        private void EnlargeStrokeWidth(int key)
        {
            ValueAnimator animator = ValueAnimator.OfFloat(new float[] { ArcsList[key].StandardStrokeWidth, ArcsList[key].StandardStrokeWidth * 1.2f});
            animator.SetDuration(1000);
            animator.Update += (sender, args) =>
            {
                ArcsList[key].StrokeWidth = (float)animator.AnimatedValue;
            };
            animator.Start();
        }

        private void ShrinkStrokeWidth(int key)
        {
            if (key < 0) return;
            ValueAnimator animator = ValueAnimator.OfFloat(new float[] { ArcsList[key].StrokeWidth, ArcsList[key].StandardStrokeWidth });
            animator.SetDuration(1000);

            animator.Update += (sender, args) =>
            {
                ArcsList[key].StrokeWidth = (float)animator.AnimatedValue;
            };
                        animator.Start();
        }

        private void TurnToSelectedSegment(int key)
        {
            if (key < 0) return;
            var prevFractionSum = ArcsList.Take(key).Sum(arc => arc.ChartFraction);
            var totalFractonSum = prevFractionSum + ArcsList[key].ChartFraction;
            var Value = ( 360.0f - (prevFractionSum + ArcsList[key].ChartFraction / 2.0f )*360.0f + SelectedTargetAngle )%360.0f;

            ValueAnimator animator = ValueAnimator.OfFloat( new float[] { Angle, ClosestRotateTo(Value) } );
            animator.SetDuration(1000);
            animator.Update += (sender, args) =>
            {
                Angle = (float)animator.AnimatedValue;
            };
            animator.Start();

            float RightRotateTo(float value)
            {
                if (value > Angle) return value;
                else return value += 360.0f;
            }

            float LeftRotateTo(float value)
            {
                if (value < Angle) return value;
                else return value -= 360.0f;
            }

            float ClosestRotateTo(float value)
            {
                if( Angle > (Angle+180)%360 )
                {
                    if (value > (Angle + 180) % 360 && value < Angle)
                        return LeftRotateTo(value);
                    else
                        return RightRotateTo(value);
                }
                else
                {
                    if (value < (Angle + 180) % 360 && value > Angle)
                        return RightRotateTo(value);
                    else
                        return LeftRotateTo(value);
                }
            }
        }
    }
}