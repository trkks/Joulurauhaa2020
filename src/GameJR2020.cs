using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Joulurauhaa2020
{
    public class GameJR2020 : Game
    {
        // Global static fields
        public static Color colorOfDeath = Color.Gray;
        public static Color colorOfHurt = new Color(0xaa, 0xff, 0x55);
        public static Random random = new Random();
        public static Vector2 bottlesPosition;
        public static SoundEffect elfGrab;
        public static SoundEffect elfBoink;
        public static SoundEffect bottleBreak;
        public static SoundEffect[] bottleHits;
        public static SoundEffect[] fistHits;
        public static SoundEffect[] bottlePickups;
        
        private static string helpText = "<Esc> to quit and R to restart";
        private static Vector2 sceneDimensions = new Vector2(800,600);
        private static Vector2 playerStartPosition;
        private static Vector2 pointsPosition;
        private static Vector2 helpPosition;
        private static Vector2[] spawnPositions;

        // Value-type fields
        private bool gameover = false;
        private bool restarted = false;
        private int spawnRate = 600;
        private int spawnThreshold = 500;
        private uint points;
        private string pointsText;
        private Vector2 scenePosition;
        private Vector2 overlayPosition;

        // Object fields
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GraphicsDevice device; // "The hardware graphical device"

        // "Cosmetic" objects
        private static SoundEffectInstance theme;
        private static SpriteFont font;
        private static Texture2D crosshairTexture;
        private static Texture2D floorTexture;
        private static Texture2D elfTexture;
        private static Texture2D bottleTexture;
        private static Texture2D overlayTexture;
        private static Texture2D wallsTexture;


        // Gameobjects
        private List<Elf> elves;
        private Santa player;
        private List<Projectile> projectiles;
        private Wall[] walls;

        public GameJR2020()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Backbuffer contains what will be drawn to screen
            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            Window.Title = "Joulurauhaa2020";
            IsMouseVisible = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // General fields:
            float wallThickness = 50f;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;
            scenePosition = new Vector2(
                 (float)device.PresentationParameters.BackBufferWidth -
                     sceneDimensions.X,
                 (float)device.PresentationParameters.BackBufferHeight -
                     sceneDimensions.Y
            ) / 2f;

            playerStartPosition = scenePosition +
                new Vector2(400, 300);
            overlayPosition = scenePosition +
                new Vector2(-60); 
            pointsPosition = scenePosition + 
                new Vector2(20, -45);
            helpPosition = scenePosition +
                new Vector2(20, sceneDimensions.Y + 20);
            bottlesPosition = scenePosition +
                new Vector2(sceneDimensions.X - 20, sceneDimensions.Y + 40);

            spawnPositions = new Vector2[] {
                // Left
                scenePosition + new Vector2(0 + 16, 300 + random.Next(30)),
                scenePosition + new Vector2(0 + 16, 300 + random.Next(30)),
                scenePosition + new Vector2(0 + 16, 300 + random.Next(30)),
                // Right
                scenePosition + new Vector2(800 - 16, 300 + random.Next(30)),
                scenePosition + new Vector2(800 - 16, 300 + random.Next(30)),
                scenePosition + new Vector2(800 - 16, 300 + random.Next(30)),
                // Top
                scenePosition + new Vector2(400 + random.Next(30), 0 + 16),
                scenePosition + new Vector2(400 + random.Next(30), 0 + 16),
                scenePosition + new Vector2(400 + random.Next(30), 0 + 16)
            };

            // Global sounds:
            bottleHits = new SoundEffect[] {
                Content.Load<SoundEffect>("bottlehit1"),
                Content.Load<SoundEffect>("bottlehit2"),
                Content.Load<SoundEffect>("bottlehit3")
            };
            fistHits = new SoundEffect[] {
                Content.Load<SoundEffect>("punch1"),
                Content.Load<SoundEffect>("punch2"),
                Content.Load<SoundEffect>("punch3")
            };
            bottlePickups = new SoundEffect[] {
                Content.Load<SoundEffect>("drink1"),
                Content.Load<SoundEffect>("drink2"),
                Content.Load<SoundEffect>("drink3")
            };
            bottleBreak = Content.Load<SoundEffect>("glassbreak");
            elfBoink = Content.Load<SoundEffect>("boink");
            elfGrab = Content.Load<SoundEffect>("elfGrab");
            theme = Content.Load<SoundEffect>("theme").CreateInstance();
            theme.IsLooped = true;
            theme.Play();

            // Global visuals:
            font = Content.Load<SpriteFont>("font");
            bottleTexture = Content.Load<Texture2D>("bottle");
            crosshairTexture = Content.Load<Texture2D>("crosshair");
            floorTexture = Content.Load<Texture2D>("floor");
            elfTexture = Content.Load<Texture2D>("elf_atlas");
            overlayTexture = Content.Load<Texture2D>("overlay");
            wallsTexture = Content.Load<Texture2D>("walls");

            // Game objects:

            projectiles = new List<Projectile>(32);

            player = new Santa(playerStartPosition, 
                Content.Load<Texture2D>("santaBottle_atlas"),
                Content.Load<Texture2D>("santaFist_atlas"),
                projectiles
            );
            player.AddProjectile(CreateBottle());

            elves = new List<Elf>(128);

            walls = new Wall[] {
                new Wall( // Left
                    new Vector2(wallThickness, sceneDimensions.Y),
                    Vector2.UnitX,
                    scenePosition + 
                        new Vector2(-wallThickness + 10, 0)
                ),
                new Wall( // Right
                    new Vector2(wallThickness, sceneDimensions.Y),
                    -Vector2.UnitX,
                    scenePosition +
                        new Vector2(sceneDimensions.X - 10, 0)
                ),
                new Wall( // Top
                    new Vector2(sceneDimensions.X, wallThickness),
                    Vector2.UnitY,
                    scenePosition + 
                        new Vector2(0, -wallThickness + 10)
                ),
                new Wall( // Bottom
                    new Vector2(sceneDimensions.X, wallThickness),
                    -Vector2.UnitY,
                    scenePosition + 
                        new Vector2(0, sceneDimensions.Y - 10)
                )
            };
        }

        protected override void Update(GameTime gameTime)
        {
            // Handle UI-specific controls
            var kbstate = Keyboard.GetState();
            if (kbstate.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if (kbstate.IsKeyDown(Keys.R))
            {
                if (!restarted)
                {
                    Restart();
                }
            }
            restarted = !kbstate.IsKeyUp(Keys.R);

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update all updatables:

            player.Update(deltaTime);

            foreach (Projectile projectile in projectiles)
            {
                projectile.Update(deltaTime);
            }

            // Elves' update depends on player, so they're updated after
            foreach (Elf elf in elves)
            {
                if (elf.alive)
                {
                    elf.Update(deltaTime, player);
                }
            }

            // Store elves ready for removal when they collide with player
            // NOTE this is stupid
            var removableElves = new List<Elf>(8);
            var removableProjectiles = new List<Projectile>(projectiles.Count);

            // Handle collisions between collidables
            foreach (Elf elf in elves)
            {
                foreach (Elf elf2 in elves)
                {
                    if (elf != elf2)
                    {
                        if (elf.body.Colliding(elf2.body))
                        {
                            Collisions.Handle(elf, elf2);
                        }
                        else
                        {
                            // Reset speed to recover from Elf-to-Elf-collision
                            elf.ResetSpeed();
                        }
                    }
                }

                if (elf.body.Colliding(player.body))
                {
                    Collisions.Handle(player, elf, removableElves, ref points);
                }

                if (elf.body.Colliding(player.melee))
                {
                    // Only check melee collision once per "continuous event"
                    if (!elf.invincible)
                    {
                        Collisions.Handle(player, elf, null, ref points);
                        elf.invincible = true;
                    }
                }
                else
                {
                    elf.invincible = false;
                }

                foreach (Wall wall in walls)
                {
                    if (wall.Colliding(elf.body))
                    {
                        Collisions.Handle(elf, wall);
                    }
                }
            }

            foreach (Projectile projectile in projectiles)
            {
                foreach (Elf elf in elves)
                {
                    if (projectile.body.Colliding(elf.body))
                    {
                        Collisions.Handle(elf, projectile, ref points);
                    }
                }

                if (projectile.body.Colliding(player.body))
                {
                    Collisions.Handle(player, projectile, removableProjectiles);
                }
            }

            foreach (Wall wall in walls)
            {
                if (wall.Colliding(player.body))
                {
                    Collisions.Handle(player, wall);
                }
                foreach (Projectile projectile in projectiles)
                {
                    if (wall.Colliding(projectile.body))
                    {
                        Collisions.Handle(projectile, wall);
                    }
                }
            }

            // Remove elves turned into projectiles from living elves
            // NOTE this is still kinda stupid
            elves.RemoveAll(elf => removableElves.Contains(elf));
            projectiles.RemoveAll(proj => removableProjectiles.Contains(proj));

            float bottleIndex = -1f;
            foreach (Projectile bottle in player.GetBottles())
            {
                bottle.body.position = bottlesPosition +
                    new Vector2(bottleIndex * bottle.body.radius + 5f, 0);
                bottle.angle = (float)(Math.PI/4.0);
                bottleIndex -= 1f;
            }


            // Increase points from survival time every second
            if (gameTime.TotalGameTime.Milliseconds % 1000 == 0)
            {
                points += 5;
            }

            // Check for gameover
            if (!gameover)
            {
                if (player.IsDead)
                {
                    GameOver();
                }
                else
                {
                    // Spawn new elves 
                    if ((int)(gameTime.TotalGameTime.Milliseconds) 
                        % spawnRate == 0)
                    {
                        if (random.Next(10) < 1) // 10% chance for bottle
                        {
                            SpawnBottle();
                        }
                        else
                        {
                            SpawnElf();
                        }
                    }

                    // Increase spawnRate
                    if (points > spawnThreshold)
                    {
                        spawnRate = (int)(spawnRate * 0.7);
                        if (spawnRate < 10)
                        {
                            spawnRate = 10;
                        }
                        spawnThreshold += 250;
                    }

                    // Update points normally
                   pointsText = $"Joulupisteet: {points}";
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            // Get the objects to draw in specific order without using layers
            // -> no "flickering" because of unstable sort
            var bottomElves = elves.FindAll(elf => !elf.alive);
            var topElves = elves.FindAll(elf => elf.alive);
            var bottomProjectiles = projectiles.FindAll(
                projectile => projectile.StateIs(
                    Projectile.State.Broken | Projectile.State.Pickup));
            var topProjectiles = projectiles.FindAll(
                projectile => projectile.StateIs(Projectile.State.Flying));


            spriteBatch.Begin();


            spriteBatch.Draw(
                floorTexture,
                scenePosition,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0
            );
            
            foreach (Elf bottomElf in bottomElves)
            {
                bottomElf.Draw(spriteBatch);
            }

            foreach (Projectile bottomProjectile in bottomProjectiles)
            {
                bottomProjectile.Draw(spriteBatch);
            }

            foreach (Elf elf in topElves)
            {
                elf.Draw(spriteBatch);
            }
            
            foreach (Projectile projectile in topProjectiles)
            {
                projectile.Draw(spriteBatch);
            }

            player.Draw(spriteBatch);

            spriteBatch.Draw(
                wallsTexture,
                scenePosition,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0
            );
            spriteBatch.Draw(
                overlayTexture,
                overlayPosition,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0
            );

            spriteBatch.Draw(
                crosshairTexture,
                Mouse.GetState().Position.ToVector2()-new Vector2(8),
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0
            );

            spriteBatch.DrawString(
                font, 
                pointsText,
                pointsPosition,
                Color.White
            );

            spriteBatch.DrawString(
                font, 
                helpText,
                helpPosition,
                Color.Gray
            );

            foreach (Projectile bottle in player.GetBottles())
            {
                bottle.Draw(spriteBatch);
            }


            // DEBUG
            //spriteBatch.DrawString(
            //    font,
            //    $"Elf count: {elves.Count}",
            //    new Vector2(50,50),
            //    Color.White
            //);
            //spriteBatch.DrawString(
            //    font,
            //    $"Projectile count: {projectiles.Count}",
            //    new Vector2(50,90),
            //    Color.White
            //);           
            //spriteBatch.DrawString(
            //    font,
            //    $"MS: {gameTime.TotalGameTime.Milliseconds}",
            //    new Vector2(50,130),
            //    Color.White
            //);  
            //spriteBatch.DrawString(
            //    font,
            //    $"S: {gameTime.TotalGameTime.Seconds}",
            //    new Vector2(50,170),
            //    Color.White
            //);              
            //spriteBatch.DrawString(
            //    font,
            //    $"Spawn rate: {spawnRate}",
            //    new Vector2(50,210),
            //    Color.White
            //);   
            // DEBUG
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static SoundEffectInstance GetBottleBreakSound()
        {
            return bottleBreak.CreateInstance();
        }

        public static void PlayBottleHit()
        {
            bottleHits[random.Next(bottleHits.Length)]
                .CreateInstance().Play();
        }

        public static void PlayFistHit()
        {
            fistHits[random.Next(fistHits.Length)]
                .CreateInstance().Play();
        }

        public static void PlayBottlePickup()
        {
            bottlePickups[random.Next(bottlePickups.Length)]
                .CreateInstance().Play();
        }
 
        private void GameOver()
        {
            gameover = true;
            pointsText = "Rauhallista Joulua 2020\n"+
                         $"Pisteet: {points}".PadLeft(22, ' ');
            pointsPosition = scenePosition + new Vector2(250, -80);
            //Console.WriteLine("Game over.");
        }

        private void Restart()
        {
            elves = new List<Elf>(128);
            projectiles = new List<Projectile>(32);
            player = new Santa(playerStartPosition, 
                Content.Load<Texture2D>("santaBottle_atlas"),
                Content.Load<Texture2D>("santaFist_atlas"),
                projectiles
            );
            player.AddProjectile(CreateBottle());

            theme.Stop();
            theme.Play();
            pointsPosition =  scenePosition + new Vector2(20, -45);

            spawnRate = 600;
            spawnThreshold = 500;
            points = 0;
            gameover = false;
        }

        private Projectile CreateBottle(Vector2? position=null, 
                                        float angle=0)
        {
            //TODO 0.25f == GameJR2020.DeadLayer
            var bottle = new Projectile(
                new AnimatedTexture2D(bottleTexture, 
                    new Point(44,44), new Vector2(22,22),
                    new uint[]{ 5, 2, 2 }, 0.25f),
                new CircleBody(bottleTexture.Bounds.Size.Y, 
                               position ?? Vector2.Zero),
                1500, 150, Tag.Bottle, Projectile.State.Pickup);
            bottle.angle = angle;
            return bottle;
        }

        private void SpawnBottle()
        {
            // Choose a (predefined?) position
            // Play spawn animation
            // Add a bottle at position
            var bottlePosition = 
                spawnPositions[random.Next(spawnPositions.Length)];
            float angle = (float)random.NextDouble()*2f-1f;

            projectiles.Add(CreateBottle(bottlePosition, angle));
        }

        private void SpawnElf()
        {
            // Choose a (predefined?) position
            // Play spawn animation
            // Add an elf at position
            var elfPosition = 
                spawnPositions[random.Next(spawnPositions.Length)];

            elves.Add(new Elf(elfPosition, elfTexture));
        }
    }
}
