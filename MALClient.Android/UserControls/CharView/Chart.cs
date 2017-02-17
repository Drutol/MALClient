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

namespace MALClient.Android.UserControls
{
    public partial class Chart
    {
        private View _view;
        private List<Arc> ArcsList = new List<Arc>();

        private int _selectedArc = -1;
        public float InnerCircleRadius { get; set; } = 400;
        public float OutterCircleRadius { get; set; } = 500;

        private float _currentSum = 0;
        private float currentSum
        {
            get { return _currentSum; }
            set
            {
                _currentSum = value;
                SumUpdate();
            }
        }
        public Chart(View view)
        {
            _view = view;
        }

        public Chart(View view, float innerCircleRadius, float outterCircleRadius)
        {
            _view = view;
            InnerCircleRadius = innerCircleRadius;
            OutterCircleRadius = outterCircleRadius;
        }
        
        public void Draw(Canvas canvas)
        {
            canvas.Save();
            foreach(var arc in ArcsList)
            {
                arc.Draw(canvas);
                canvas.Rotate((arc.GetValue() / currentSum) * 360.0f);
            }
            canvas.Restore();
        }

        public void AddArc(float value, Color color)
        {
            ArcsList.Add(new Arc(value, color, this));
        }
        //DEBUG
        public void incVal()
        {
            ArcsList[0].SetValue(ArcsList[0].GetValue() + 10);
            //ArcsList[0].SetValue(ArcsList[0].GetValue() + 10);
        }
        //-----//
        public void SumUpdate()
        {
            foreach (Arc arc in ArcsList) arc.UpdateLength();
            _view.Invalidate();
        }
    }
}