using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Joulurauhaa2020
{
    public class Santa : ICollidable, IDrawable, IUpdatable
    {
        // TODO bunch these fields up into classes for composition
        // position and collisions -> Collisionbody
        // NOTE avoid too much indirection eg.
        // CheckCollision() -> Santa -> Circle -> body -> Circle ...
        public bool alive;
        public bool mLeftReleased;
        public float speed;

        public AnimatedTexture2D animation;
        public CircleBody body;
        public Stack<IProjectile> projectiles;
        public Vector2 direction;

        public Santa(Vector2 position, Texture2D spriteAtlas)
        {
            this.body = new CircleBody(256, position);
            this.animation = new AnimatedTexture2D(spriteAtlas, 
                new Point(64,64), new Vector2(20,32),
                new uint[5] { 2, 4, 10, 2, 5 });
            this.projectiles = new Stack<IProjectile>(13); //max_elves+bottle

            this.speed = 300;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            this.animation.Draw(spriteBatch, this.body.position, 
                this.body.angle);

            // NOTE debug shapes:
            //spriteBatch.Draw(
            //    this.spriteAtlas,
            //    this.body.position,
            //    new Rectangle(257,0, 256,256), // Circle
            //    Color.White,
            //    0f,// angle,
            //    spriteOrigin,
            //    this.body.radius/128, //scale
            //    SpriteEffects.None,
            //    0
            //);            
            //spriteBatch.Draw(
            //    this.spriteAtlas,
            //    this.body.position,
            //    new Rectangle(0,0, 256,256),// spriteRect,
            //    this.color,
            //    (float)(Math.PI/4.0),// angle,
            //    this.spriteOrigin,
            //    1f,
            //    SpriteEffects.None,
            //    0
            //);
        }

        public void ResolveIfColliding(ICollidable target)
        { 
            switch (target)
            {
                case Elf elf:
                    if (this.body.Colliding(elf.body))
                    {
                        //System.Console.WriteLine("Santa collision to Elf");
                        this.animation.color = Color.LightGreen;
                    }
                    else
                    {
                        this.animation.color = Color.White;
                    }
                    break;

                case Wall wall:
                    //System.Console.WriteLine("Santa collision to Wall");
                    break;

                default:
                    /* Actual default implementation not needed */
                    System.Console.WriteLine($"{this} at {target}");
                    break;
            }
        }
 
        public void Update(float deltaTime)
        { 
            // Stop movement, so that it will be set according to input
            this.direction = Vector2.Zero;
            ProcessInput();
            // FIXME Disappears when normalizing; 
            // possibly direction * speed going haywire?
            // Normalize the direction, so eg. abs(Up + Left) == abs(Up)
            //this.direction = Vector2.Normalize(this.direction);
            // Change position according to game time
            this.body.position += (this.direction * this.speed) * deltaTime;
        }

        private void ProcessInput()
        {
            /* Get needed variables */
            MouseState mState = Mouse.GetState();
            KeyboardState kbState = Keyboard.GetState();
            Vector2 towardsMouse = Vector2.Normalize(
                mState.Position.ToVector2() - this.body.position);

            /* Related to position of mouse */
            this.body.angle = 
                (float)Math.Atan2(towardsMouse.Y, towardsMouse.X);
            
            /* Mouse 1 */
            if (mState.LeftButton == ButtonState.Pressed)
            {
                if (mLeftReleased)
                {
                    mLeftReleased = false;
                    // TODO ExecuteHit();
                    this.animation.PlayOnce();
                }
            }
            else
            {
                mLeftReleased = true;
            }

            /* Mouse 2 */
            if (mState.RightButton == ButtonState.Pressed)
            {
                Console.WriteLine("Santa be throwing");
                //if (this.projectiles.Count > 0)
                //{
                //    IProjectile projectile = this.projectiles.Pop();
                //    Console.WriteLine(projectile);
                //    projectile.Direction = towardsMouse;
                //    projectile.Direction *= 20;
                //    projectile.SetDefaultUpdate();
                //}
                /* else 
                {
                    playErrorSound();
                }
                */
            }

            /* WASD */
            if (kbState.IsKeyDown(Keys.W))
            {
                this.direction -= Vector2.UnitY; 
            }
            if (kbState.IsKeyDown(Keys.A))
            {
                this.direction -= Vector2.UnitX; 
            }
            if (kbState.IsKeyDown(Keys.S))
            {
                this.direction += Vector2.UnitY; 
            }
            if (kbState.IsKeyDown(Keys.D))
            {
                this.direction += Vector2.UnitX; 
            }
        }

        private void ExecuteHit(Vector2 towards)
        {
            // Play animation
            // Spawn a hitbox for the hit at correct animation frame
            // Remove hitbox at correct animation frame
        }
    }
}
