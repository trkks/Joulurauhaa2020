﻿using Microsoft.Xna.Framework;
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

        private static Random random = new Random();
        private static Vector2 playerStartPosition = new Vector2(400, 400);
        //private static Timer playtime; //TODO use gametime.TotalGameTime?
        private static Texture2D elfTexture;

        // Primitive fields
        private int elfSpawnRate = 600;
        private int screenWidth;
        private int screenHeight;

        // Object fields
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GraphicsDevice device; // "The hardware graphical device"

        // "Cosmetic" objects
        private Texture2D floorTexture;

        // Gameobjects
        private List<Elf> elves;
        private Santa player;
        private List<Projectile> projectiles;
        private Wall[] walls;
        private List<object> deadObjects;

        public GameJR2020()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Backbuffer contains what will be drawn to screen
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Joulurauhaa2020";
            base.Initialize();
        }

        private void SpawnElf()
        {
            // Choose a door
            // Play the door's animation
            // Spawn an elf at the door
            var elfPosition = new Vector2(
                (float)random.NextDouble() * (float)screenWidth,
                (float)random.NextDouble() * (float)screenHeight
            );

            elves.Add(new Elf(elfPosition, elfTexture));
        }

        protected override void LoadContent()
        {
            // General fields:
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;
            screenWidth = device.PresentationParameters.BackBufferWidth;
            screenHeight = device.PresentationParameters.BackBufferHeight;
            //elfSpawner.Elapsed += (s,e) => SpawnElf();
            //elfSpawner.AutoReset = true;
            //elfSpawner.Enabled = true;

            // Global visuals:
            floorTexture = Content.Load<Texture2D>("floor");

            // Game objects:
            //Texture2D bottleTexture = Content.Load<Texture2D>("bottle");
            player = new Santa(playerStartPosition, 
                Content.Load<Texture2D>("santa_atlas")
            );

            elves = new List<Elf>(128);
            elfTexture = Content.Load<Texture2D>("elf_atlas");

            // Make some debug elves
            for (int i = 0; i < 10; ++i)
            {
                SpawnElf();
            }

            var wallHorizontal = Content.Load<Texture2D>("wall_horizontal");
            var wallVertical = Content.Load<Texture2D>("wall_vertical");

            float wallThickness = 10f;
            walls = new Wall[] {
                new Wall( // Left
                    wallVertical,
                    new Vector2(wallThickness, (float)screenHeight),
                    Vector2.UnitX,
                    Vector2.Zero
                ),
                new Wall( // Right
                    wallVertical,
                    new Vector2(wallThickness, (float)screenHeight),
                    -Vector2.UnitX,
                    new Vector2((float)screenWidth-wallThickness, 0)
                ),
                new Wall( // Top
                    wallHorizontal,
                    new Vector2((float)screenWidth, wallThickness),
                    Vector2.UnitY,
                    Vector2.Zero
                ),
                new Wall( // Bottom
                    wallHorizontal,
                    new Vector2((float)screenWidth, wallThickness),
                    -Vector2.UnitY,
                    new Vector2(0, (float)screenHeight-wallThickness)
                )
            };

            projectiles = new List<Projectile>(32);

            deadObjects = new List<object>(128);
        }

        protected override void Update(GameTime gameTime)
        {
            // Spawn new elves 
            if (gameTime.TotalGameTime.Milliseconds % elfSpawnRate == 0)
            {
                SpawnElf();
            }

            // Handle UI-specific controls
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update all updatables:

            player.Update(deltaTime, projectiles);

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
            var removableElves = new List<Elf>(8);

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
                    Collisions.Handle(player, elf, removableElves);
                }

                if (elf.body.Colliding(player.melee))
                {
                    Console.WriteLine("Melee hit!!");
                    Collisions.Handle(player, elf, null);
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
                        Collisions.Handle(elf, projectile);
                    }
                }

                if (projectile.body.Colliding(player.body))
                {
                    Collisions.Handle(player, projectile);
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
            // NOTE this is stupid
            elves.RemoveAll(elf => removableElves.Contains(elf));

            // Check for gameover
            if (!player.alive)
            {
                GameOver();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //FIXME Unstable draw-sorting makes dead objects flicker
            spriteBatch.Begin(SpriteSortMode.FrontToBack);


            spriteBatch.Draw(
                floorTexture,
                Vector2.Zero,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0
            );
            
            foreach (Elf elf in elves)
            {
                elf.Draw(spriteBatch);
            }

            player.Draw(spriteBatch);

            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }

            foreach (Wall wall in walls)
            {
                wall.Draw(spriteBatch);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void GameOver()
        {
            //TODO
            Console.WriteLine("Game over.");
        }
    }
}
