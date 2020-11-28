using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Joulurauhaa2020
{
    public class Santa : ICollidable, IDrawable, IUpdatable
    {
        public bool alive;
        public float speed;
        public float angle;
        public int spriteIndex;

        public Vector2 position;
        public Vector2 direction;
        public Vector2 origin;

        public Rectangle Bounds { get; }

        public Santa()
        { }

        public void Draw(SpriteBatch spriteBatch)
        { }

        public void ResolveIfColliding(ICollidable target)
        { }

        public void Update(GameTime gameTime)
        { }
    }
}
