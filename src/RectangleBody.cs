using System;
using Microsoft.Xna.Framework;

namespace Joulurauhaa2020
{
    public class RectangleBody 
    {
        public Vector2 dimensions;
        public Vector2 position;

        public RectangleBody(Vector2 dimensions, Vector2 position)
        {
            this.dimensions = dimensions;
            this.position = position;
        }

        public bool Colliding(CircleBody target)
        {
            // Do a rough estimate of collision by bounding rectangle and circle
            // Check for closest rectangle edge to circle
            // FIXME where's equality checked?
            float testX = target.position.X;
            float testY = target.position.Y;
            if (target.position.X < position.X) // Left
            {
                testX = position.X;
            }
            else if (target.position.X > position.X+dimensions.X) // Right
            {
                testX = position.X+dimensions.X;
            }

            if (target.position.Y < position.Y) // Top
            {
                testY = position.Y;
            }
            else if (target.position.Y > position.Y+dimensions.Y) // Bottom
            {
                testY = position.Y+dimensions.Y;
            }

            // Collision-check
            var testVector = new Vector2(testX, testY);
            float distance = (target.position-testVector).Length();

            return distance <= target.radius;

            // TODO if (true) do texture-level (pixel) collision detection
        }

        public bool Colliding(RectangleBody target)
        {
            // Do a rough estimate of collision by bounding rectangles
            return this.position.X + this.dimensions.X >= target.position.X &&
                   this.position.Y + this.dimensions.Y >= target.position.Y &&
                   this.position.X <= target.position.X + target.dimensions.X &&
                   this.position.Y <= target.position.Y + target.dimensions.Y; 
            // TODO if (true) do texture-level (pixel) collision detection
        } 
    }
}
