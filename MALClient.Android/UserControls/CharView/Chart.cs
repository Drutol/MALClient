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
using System.Collections.ObjectModel;


namespace MALClient.Android.UserControls
{
    using MALClient.XShared.Utils;
    public partial class Chart
    {
        class EventList<T> : List<T>
        {
            public event EventHandler<int> ValueChanged;
            public event EventHandler<T> ValueAdded;
            public event EventHandler<T> ValueRemoved;
            public event EventHandler<int> CollectionChanged;

            public new T this[int key]
            {
                get => base[key];
                set
                {
                    base[key] = value;
                    ValueChanged?.Invoke(this, key);
                    CollectionChanged?.Invoke(this, key);
                }
            }

            public void Add(T item)
            {
                base.Add(item);
                ValueAdded?.Invoke(this, item);
                CollectionChanged?.Invoke(this, this.IndexOf(item));
            }

            public void Remove(T item)
            {
                Remove(item);
                ValueRemoved?.Invoke(this, item);
            }

            public void RemoveAt(int key)
            {
                ValueRemoved?.Invoke(this, this[key]);
                CollectionChanged?.Invoke(this, key);
                base.RemoveAt(key);
            }
        }
        private EventList<float> values = new EventList<float>();

        private List<Arc> arcs = new List<Arc>();

        private float Sum { get; set; }

        private float _angle = 0;
        public float Angle
        {
            get => _angle;
            set => _angle = (value + 360.0f) % 360.0f;
        }

        private Circle _centerCircle = new Circle(new Vector2D(0, 0), 0);
        public float CenterRadius
        {
            get => _centerCircle.Radius;
            set
            {
                _centerCircle.Radius = value;
                SegmentsDrawingRadius = value + SegmentsWidth / 2.0f; // THIS... and
            }
        }
        public Vector2D Position
        {
            get => _centerCircle.Position;
            set
            {
                _centerCircle.Position = value;
                ChartLayoutUpdated?.Invoke(this, null);
            }
        }
        private float _segmentsDrawingRadius;
        private float SegmentsDrawingRadius
        {
            get => _segmentsDrawingRadius;
            set
            {
                _segmentsDrawingRadius = value;
                UpdateArcsLengths();
            }
        }
        private float _segmetsWidth;
        public float SegmentsWidth
        {
            get => _segmetsWidth;
            set
            {
                _segmentsDrawingRadius = CenterRadius + value / 2.0f; // THIS. These two are ugly as fuck.
                _segmetsWidth = value;
                foreach (var arc in arcs) arc.StrokeWidth = value;
            }
        }

        private int _selectedSegment = -1;
        public int SelectedSegment
        {
            get => _selectedSegment;
            set
            {
                SegmentSelected?.Invoke(arcs[value], value);
                _selectedSegment = value;
            }
        }

        public float SelectedSegmentWidth { get; set; }

        private event EventHandler<MotionEvent> CenterCirclePressed;
        private event EventHandler<MotionEvent> SegmentsCirclePressed;
        private event EventHandler<int> SegmentSelected;
        public event EventHandler ChartLayoutUpdated;

        public Chart()
        {
            Init();
        }

        public void Draw(Canvas canvas)
        {
            canvas.Save();
            canvas.Translate(Position.X, Position.Y);
            //DEBUG AF
            Paint temp = new Paint();
            temp.Color = new Color(0, 0, 0);
            canvas.DrawCircle(0,0,CenterRadius, temp);
            //------//
            for(int i=0; i<values.Count; i++)
            {
                arcs[i].Draw(canvas, _segmentsDrawingRadius);
                canvas.Rotate((values[i] / Sum) * 360.0f);
            }
            canvas.Restore();
        }

        private void Init()
        {
            //Events for changes in values represented by chart.
            values.CollectionChanged += (list, key) => UpdateSum();
            values.CollectionChanged += (list, key) => UpdateArcsLengths();
            values.CollectionChanged += (list, key) => UpdateSelectionAngle(); //Animations Stuff.
            values.CollectionChanged += (list, key) => ChartLayoutUpdated?.Invoke(this, null);

            SegmentSelected += (value, args) => UpdateSelectionAngle();
            SegmentsWidth = 100; // DEBUG
        }

        private void UpdateSum()
        {
            Sum = values.Sum();
            
        }

        private void UpdateArcsLengths()
        {
            int i = 0;
            float peremiter = SexyMath.CirclePeremiter(_segmentsDrawingRadius);
            foreach(var arc in arcs)
            {
                arc.Length = (values[i] / Sum) * peremiter;
                i++;
            }
        }

        private void UpdateSelectionAngle()
        {
            if (SelectedSegment < 0) return;

            var Value = ( 360.0f -(values.Take(SelectedSegment).Sum() + values[SelectedSegment]/2.0f) * 360.0f + 90.0f/*Selected target angle*/ )% 360 ;

            ValueAnimator animator = ValueAnimator.OfFloat(new float[] { Angle, RightRotateTo(Value) });
            animator.SetDuration(1000);
            animator.Update += (sender, args) =>
            {
                Angle = (float)animator.AnimatedValue;
            };
            animator.AnimationEnd += (sender, args) =>
            {
                Angle = Value;
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
                if (Angle > (Angle + 180) % 360)
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

        public void OnClick(MotionEvent click)
        {
            Point clickPoint = new Point(click.GetX(), click.GetY());
            //Check if chart was pressed at all.
            if (!SexyMath.CheckCollision(new Circle(Position, CenterRadius+SegmentsWidth), clickPoint) ) return;
            //Check if center circle was pressed. If not, segments circle was pressed.
            if (_centerCircle.CheckCollision(clickPoint))
                CenterCirclePressed?.Invoke(this, click);
            else
            {
                SegmentsCirclePressed?.Invoke(this, click);
            }
        }

        private void SelectSegment(Vector2D pos)
        {
            float relativePointAngleFraction = (pos-Position).GetAngle()/360.0f;
            float tempSum = 0;
            int i = 0;
            foreach(var value in values)
            {
                tempSum += value;
                if (relativePointAngleFraction < tempSum / Sum)
                {
                    SelectedSegment = i;
                    return;
                }
                i++;
            }
        }

        public void Add(float value, Color color) //DEBUG ONLY. TO BE DELETED!
        {
            Arc temp = new Arc();
            temp.Color = color;
            temp.StrokeWidth = _segmetsWidth;
            arcs.Add(temp);
            values.Add(value);
        }


//Don't open pls.
/*
                
        private event EventHandler OnLoaded;
        private List<Arc> ArcsList = new List<Arc>();
        public float SelectedTargetAngle { get; set; }

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
            temp.OnValueChanged += (sender, args) => UpdateSum(); 
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
                InnerCircleTouch(args);
            else
                OutterCircleTouch(args);
        }

        private void InnerCircleTouch(MotionEvent args)
        {
            switch(args.Action)
            {
                case MotionEventActions.Up:
                    //_selectedArc = -1;
                    //DBUG
                    ArcsList[0].Value += 5;
                    //
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
                    var dupsko = ArcsList.Sum(arc => arc.ChartFraction);
                    SelectedArc = i;
                    return;
                }
            }
        }

        private void EnlargeStrokeWidth(int key)
        {
            ValueAnimator animator = ValueAnimator.OfFloat(new float[] { ArcsList[key].StandardStrokeWidth, ArcsList[key].StandardStrokeWidth * 1.2f});
            animator.SetDuration(SegmentEnlargeTime);
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
            animator.SetDuration(SegmentShrinkTime);

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

            
            ValueAnimator animator = ValueAnimator.OfFloat( new float[] { Angle, EndAngle(Value) } );
            animator.SetDuration(SegmentSelectRotationTime);
            animator.Update += (sender, args) =>
            {
                Angle = (float)animator.AnimatedValue;
            };
            animator.AnimationEnd += (sender, args) =>
            {
                Angle = Value;
            };
            animator.Start();

            float EndAngle(float value)
            {
                switch(SelectRotateDirection)
                {
                    case RotateDirection.Left:
                        return LeftRotateTo(value);
                    case RotateDirection.Right:
                        return RightRotateTo(value);
                    case RotateDirection.Closest:
                        return ClosestRotateTo(value);
                }
                return 0;
            }

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
        }*/
    }
}