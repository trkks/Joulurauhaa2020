using System;
using Microsoft.Xna.Framework;

namespace Joulurauhaa2020
{
    public class CircleBody
    {
        public float radius;
        public Vector2 position;

        public CircleBody(float squareW, Vector2 position)
        {
            // Set to the maximum radius for a circle in square
            this.radius = (float)(0.5 * Math.Sqrt(2.0 * squareW * squareW));
            this.position = position;
        }

        public bool Colliding(CircleBody target)
        {
            // Do a rough estimate of collision by bounding circles
            return Vector2.Distance(this.position, target.position) 
                <= this.radius + target.radius;
            // TODO if (true) do texture-level (pixel) collision detection
        }

        public bool Colliding(RectangleBody target)
        {
            // Do a rough estimate of collision by bounding circle and rectangle
            return false; //TODO Circle X Rectangle
            // TODO if (true) do texture-level (pixel) collision detection
        } 
    }
}
