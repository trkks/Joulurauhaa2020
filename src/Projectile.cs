using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Joulurauhaa2020
{
    // Tag for differentiating between collisions reactions etc
    public enum Tag
    {
        Elf,
        Bottle
    }

    public class Projectile
    {
        public bool bounced; 
        public bool flying; 
        public float angle; 
        public float speed;
        public Tag tag;
        public AnimatedTexture2D animation;
        public CircleBody body; // Object spinning in air -> use circles
        public Vector2 direction;

        private const float bounceMultiplier = 1.5f;
        private float slowdown = 10f;

        public Projectile(AnimatedTexture2D animation, CircleBody body, 
                          float speed, Tag tag)
        {
            this.bounced = false;
            this.flying = false;
            this.angle = 0;
            this.speed = speed;
            this.animation = animation;
            this.body = body;
            this.direction = Vector2.Zero;
            this.tag = tag;

            this.animation.layer = 0.8f;
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
            animation.animating = false;
            animation.color = GameJR2020.colorOfDeath;
            animation.layer = 0.25f;
            //TODO Play breaking animation & darken color
        }

        public void Fly(Vector2 toward)
        {
            direction = Vector2.Normalize(toward);
            flying = true;
        }

        public void Update(float deltaTime)
        {
            if (flying)
            {
                if (bounced)
                {
                    slowdown *= bounceMultiplier; 
                }
                speed -= slowdown;
                if (speed <= 0)
                {
                    Break();
                }
                else
                {
                    body.position += direction * speed * deltaTime;
                }
                angle += 0.2f;
            }
        }
    }
}
