using System;
using Microsoft.Xna.Framework;

namespace Joulurauhaa2020
{
    public class CircleBody
    {
        public bool isActive;
        public float radius;
        public Vector2 position;

        /// <summary>
        /// Circle-shaped body for collisions
        /// </summary>
        /// <param name="squareW"> Width of a sprite to fit into </param>
        /// <param name="position"></param>
        public CircleBody(float squareW, Vector2 position)
        {
            this.isActive = true;
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
            return Vector2.Distance(position, target.position) 
                <= radius + target.radius;
            // TODO if (true) do texture-level (pixel) collision detection
        }

        public bool Colliding(RectangleBody target)
        {
            // Do a rough estimate of collision by bounding circle and rectangle
        
            // Check for closest rectangle edge to circle
            float testX = position.X;
            float testY = position.Y;
            if (position.X < target.position.X) // Left
            {
                testX = position.X;
            }
            else if (position.X > target.position.X+target.dimensions.X) // Right
            {
                testX = target.position.X+target.dimensions.X;
            }

            if (position.Y < target.position.Y) // Top
            {
                testY = target.position.Y;
            }
            else if (position.Y > target.position.Y+target.dimensions.Y) // Bottom
            {
                testY = target.position.Y+target.dimensions.Y;
            }

            // Collision-check
            var testVector = new Vector2(testX, testY);
            float distance = (position-testVector).Length();

            return distance <= radius;

            // TODO if (true) do texture-level (pixel) collision detection
        } 
    }
}
