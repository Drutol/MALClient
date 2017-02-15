using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.XShared.Utils
{
    public static class SexyMath
    {
        /// <summary>
        /// <para>Returns angle between vector (x, y) that starts in (0, 0) relative to X+ axis.</para>
        /// <para>X values grow to the right, Y values grow to the bottom.</para>
        /// </summary>
        public static float VectorAngle(float x, float y)
        {
            float relativeAngle = 0;

            if (x < 0)
                relativeAngle += 180;
            else if (y < 0)
                relativeAngle += 360;

            return (float)(Math.Atan(y / x) * (180.0f / Math.PI)) + relativeAngle;
        }

        /// <summary>
        /// <para>Returns angle between vector (pointX, pointY) that starts in (pivotX, pivotY) relative to X+ axis.</para>
        /// <para>X values grow to the right, Y values grow to the bottom.</para>
        /// </summary>
        public static float VectorAngle(float pointX, float pointY, float pivotX, float pivotY)
        {
            return VectorAngle(pointX - pivotX, pointY - pivotY);
        }

        /// <summary>
        /// <para>Returns length of a vector.</para>
        /// <para>It's basically length of line segment from (0,0) to (x, y)</para>
        public static float VectorLength(float x, float y)
        {
            return (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        /// <summary>
        /// <para>Converts degrees value to radians.</para>
        /// <para>Be careful not to convert radians to radians!</para>
        /// </summary>
        public static float ToRadians(float angle)
        {
            return angle * (float)Math.PI / 180.0f;
        }

        /// <summary>
        /// <para>Converts radians value to degrees.</para>
        /// <para>Be careful not to convert degrees to degrees!</para>
        /// </summary>
        public static float ToDegrees(float angle)
        {
            return angle * 180.0f / (float)Math.PI;
        }

        /// <summary>
        /// Returns a vector that points to the defined direction (x, y) but has length equal to 1.
        /// </summary>
        public static Vector2D UnitVector(float x, float y)
        {
            var rescaler = 1.0f/VectorLength(x, y);
            return new Vector2D(x*rescaler, y*rescaler);
        }
    }
}
