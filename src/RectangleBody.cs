using System;
using Microsoft.Xna.Framework;

namespace Joulurauhaa2020
{
    public class RectangleBody 
    {
        public Vector2 dimensions;
        public Vector2 position;

        public RectangleBody(float width, float height, Vector2 position)
        {
            this.dimensions = new Vector2(width, height);
            this.position = position;
        }

        public bool Colliding(CircleBody target)
        {
            // Do a rough estimate of collision by bounding rectangle and circle
            return false; //TODO Rectangle X Circle
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
