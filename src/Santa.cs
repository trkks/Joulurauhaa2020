using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Joulurauhaa2020
{
    public class Santa
    {
        public bool alive;
        public AnimatedTexture2D animation;
        public CircleBody body;

        private bool mLeftReleased;
        private bool mRightReleased;
        private float angle;
        private float speed;
        private Stack<Projectile> hangingElves;
        private Vector2 direction;

        public Santa(Vector2 position, Texture2D spriteAtlas)
        {
            this.alive = true;
            this.angle = 0;
            this.animation = new AnimatedTexture2D(spriteAtlas, 
                new Point(128,128), new Vector2(40,64),
                new uint[5] { 2, 4, 10, 2, 5 });
                //, Color.White, 2f);
            this.body = new CircleBody(40, position);
            this.hangingElves = new Stack<Projectile>(8); //MAX_ELVES);
            this.speed = 300;
        }

        public void AddProjectile(Projectile projectile)
        {
            hangingElves.Push(projectile);
            if (hangingElves.Count >= 8) // TODO MAX_ELVES
            {
                alive = false;
                speed = 0;
            }
            // Slow down walking speed
            speed -= speed >= 50f ? 25f : 0f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch, body.position, angle);

            foreach (Projectile elf in hangingElves)
            {
                elf.Draw(spriteBatch);
            }
        }

        private void ExecuteHit(Vector2 towards)
        {
            // Play animation
            // Spawn a hitbox for the hit at correct animation frame
            // Remove hitbox at correct animation frame
        } 

        public void Update(float deltaTime, List<Projectile> projectilesIn)
        { 
            // Stop movement, so that it will be set according to input
            direction = Vector2.Zero;

            // Process input:

            // Get some needed values
            MouseState mState = Mouse.GetState();
            KeyboardState kbState = Keyboard.GetState();
            Vector2 towardsMouse = Vector2.Normalize(
                mState.Position.ToVector2() - body.position);

            // Turn towards position of mouse
            angle = (float)Math.Atan2(towardsMouse.Y, towardsMouse.X);
            
            /* Mouse 1 */
            if (mState.LeftButton == ButtonState.Pressed && mLeftReleased)
            {
                Console.WriteLine("Santa be swinging");
                // TODO ExecuteHit();
                animation.PlayOnce();
            }
            // No holding down
            mLeftReleased = mState.LeftButton == ButtonState.Released;

            /* Mouse 2 */
            if (mState.RightButton == ButtonState.Pressed && mRightReleased)
            {
                if (hangingElves.Count > 0)
                {
                    Console.WriteLine("Santa be throwing");
                    Projectile elf = hangingElves.Pop();
                    elf.Fly(towardsMouse);
                    projectilesIn.Add(elf);
                    // Increase speed from weight loss
                    speed += speed <= 300f ? 25f : 0f;
                }
                else
                {
                    Console.WriteLine("Santa's out of projectiles");
                    //playErrorSound();
                }
            }
            // No holding down
            mRightReleased = mState.RightButton == ButtonState.Released;

            /* WASD */
            if (kbState.IsKeyDown(Keys.W))
            {
                direction -= Vector2.UnitY; 
            }
            if (kbState.IsKeyDown(Keys.A))
            {
                direction -= Vector2.UnitX; 
            }
            if (kbState.IsKeyDown(Keys.S))
            {
                direction += Vector2.UnitY; 
            }
            if (kbState.IsKeyDown(Keys.D))
            {
                direction += Vector2.UnitX; 
            }
            

            // FIXME *Everything* disappears when normalizing; 
            // possibly direction * speed going haywire?
            // Normalize the direction, so eg. abs(Up + Left) == abs(Up)
            //this.direction = Vector2.Normalize(this.direction);
            // Change position according to game time
            body.position += direction * speed * deltaTime;
    
            // Update the elf-projectiles attached to santa
            int hangingIndex = 1;
            foreach (Projectile elf in hangingElves)
            {
                elf.body.position = body.position + Vector2.Transform(
                    1.2f * body.radius * Vector2.Normalize(towardsMouse), 
                    Matrix.CreateRotationZ(
                            hangingIndex * (float)(Math.PI / 4)
                    )
                );

                Vector2 towardsSanta = Vector2.Normalize(
                    body.position - elf.body.position);
                elf.angle = (float)Math.Atan2(towardsSanta.Y, 
                                              towardsSanta.X);

                hangingIndex++;
            }
        }
    }
}
