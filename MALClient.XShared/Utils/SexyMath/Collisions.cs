using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.XShared.Utils
{
    public static partial class SexyMath
    {
        /// <summary>
        /// Returns true if two circles intersect.
        /// </summary>
        public static bool CheckCollision(Circle one, Circle two)
        {
            Vector2D Debug = one.Position - two.Position;
            var length = VectorLength(Debug);
            return ( VectorLength(one.Position - two.Position) <= one.Radius + two.Radius );
        }

        /// <summary>
        /// Returns true if point is within radius value from (0, 0) point.
        /// </summary>
        public static bool CheckCollision(float radius, Vector2D point)
        {
            return (VectorLength(point) <= radius);
        }

        /// <summary>
        /// Returns true if point is within circle's field.
        /// </summary>
        public static bool CheckCollision(Circle circle, Point point)
        {
            return ( VectorLength(circle.Position - point.Position) <= circle.Radius );
        }
        public static bool CheckCollision(Point point, Circle circle)
        {
            return CheckCollision(circle, point);
        }

        /// <summary>
        /// Returns true if two points overlap.
        /// </summary>
        public static bool CheckCollision(Vector2D one, Vector2D two)
        {
            return one.X == two.X && one.Y == two.Y;
        }

        
    }
}
