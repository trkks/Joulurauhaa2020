using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Joulurauhaa2020
{
    public class Wall : ICollidable, IDrawable
    {
        public enum Edge
        {
            Left,
            Right,
            Top,
            Bottom
        }

        public float angle;

        public Vector2 origin;
        public Vector2 position;

        public RectangleBody Body { get; set; }

        public Wall(Wall.Edge index)
        { }

        public void Draw(SpriteBatch spriteBatch)
        { }

        public void ResolveIfColliding(ICollidable target)
        { /* static - no response */ }

        public void Update(GameTime gameTime)
        { /* static - no action */ }
    }
}
