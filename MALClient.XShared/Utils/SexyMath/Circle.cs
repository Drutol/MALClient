using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.XShared.Utils
{
    public class Circle : Shape
    {
        public float Radius { get; set; }

        public Circle(Vector2D center, float radius)
        {
            Position = center;
            Radius = radius;
        }

        public Circle(float x, float y, float radius)
        {
            Position = new Vector2D(x, y);
            Radius = radius;
        }

        public float GetPeremiter()
        {
            return SexyMath.CirclePeremiter(Radius);
        }

        public override bool CheckCollision(Circle collider)
        {
            return SexyMath.CheckCollision(this, collider);
        }

        public override bool CheckCollision(Point collider)
        {
            return SexyMath.CheckCollision(this, collider);
        }

        public override bool CheckCollision(Shape collider)
        {
            return collider.CheckCollision(this);
        }

        public float GetField()
        {
            return SexyMath.CircleField(Radius);
        }

    }
}
