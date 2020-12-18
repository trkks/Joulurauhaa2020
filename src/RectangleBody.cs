using System;
using Microsoft.Xna.Framework;

namespace Joulurauhaa2020
{
    public class RectangleBody 
    {
        public bool active;
        public Vector2 dimensions;
        public Vector2 position;

        public RectangleBody(Vector2 dimensions, Vector2 position)
        {
            this.active = true;
            this.dimensions = dimensions;
            this.position = position;
        }

        public bool Colliding(CircleBody target)
        {
            if (!active || !target.active) { return false; }

            // Do a rough estimate of collision by bounding rectangle and circle
            return Utilities.CollidingRectangleCircle(position, dimensions,
                target.position, target.radius);
            // TODO if (true) do texture-level (pixel) collision detection
        }

        public bool Colliding(RectangleBody target)
        {
            if (!active || !target.active) { return false; }

            // Do a rough estimate of collision by bounding rectangles
            return Utilities.CollidingRectangles(position, dimensions,
                target.position, target.dimensions);
            // TODO if (true) do texture-level (pixel) collision detection
        } 
    }
}
