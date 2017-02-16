using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.XShared.Utils
{
    public abstract class Shape
    {
        public Vector2D Position { get; set; }

        public abstract bool CheckCollision(Shape collider);
        public abstract bool CheckCollision(Circle circle);
        public abstract bool CheckCollision(Point point);
    }
}
