using System;
using Microsoft.Xna.Framework;

namespace Joulurauhaa2020
{
    public class CircleBody
    {
        public bool isActive;
        public float angle;
        public float radius;
        public Vector2 position;

        public CircleBody(float squareW, Vector2 position)
        {
            this.isActive = true;
            this.angle = 0; // Face along X-axis
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
        
            // Check for closest rectangle edge to circle
            // FIXME where's equality checked?
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
