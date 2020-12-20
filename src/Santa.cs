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
        public float speed;
        public int meleeDamage;
        public AnimatedTexture2D animation;
        public CircleBody body;
        public CircleBody melee;

        private bool mLeftReleased;
        private bool mRightReleased;
        private bool swinging;
        private bool hasBottle;
        private float angle;
        private uint swingStart;
        private uint swingEnd;
        private AnimatedTexture2D bottleAnimation;
        private AnimatedTexture2D fistAnimation;
        private Stack<Projectile> projectiles;
        private Projectile bottle;
        private Vector2 direction;
        private Vector2 facingDirection;

        public Santa(Vector2 position, Texture2D bottleSpriteAtlas,
                     Texture2D fistSpriteAtlas)
        {
            this.alive = true;
            this.angle = 0;
            this.bottleAnimation = new AnimatedTexture2D(bottleSpriteAtlas, 
                new Point(128,128), new Vector2(40,64),
                new uint[5] { 2, 4, 10, 2, 5 }, 0.75f);
            this.fistAnimation = new AnimatedTexture2D(fistSpriteAtlas, 
                new Point(128,128), new Vector2(40,64),
                new uint[5] { 2, 4, 10, 2, 5 }, 0.75f);
            this.body = new CircleBody(40, position);
            this.projectiles = new Stack<Projectile>(8); //MAX_ELVES);
            //TODO This needs to be huge... its weird
            this.melee = new CircleBody(60, position);
            this.meleeDamage = 2;
            this.speed = 300;
            this.swingStart = 4;
            this.swingEnd = 7;
            this.hasBottle = true;
            this.animation = bottleAnimation;

            this.melee.active = false;
        }

        public void AddProjectile(Projectile projectile)
        {
            projectile.Reset();
            if (projectile.tag == Tag.Elf)
            {
                // Swap elf below bottles, so bottles always on top:

                // Take bottles
                var bottlesTemp = new List<Projectile>(3);
                while (projectiles.Count > 0 &&
                       projectiles.Peek().tag == Tag.Bottle)
                {
                    bottlesTemp.Add(projectiles.Pop());
                }

                // Add elf
                projectiles.Push(projectile);
                // Slow down walking speed
                speed -= speed >= 50f ? 25f : 0f;

                // Push bottles back
                foreach (Projectile bottleTemp in bottlesTemp)
                {
                    projectiles.Push(bottleTemp);
                }

                // Check for death
                int elfCount = 0; 
                foreach (Projectile p in projectiles)
                {
                    if (p.tag == Tag.Elf)
                    {
                        elfCount++;
                    }
                }
                if (elfCount >= 8) // TODO MAX_ELVES
                {
                    //TODO Die();
                    alive = false;
                    speed = 0;
                }
            }
            else if (projectile.tag == Tag.Bottle)
            {
                // Check for max bottles in inventory
                int bottleCount = 0; 
                foreach (Projectile p in projectiles)
                {
                    if (p.tag == Tag.Bottle)
                    {
                        bottleCount++;
                    }
                }         
                if (bottleCount < 3)
                {
                    // Hide from view
                    projectile.body.position = new Vector2(-44, -44);
                    // Push bottle on top
                    projectiles.Push(projectile);
                }
                // TODO Do these in some better place?
                //hasBottle = true;
                //animation = bottleAnimation;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch, body.position, angle);

            foreach (Projectile elf in projectiles)
            {
                elf.Draw(spriteBatch);
            }
        }

        public void Update(float deltaTime, List<Projectile> projectilesIn)
        { 
            // Get some needed values
            MouseState mState = Mouse.GetState();
            KeyboardState kbState = Keyboard.GetState();

            // Process key-input:

            /* Mouse 1 */
            if (mState.LeftButton == ButtonState.Pressed && mLeftReleased)
            {
                swinging = true;
            }
            // No holding down
            mLeftReleased = mState.LeftButton == ButtonState.Released;

            /* Mouse 2 */
            bool throwing = mState.RightButton == ButtonState.Pressed &&
                            mRightReleased;

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
            
            // React to inputs: 

            if (swinging) // Continue or stop the swing:
            {
                // TODO Wrap this logic into an object?:
                // FramedExecutor.Execute(); 
                // == Animation + Action on specific frame

                // Play animation
                // Spawn a hitbox for the hit at correct animation frame
                // Remove hitbox at correct animation frame
 
                uint tft = animation.TotalFrameTime;
                //System.Console.WriteLine($"{swingStart} < {tft} < {swingEnd}");
                if (tft < swingStart)
                {
                    Console.WriteLine("Santa started swing");
                    // Start animation
                    animation.PlayOnce();
                }
                else if (tft <= swingEnd)
                {
                    System.Console.WriteLine("Santa swinging");
                    // Activate melee hitbox
                    melee.active = true;
                    // Position hitbox to front
                    melee.position = body.position + 
                        facingDirection * (body.radius + melee.radius);
                }
                else if (tft > swingEnd)
                {
                    System.Console.WriteLine("Santa stopped swing");
                    // Disable swinging-state and melee hitbox
                    swinging = false;
                    melee.active = false;
                    //System.Console.WriteLine("Melee inactive");
                }
            }
            else // Only update certain actions when not attacking
            {
                // Allow moving when not swinging
                // FIXME *Everything* disappears when normalizing; 
                // possibly direction * speed going haywire?
                // Normalize the direction, so eg. abs(Up + Left) == abs(Up)
                //this.direction = Vector2.Normalize(this.direction);
                // Change position according to game time
                body.position += direction * speed * deltaTime;

                // Turn towards the mouse
                facingDirection = Vector2.Normalize(
                    mState.Position.ToVector2() - body.position);
                angle = (float)Math.Atan2(facingDirection.Y,
                                          facingDirection.X);
            }

            if (throwing)
            {
                // TODO Play animation
                ThrowProjectile(projectilesIn);
            }

            // Reset movement direction as to prevent "drifting"
            // TODO direction here means movement direction, when in other 
            // classes it's the facing -direction :/
            direction = Vector2.Zero;

            // Update the elf-projectiles attached to santa:

            int hangingIndex = 1;
            var elves = Array.FindAll(projectiles.ToArray(), 
                                      p=>p.tag == Tag.Elf);
            foreach (Projectile elf in elves)
            {
                elf.body.position = body.position + Vector2.Transform(
                    1.2f * body.radius * Vector2.Normalize(facingDirection), 
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

        private void ThrowProjectile(List<Projectile> projectilesIn)
        {
            if (projectiles.Count > 0)
            {
                Projectile projectile = projectiles.Pop();
                Console.WriteLine($"Santa throwing {projectile.tag}");

                // Move out of santa's body
                float nudge = 5.0f;
                projectile.body.position = body.position + 
                    facingDirection * 
                    (body.radius + projectile.body.radius + nudge);
                // Launch projectile forward
                projectile.Fly(facingDirection);
                // Add projectile to top-level -simulation
                projectilesIn.Add(projectile);

                // Increase santa's speed from weight loss if elf
                if (projectile.tag == Tag.Elf)
                {
                    speed += speed <= 300f ? 25f : 0f;
                }
            }
            else
            {
                Console.WriteLine("Santa's out of projectiles");
                //playErrorSound();
            }
        }
    }
}
