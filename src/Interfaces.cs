using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Joulurauhaa2020
{
    public interface ICollidable
    {
        void ResolveIfColliding(ICollidable target);
    }
    
    public interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch);
    }

    public interface IUpdatable
    {
        void Update(float deltaTime);
    }

    public interface IProjectile : IUpdatable, IDrawable, ICollidable
    {
        float Angle { get; set; } // For spinning while flying
        float Speed { get; set; }
        Vector2 Direction { get; set; }
    }
}
