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
        public int Health 
        { //FIXME Great example of "programming to an implementation"
            set 
            { 
                health = value;
                if (health <= 1)
                {
                    animation.color = GameJR2020.colorOfHurt;
                    movementSpeed = 50;
                }
            }
        }

        public bool alive;
        public bool invincible;
        public float angle;
        public float speed;
        public AnimatedTexture2D animation;
        public CircleBody body;
        public Vector2 direction;

        private float directionLerp;
        private float movementSpeed;
        private int health;
        private AnimatedTexture2D winAnimation;
 
        public Vector2 Direction 
        {
            get => direction;
            // NOTE always a normalized vector
            set => direction = Vector2.Normalize(value);
        }

        public Elf(Vector2 position, Texture2D spriteAtlas)
        {
            this.alive = true;
            this.invincible = false;
            this.animation = new AnimatedTexture2D(spriteAtlas, 
                new Point(32,32), new Vector2(16,16),
                new uint[4] { 5, 3, 5, 3 }, 0.5f);
            this.body = new CircleBody(20, position);
            this.health = 2;
            this.winAnimation = new AnimatedTexture2D(spriteAtlas, 
                new Point(32,32), new Vector2(16,16),
                new uint[4] { 15, 9, 15, 9 }, 0.5f);
         
            this.movementSpeed = 100;
            ResetSpeed();
            // Start animation immediately
            this.animation.animating = true;
            // Also "start" win animation here prematurely
            this.winAnimation.animating = true;
        }

        public Projectile AsProjectile()
        {
            // FIXME
            // Freaking slow.. maybe direction in projectile not normalized?
            // or ive not understood/calculated deltaTime right top level...
            var projectile = new Projectile(animation, body, 800, 2,
                                            Tag.Elf);
            return projectile;
        }

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

        public bool Hurt(int damage)
        {
            if (!invincible)
            {
                health -= damage;
                //TODO hurtAction.Active=true; to temporarily disable hitbox
                if (health <= 1)
                {
                    movementSpeed = 50;
                    animation.color = GameJR2020.colorOfHurt;
                }
                if (health <= 0)
                {
                    Die();
                    return true;
                }
            }
            return false;
        }

        public void ResetSpeed()
        {
            speed = movementSpeed;
        }

        public void SlowDown()
        {
            // FIXME slowdown aint working; collisions fricked?
            speed *= slowdown;
        }

        public void Update(float deltaTime, Santa santa)
        {
            if (speed != movementSpeed) Console.WriteLine(speed);
            // FIXME lerpin aint workin
            //if (directionLerp >= 1f)
            //{   
            //    directionLerp = 0;
            //}

            directionLerp += 0.1f;            
            Direction = Vector2.Lerp(
                Direction, santa.body.position-body.position,
                directionLerp);
            angle = (float)Math.Atan2(Direction.Y, Direction.X);
            
            if (santa.IsDead)
            {
                if (animation != winAnimation)
                {
                    animation = winAnimation;
                    // Swing in sync with other spawned elves
                    directionLerp = 0;
                }
                // Rock back and forth to simulate dancing
                angle += (float)Math.Clamp(Math.Sin(directionLerp), -.5, .5);
            }
            else
            {
                body.position += Direction * speed * deltaTime;
            }
        }
    }
}
