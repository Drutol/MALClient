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
        public float InnerCircleRadius { get; set; } = 300;
        public float OutterCircleRadius { get; set; } = 500;

        private float currentSum { get; set; } = 0;

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
            float angle = 0;
            canvas.Save();
            foreach(var arc in ArcsList)
            {
                arc.Draw(canvas);
                canvas.Rotate((arc.Value / currentSum) * 360.0f);
            }
            canvas.Restore();
        }

        public void AddArc(float value, Color color)
        {
            currentSum += value;
            ArcsList.Add(new Arc(value, color, this));
            ValueChanged();
        }

        public void incVal()
        {
            ArcsList[0].Value++;
        }

        public void ValueChanged()
        {
            currentSum = 0;
            foreach (Arc arc in ArcsList) currentSum += arc.Value;
            foreach (Arc arc in ArcsList) arc.UpdateLength();
            _view.Invalidate();
        }
    }
}