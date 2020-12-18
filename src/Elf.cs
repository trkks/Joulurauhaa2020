using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Joulurauhaa2020
{
    public class Elf
    {
        public static float slowdown = 0.4f;

        public bool alive;
        public float angle;
        public float speed;
        public int health;
        public AnimatedTexture2D animation;
        public CircleBody body;
        public Vector2 direction;

        private float directionLerp;
 
        public Vector2 Direction 
        {
            get => direction;
            // NOTE always a normalized vector
            set => direction = Vector2.Normalize(value);
        }

        public Elf(Vector2 position, Texture2D spriteAtlas)
        {
            this.alive = true;
            this.animation = new AnimatedTexture2D(spriteAtlas, 
                new Point(32,32), new Vector2(16,16),
                new uint[4] { 5, 3, 5, 3 }, 0.5f);
            this.body = new CircleBody(20, position);
            this.health = 2;

            ResetSpeed();
            // Start animation immediately
            this.animation.animating = true;
        }

        public Projectile AsProjectile()
        {
            // FIXME
            // Freaking slow.. maybe direction in projectile not normalized?
            // or ive not understood/calculated deltaTime right top level...
            var projectile = new Projectile(animation, body, 800, Tag.Elf);
            return projectile;
        }

        // TODO change this into Hurt()?
        public void Die()
        {
            animation.animating = false;
            animation.layer = 0.25f; // Move to bottom
            animation.color = GameJR2020.colorOfDeath;
            alive = false;
            //TODO Play death animation
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch, body.position, angle);
        }

        public void Hurt(int damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
        }

        public void ResetSpeed()
        {
            speed = 100;
        }

        public void SlowDown()
        {
            speed *= Elf.slowdown;
            if (speed < 10)
            {
                speed = 10;
            }
        }

        public void Update(float deltaTime, Santa santa)
        {
            if (directionLerp >= 1f)
            {   
                directionLerp = 0;
            }
            directionLerp += 0.2f;
            Direction = Vector2.Lerp(
                Direction, santa.body.position-body.position,
                directionLerp);
            angle = (float)Math.Atan2(Direction.Y, Direction.X);
            body.position += Direction * speed * deltaTime;
        }
    }
}
