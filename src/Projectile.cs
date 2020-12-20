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
        public enum State
        {
            None = 0,
            Pickup = 1,
            Flying = 2,
            Broken = 4
        }

        public bool bounced { get => bounceCount > 0; }
        public float angle; 
        private State state;
        public Tag tag;
        public AnimatedTexture2D animation;
        public CircleBody body; // Object spinning in air -> use circles

        private const float bounceMultiplier = 1.5f;
        private readonly float originalSpeed;
        private float slowdown = 100f;
        private float speed;
        private int bounceCount; 
        private Vector2 direction;

        public Projectile(AnimatedTexture2D animation, CircleBody body, 
                          float speed, Tag tag, State state=State.None)
        {
            this.bounceCount = 0;
            this.state = state;
            this.angle = 0;
            this.speed = speed;
            this.originalSpeed = speed;
            this.animation = animation;
            this.body = body;
            this.direction = Vector2.Zero;
            this.tag = tag;

            // TODO 0.8f == GameJR2020.ProjectileLayer
            this.animation.layer = 0.8f;
        }

        public void Bounce(Vector2 normal)
        {
            if (state != State.Broken)
            {
                direction = Vector2.Normalize(
                    Vector2.Reflect(direction, normal));
                bounceCount++;
                slowdown *= bounceMultiplier * bounceCount;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (state == State.Broken)
            {
                animation.DrawLastFrame(spriteBatch, body.position, angle);
            }
            else
            {
                animation.Draw(spriteBatch, body.position, angle);
            }
        }

        public void Break()
        {
            bounceCount = 0;
            state = State.Broken;
            body.active = false;
            animation.color = GameJR2020.colorOfDeath;
            animation.layer = 0.25f;
            animation.PlayOnce();
        }

        public void Fly(Vector2 toward)
        {
            direction = Vector2.Normalize(toward);
            state = State.Flying | State.Pickup;
            body.active = true;
        }

        public bool StateIs(State s)
        {
            return (state & s) != State.None;
        }

        public void Reset()
        {
            bounceCount = 0;
            state = State.None;
            speed = originalSpeed;

            animation.layer = 0.8f;
        }

        public void Update(float deltaTime)
        {
            if (StateIs(State.Flying))
            {
                System.Console.WriteLine(speed);
                speed -= slowdown * deltaTime;
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
