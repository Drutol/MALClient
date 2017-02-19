using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.XShared.Utils
{
    public class Point : Shape
    {
        public Point(float x, float y)
        {
            Position = new Vector2D(x, y);
        }

        public Point(Vector2D position)
        {
            Position = new Vector2D(position);
        }

        public override bool CheckCollision(Circle circle)
        {
            throw new NotImplementedException();
        }

        public override bool CheckCollision(Point point)
        {
            throw new NotImplementedException();
        }

        public override bool CheckCollision(Shape collider)
        {
            return collider.CheckCollision(this);
        }
    }
}
