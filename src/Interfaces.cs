using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Joulurauhaa2020
{
    public interface ICollidable
    {
        void ResolveIfColliding(ICollidable target);
    }
    
    interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch);
    }

    interface IUpdatable
    {
        void Update(GameTime gameTime);
    }
}
