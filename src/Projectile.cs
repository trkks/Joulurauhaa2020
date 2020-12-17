using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Joulurauhaa2020
{
    public class Projectile
    {
        public bool bounced; 
        public bool flying; 
        public float angle; 
        public float speed;
        public AnimatedTexture2D animation;
        public CircleBody body; // Object spinning in air -> use circles
        public Vector2 direction;

        public Projectile(AnimatedTexture2D animation, CircleBody body, 
                          float speed)
        {
            this.bounced = false;
            this.flying = false;
            this.angle = 0;
            this.speed = speed;
            this.animation = animation;
            this.body = body;
            this.direction = Vector2.Zero;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch, body.position, angle);
        }

        public void Bounce(Vector2 normal)
        {
            direction = Vector2.Normalize(
                Vector2.Reflect(direction, normal));
            bounced = true;
        }

        public void Break()
        {
            flying = false;
            //TODO Play breaking animation & darken color
        }

        public void Fly(Vector2 toward)
        {
            direction = Vector2.Normalize(toward);
            flying = true;
        }

        public void Update(float deltaTime)
        {
            if (bounced)
            {
                // Slow down until stopped
                speed -= 40; 
                if (speed <= 0)
                {
                    Break();
                }
            }
            if (flying)
            {
                body.position += direction * speed * deltaTime;
                angle += 0.2f;
            }
        }
    }
}
