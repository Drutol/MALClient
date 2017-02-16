using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.XShared.Utils
{
    public class Vector2D
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2D(Vector2D vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        /// <summary>
        /// Moves vector by given dimensions.
        /// </summary>
        public void Translate(float x, float y)
        {
            X += x;
            Y += y;
        }

        /// <summary>
        /// Moves vector by given vector.
        /// </summary>
        public void Translate(Vector2D vector)
        {
            X += vector.X;
            Y += vector.Y;
        }

        /// <summary>
        /// Rotates vector by given angle.
        /// </summary>
        /// <param name="angle"></param>
        public void Rotate(float angle)
        {
            var length = GetLength();
            if (length == 0) return;
            var relativeAngle = GetAngle();

            X = (float)Math.Cos(relativeAngle + angle) * length;
            Y = (float)Math.Sin(relativeAngle + angle) * length;
        }

        /// <summary>
        /// Turns vector by 180 degrees.
        /// </summary>
        public void Flip()
        {
            X = -X;
            Y = -Y;
        }

        /// <summary>
        /// Turns vector by +90 degrees. It's a fast way of making a perpendicular vector.
        /// </summary>
        public void FlipPerpendicular()
        {
            float temp = Y;
            Y = -X;
            X = temp;
        }

        /// <summary>
        /// Multiplies vector by the same value.
        /// </summary>
        /// <param name="scale"></param>
        public void Scale(float scale)
        {
            X *= scale;
            Y *= scale;
        }

        /// <summary>
        /// Multiplies vector by values.
        /// </summary>
        public void Scale(float scaleX, float scaleY)
        {
            X *= scaleX;
            Y *= scaleY;
        }

        /// <summary>
        /// <para>Returns angle between vector (x, y) that starts in (0, 0) relative to X+ axis.</para>
        /// <para>X values grow to the right, Y values grow to the bottom.</para>
        /// </summary>
        public float GetAngle()
        {
            return SexyMath.VectorAngle(X, Y);
        }

        /// <summary>
        /// <para>Returns angle between vector (x, y) that starts in (pivotX, pivotY) relative to X+ axis.</para>
        /// <para>X values grow to the right, Y values grow to the bottom.</para>
        /// </summary>
        public float GetAngle(Vector2D pivot)
        {
            return (this - pivot).GetAngle();
        }

        /// <summary>
        /// <para>Returns length of a vector.</para>
        /// <para>It's basically length of line segment from (0,0) to (x, y)</para>
        public float GetLength()
        {
            return SexyMath.VectorLength(X, Y);
        }

        public static Vector2D operator +(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2D operator -(Vector2D a, Vector2D b)
        {
            return new Vector2D(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2D operator *(Vector2D a, float value)
        {
            Vector2D result = new Vector2D(a);
            result.Scale(value);
            return result;
        }

        /// <summary>
        /// Returns a vector that points to the defined direction (x, y) but has length equal to 1.
        /// </summary>
        public Vector2D GetUnitVector()
        {
            return SexyMath.UnitVector(X, Y);
        }
    }
}
