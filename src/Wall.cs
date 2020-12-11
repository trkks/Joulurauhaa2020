using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Joulurauhaa2020
{
    public class Wall : ICollidable, IDrawable
    {
        public float angle;

        private Color color;
        private Point dimensions;

        public RectangleBody body;
        public Texture2D sprite;
        public Vector2 direction; // Direction, where collisions reflect
        public Vector2 origin;

        public Wall(Texture2D sprite, Vector2 dimensions, Vector2 direction,
            Vector2 position)
        { 
            this.sprite = sprite;
            this.direction = direction;

            this.origin = dimensions / 2f; // Origin always dead center
            this.angle = (float)Math.Atan2(direction.Y, direction.X);
            this.body = new RectangleBody(dimensions, position);

            this.dimensions = dimensions.ToPoint();
            this.color = Color.White;
        }

        public List<Dot> dots = new List<Dot>(100);
        public void Draw(SpriteBatch spriteBatch)
        { 
            //TODO Animate doors here?; would need to know when spawning, so
            // also make walls spawners? (-> new bottles also spawn from doors?)

            spriteBatch.Draw(
                sprite,
                body.position,
                new Rectangle(Point.Zero,dimensions),
                color,
                angle,
                origin,
                1f,
                SpriteEffects.None,
                0
            );            
            foreach (Dot dot in dots)
                dot.Draw(spriteBatch);
        }

        public void ResolveIfColliding(ICollidable target)
        { 
            switch (target)
            {
                case Santa santa:
                    while (body.Colliding(santa.body))
                    {
                        dots.Add(new Dot(
                            sprite, 
                            body.position)
                        );
                    
                        dots.Add(new Dot(
                            sprite, 
                            santa.body.position)
                        );    // Push away from wall
                        santa.body.position += direction;
                    }
                    break;
                case Wall _:
                    // No response
                    break;
             }
            /* static - no response */ }

        public void Update(float deltaTime)
        { /* static - no action */ }
    }
}
