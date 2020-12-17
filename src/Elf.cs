using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Joulurauhaa2020
{
    public class Elf
    {
        public bool alive;
        public float angle;
        public float speed;
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
                new uint[4] { 5, 3, 5, 3 });
            this.body = new CircleBody(16, position);
            this.speed = 50;

            // Start animation immediately
            this.animation.animating = true;
        }

        public Projectile AsProjectile()
        {
            // FIXME
            // Freaking slow.. maybe direction in projectile not normalized?
            // or ive not understood/calculated deltaTime right top level...
            var projectile = new Projectile(animation, body, 800);
            return projectile;
        }

        public void Die()
        {
            animation.color = Color.Pink;
            animation.animating = false;
            alive = false;
            //TODO Play death animation
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch, body.position, angle);
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
