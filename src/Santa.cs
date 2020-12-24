using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Joulurauhaa2020
{
    public class Santa
    {
        public float speed;
        public int meleeDamage;
        public AnimatedTexture2D animation;
        public CircleBody body;
        public CircleBody melee;

        private bool mLeftReleased;
        private bool mRightReleased;
        private float angle;
        private AnimatedTexture2D bottleAnimation;
        private AnimatedTexture2D fistAnimation;
        private List<Projectile> simulationProjectiles;
        private Stack<Projectile> projectiles;
        private ActionSequence action;
        private ActionSequence swingAction;
        private ActionSequence throwAction;
        private Vector2 direction;
        private Vector2 facingDirection;
        
        public bool IsDead 
        {
            get => Array.FindAll(projectiles.ToArray(), p=>p.tag == Tag.Elf)
                   .Length >= 8; // MAX_ELVES
        }

        public Santa(Vector2 position, Texture2D bottleSpriteAtlas,
                     Texture2D fistSpriteAtlas, 
                     List<Projectile> simulationProjectiles)
        {
            this.angle = 0;
            this.bottleAnimation = new AnimatedTexture2D(bottleSpriteAtlas, 
                new Point(128,128), new Vector2(40,64),
                new uint[5] { 2, 4, 10, 2, 5 }, 0.75f);
            this.fistAnimation = new AnimatedTexture2D(fistSpriteAtlas, 
                new Point(128,128), new Vector2(40,64),
                new uint[5] { 2, 4, 10, 2, 5 }, 0.75f);
            this.body = new CircleBody(40, position);
            this.simulationProjectiles = simulationProjectiles;
            this.projectiles = 
                new Stack<Projectile>(8+3);// MAX_ELVES + MAX_BOTTLES
            //TODO This needs to be huge... it's weird
            this.melee = new CircleBody(60, Vector2.Zero);
            this.speed = 300;
           
            this.swingAction = SwingAction();
            this.throwAction = ThrowAction();
            this.action = ActionSequence.None;
            this.melee.active = false;
        }

        public bool AddProjectile(Projectile projectile)
        {
            bool added = false;


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
                added = true;
                // Slow down walking speed
                speed -= speed >= 50f ? 25f : 0f;

                // Push bottles back
                foreach (Projectile bottleTemp in bottlesTemp)
                {
                    projectiles.Push(bottleTemp);
                }
            }
            else if (projectile.tag == Tag.Bottle)
            {
                // Check if bottle fits into inventory
                int bottleCount = Array.FindAll(
                    projectiles.ToArray(), p=>p.tag == Tag.Bottle)
                    .Length; 
                if (bottleCount < 3) // MAX_BOTTLES
                {
                    // Push bottle on top
                    projectiles.Push(projectile);
                    added = true;
                }
            }

            if (added)
            {
                // Disable hitbox etc
                projectile.Reset();
            }

            // Update attack state according to pickup
            SetCorrectAttackState();

            return added;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch, body.position, angle);

            foreach (Projectile p in projectiles)
            {
                // Only draw elves here)
                if (p.tag == Tag.Elf)
                {
                    p.Draw(spriteBatch);
                }
            }
        }

        public void Update(float deltaTime)
        { 
            if (IsDead)
            { return; }
            // Get some needed values
            MouseState mState = Mouse.GetState();
            KeyboardState kbState = Keyboard.GetState();

            // Process key-input:

            /* Mouse 1 */
            bool swinging = mState.LeftButton == ButtonState.Pressed && 
                            mLeftReleased;
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
            
            if (!action.Active && !animation.animating)
            {
                if (swinging)
                {
                    //System.Console.WriteLine("Received input for swinging");
                    action = swingAction;
                    action.Active = true;
                }
                else if (throwing)
                {
                    //System.Console.WriteLine("Received input for throwing");
                    if (projectiles.Count > 0)
                    {
                        action = throwAction;
                        action.Active = true;
                    }
                    else
                    {
                        //Console.WriteLine("Santa's out of projectiles");
                        //playErrorSound();
                    }
                }
            }

            // Prevent movement during an action
            if (!action.Active)
            {
                // Allow moving when not swinging
                // FIXME *Everything* disappears when normalizing; 
                // possibly direction * speed going haywire?
                // Normalize the direction, 
                // so eg. abs(Up + Left) == abs(Up)
                //this.direction = Vector2.Normalize(this.direction);
                // Change position according to game time
                body.position += direction * speed * deltaTime;

                // Turn towards the mouse
                facingDirection = Vector2.Normalize(
                    mState.Position.ToVector2() - body.position);
                angle = (float)Math.Atan2(facingDirection.Y,
                                          facingDirection.X);
            }
            
            // Update/Execute current action
            action.Update();

            // Reset movement direction as to prevent "drifting"
            // TODO direction here means movement direction, when in other 
            // classes it's the facing -direction :/
            direction = Vector2.Zero;

            // Position the elf-projectiles attached to santa:
            float hangingIndex = 1f;
            float nudge = 1.2f;
            foreach (Projectile p in projectiles)
            {
                if (p.tag == Tag.Elf)
                {
                    // Rotate around santa's body
                    p.body.position = body.position + Vector2.Transform(
                        nudge * body.radius * facingDirection,
                        Matrix.CreateRotationZ(
                                hangingIndex * (float)(Math.PI / 4.0)
                        )
                    );

                    // Turn towards santa's center
                    Vector2 towardsSanta = Vector2.Normalize(
                        body.position - p.body.position);
                    p.angle = (float)Math.Atan2(towardsSanta.Y, 
                                                towardsSanta.X);

                    // Increment the rotation multiplier
                    hangingIndex += 1f;
                }
            }
        }

        public Projectile[] GetBottles()
        {
            return Array.FindAll(projectiles.ToArray(), p=>p.tag == Tag.Bottle);
        }

        /// <summary>
        /// Remove the next projectile from inventory and launch it forward
        /// </summary>
        private void LaunchProjectile()
        {
            // Remove projectile from inventory
            Projectile projectile = projectiles.Pop();
            //Console.WriteLine($"Santa throwing {projectile.tag}");

            // Move outside of santa's body
            float nudge = 5.0f;
            projectile.body.position = body.position + 
                facingDirection * 
                (body.radius + projectile.body.radius + nudge);
            // Launch projectile forward
            projectile.Fly(facingDirection);
            // Add projectile to top-level -simulation
            simulationProjectiles.Add(projectile);

            // Increase santa's speed from weight loss if elf
            if (projectile.tag == Tag.Elf)
            {
                speed += speed <= 300f ? 25f : 0f;
            }
        }

        /// <summary>
        /// Set the correct animation and damage based on inventory
        /// NOTE Could bug out if pickup during throw-action
        /// </summary>
        private void SetCorrectAttackState()
        {
            // Change animation according to current bottles in inventory
            if (projectiles.Count > 0 && projectiles.Peek().tag == Tag.Bottle)
            {
                animation = bottleAnimation;
                meleeDamage = 2; // BOTTLE_DAMAGE
            }
            else
            {
                animation = fistAnimation;
                meleeDamage = 1; // FIST_DAMAGE
            }
        }

        private ActionSequence SwingAction()
        {
            return new ActionSequence(new (uint, Action)[]{
                (0, () =>
                { 
                    //Console.WriteLine("Swing started");
                    // Start the animation
                    animation.PlayOnce();
                }),
                (5, () =>
                {
                    //Console.WriteLine("Melee activated");
                    // Position hitbox to front
                    melee.position = body.position + 
                        (facingDirection * (body.radius + melee.radius));
                    // Activate melee hitbox
                    melee.active = true;
                }),
                (8, () =>
                {
                    //Console.WriteLine("Melee disabled");
                    // Disable melee hitbox
                    melee.active = false;
                }),
                (12, () =>
                {
                    //Console.WriteLine("Swinging ended.");
                })
            });
        }

        private ActionSequence ThrowAction()
        {
            return new ActionSequence(new (uint, Action)[]{
                (0, () =>
                {
                    //Console.WriteLine("Throw started.");
                    // Set throwing animation
                    animation = fistAnimation;
                    // Start animation
                    animation.PlayOnce();
                }),
                (5, () =>
                {
                    //Console.WriteLine("Projectile launched.");
                    LaunchProjectile();
                }),
                (10, () =>
                {
                    //Console.WriteLine("Throw ended.");
                    // Set original animation
                    SetCorrectAttackState();
                })
            });
        }
    }
}
