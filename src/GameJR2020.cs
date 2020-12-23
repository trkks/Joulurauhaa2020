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
        public static Color colorOfDeath = new Color(0xa9, 0xa9, 0xa9);
        public static Color colorOfHurt = Color.Red;
        public static Vector2 bottlesPosition;
        public static Random random = new Random();

        private static Vector2 sceneDimensions = new Vector2(800,600);
        private static Vector2 playerStartPosition = new Vector2(400, 400);
        private static Vector2 pointsPosition;
        //private static Timer playtime; //TODO use gametime.TotalGameTime?

        // Value-type fields
        private int elfSpawnRate = 600;
        private uint points = 0;
        private Vector2 scenePosition;
        private Vector2 overlayPosition;

        // Object fields
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GraphicsDevice device; // "The hardware graphical device"

        // "Cosmetic" objects
        private static SoundEffectInstance theme;
        private static SpriteFont font;
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
            base.Initialize();
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
            var bottlePosition = scenePosition + new Vector2(
                (float)random.NextDouble() * sceneDimensions.X,
                (float)random.NextDouble() * sceneDimensions.Y
            );

            float angle = (float)random.NextDouble()*2f-1f;

            projectiles.Add(CreateBottle(bottlePosition, angle));
        }

        private void SpawnElf()
        {
            //return;
            // Choose a (predefined?) position
            // Play spawn animation
            // Add an elf at position
            var elfPosition = scenePosition + new Vector2(
                (float)random.NextDouble() * sceneDimensions.X,
                (float)random.NextDouble() * sceneDimensions.Y
            );

            elves.Add(new Elf(elfPosition, elfTexture));
        }

        protected override void LoadContent()
        {
            // General fields:
            float wallThickness = 10f;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;
            scenePosition = new Vector2(
                 (float)device.PresentationParameters.BackBufferWidth -
                     sceneDimensions.X,
                 (float)device.PresentationParameters.BackBufferHeight -
                     sceneDimensions.Y
            ) / 2f;
                                         // - Thickness of overlay
            overlayPosition = scenePosition + new Vector2(-10); 
            pointsPosition =  scenePosition + new Vector2(20, -45);
            bottlesPosition = scenePosition + new Vector2(20, 
                                                  sceneDimensions.Y+45);

            // Global sounds:
            theme = Content.Load<SoundEffect>("theme").CreateInstance();
            theme.IsLooped = true;
            theme.Play();

            // Global visuals:
            font = Content.Load<SpriteFont>("font");
            bottleTexture = Content.Load<Texture2D>("bottle");
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

            // Make some debug elves and bottles
            for (int i = 0; i < 10; ++i)
            {
                SpawnElf();
                SpawnBottle();
            }

            walls = new Wall[] {
                new Wall( // Left
                    new Vector2(wallThickness, sceneDimensions.Y),
                    Vector2.UnitX,
                    scenePosition
                ),
                new Wall( // Right
                    new Vector2(wallThickness, sceneDimensions.Y),
                    -Vector2.UnitX,
                    scenePosition +
                        new Vector2(sceneDimensions.X - wallThickness, 0)
                ),
                new Wall( // Top
                    new Vector2(sceneDimensions.X, wallThickness),
                    Vector2.UnitY,
                    scenePosition
                ),
                new Wall( // Bottom
                    new Vector2(sceneDimensions.X, wallThickness),
                    -Vector2.UnitY,
                    scenePosition + 
                        new Vector2(0, sceneDimensions.Y - wallThickness)
                )
            };
        }

        protected override void Update(GameTime gameTime)
        {
            // Increase points from survival time
            if (gameTime.TotalGameTime.Milliseconds % 1000 == 0)
            {
                points += 5;
            }

            // Spawn new elves 
            if (gameTime.TotalGameTime.Milliseconds % elfSpawnRate == 0)
            {
                if (random.Next(10) < 1) // 10% chance for bottle
                    SpawnBottle();
                else
                    SpawnElf();
            }

            // Handle UI-specific controls
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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
                    if (elf != elf2 && elf.body.Colliding(elf2.body))
                    {
                        Collisions.Handle(elf, elf2);
                    }
                    else
                    {
                        // Reset speed to recover from Elf-to-Elf -collision
                        elf.ResetSpeed();
                    }
                }

                if (elf.body.Colliding(player.body))
                {
                    Collisions.Handle(player, elf, removableElves, ref points);
                }

                if (elf.body.Colliding(player.melee))
                {
                    // TODO if elf was hit last frame, do not go here
                    // ie. only check collision once per a "continuous event"
                    Collisions.Handle(player, elf, null, ref points);
                    elf.invincible = true;
                }
                else
                {
                    elf.invincible = false;
                }

                foreach (Wall wall in walls)
                {
                    if (elf.body.Colliding(wall.body))
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
                if (wall.body.Colliding(player.body))
                {
                    Collisions.Handle(player, wall);
                }
                foreach (Projectile projectile in projectiles)
                {
                    if (wall.body.Colliding(projectile.body))
                    {
                        Collisions.Handle(projectile, wall);
                    }
                }
            }

            // Remove elves turned into projectiles from living elves
            // NOTE this is still kinda stupid
            elves.RemoveAll(elf => removableElves.Contains(elf));
            projectiles.RemoveAll(proj => removableProjectiles.Contains(proj));

            // Check for gameover
            if (player.IsDead)
            {
                GameOver();
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

            spriteBatch.DrawString(
                font, 
                $"Joulupisteet: {points}",
                pointsPosition,
                Color.White
            );

            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void GameOver()
        {
            //TODO
            // elves.updateAction = Elf.Dance;
            //Console.WriteLine("Game over.");
        }
    }
}
