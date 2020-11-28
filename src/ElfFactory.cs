using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Joulurauhaa2020
{
    public class Elf : ICollidable, IDrawable, IUpdatable
    {
        public bool alive;
        public float speed;
        public float angle;
        public int spriteIndex;

        public Vector2 position;
        public Vector2 direction;
        public Vector2 origin;

        public Rectangle Bounds { get; }

        public Elf()
        { }

        public void Draw(SpriteBatch spriteBatch)
        { }

        public void ResolveIfColliding(ICollidable target)
        { }

        public void Update(GameTime gameTime)
        { }
    }

    public class ElfFactory
    {
        public Texture2D spriteAtlas;

        public ElfFactory(Texture2D spriteAtlas0)
        {
            spriteAtlas = spriteAtlas0;
        }

        public Elf Create()
        {
            return new Elf();
        }
    }
}
