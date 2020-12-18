using System;
using Microsoft.Xna.Framework;

namespace Joulurauhaa2020
{
    public class CircleBody
    {
        public float radius;
        public Vector2 position;

        /// <summary>
        /// Circle-shaped body for collisions
        /// </summary>
        /// <param name="squareW"> Width of a sprite to fit into </param>
        /// <param name="position"></param>
        public CircleBody(float squareW, Vector2 position)
        {
            // Calculate the maximum radius for a circle in given square
            // ie. let v = Vec2(squareW)
            // radius = Cos( PI / 4 ) * Abs( 1/2 * v )
            //       == Cos( PI / 4 ) * (sqrt( v.X * v.X + v.Y * v.Y ) / 2)
            //       == Cos( PI / 4 ) * (sqrt( v.X * v.X + v.X * v.X ) / 2)
            //       == Cos( PI / 4 ) * (sqrt( 2 * (v.X * v.X) ) / 2)
            this.radius = (float)(Math.Cos(Math.PI/4.0) * (Math.Sqrt(2.0 * squareW * squareW) / 2.0));
            this.position = position;
        }

        public bool Colliding(CircleBody target)
        {
            // Do a rough estimate of collision by bounding circles
            return Utilities.CollidingCircles(position, radius,
                target.position, target.radius);
            // TODO if (true) do texture-level (pixel) collision detection
        }

        public bool Colliding(RectangleBody target)
        {
            // Do a rough estimate of collision by bounding circle and rectangle
            return Utilities.CollidingRectangleCircle(
                target.position, target.dimensions, position, radius);
            // TODO if (true) do texture-level (pixel) collision detection
        } 
    }
}
