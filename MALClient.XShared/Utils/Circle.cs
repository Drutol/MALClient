using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.XShared.Utils
{
    public class Circle
    {
        public Vector2D CenterPoint { get; set; }
        public float Radius { get; set; }

        public Circle(Vector2D center, float radius)
        {
            CenterPoint = center;
            Radius = radius;
        }

        public Circle(float x, float y, float radius)
        {
            CenterPoint = new Vector2D(x, y);
            Radius = radius;
        }

        public float GetPeremiter()
        {
            return SexyMath.CirclePeremiter(Radius);
        }

        public float GetField()
        {
            return SexyMath.CircleField(Radius);
        }
    }
}
