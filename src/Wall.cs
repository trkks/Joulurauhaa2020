using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Joulurauhaa2020
{
    public class Wall
    {
        public float angle;

        private Color color;
        private Point dimensions;

        public RectangleBody body;
        public Vector2 direction; // Direction where collisions reflect
        public Vector2 origin;

        public Wall(Vector2 dimensions, Vector2 direction, Vector2 position)
        { 
            this.angle = (float)Math.Atan2(direction.Y, direction.X);
            this.body = new RectangleBody(dimensions, position);
            this.color = Color.White;
            this.dimensions = dimensions.ToPoint();
            this.direction = direction;
            this.origin = dimensions / 2f; // Origin always dead center
        }

        // TODO Write own collision, so that clipping through is impossible
        // eg. try to throw projectile straight at a wall
        public void PushAway(CircleBody target)
        {
            while (body.Colliding(target))
            {
                // Push away from wall
                target.position += direction;
            }
        }
    }
}
