using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Joulurauhaa2020
{
    public class JR2020 : Game
    {
        // Primitive fields
        private int screenWidth;
        private int screenHeight;

        // Object fields
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GraphicsDevice device; // "The hardware graphical device"
        private ElfFactory elfFactory;

        // Gameobjects
        private List<Elf> elves;
        private Wall[] walls;
        private Santa player;

        public JR2020()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Backbuffer contains what will be drawn to screen
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 750;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Joulurauhaa2020";
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;

            screenWidth = device.PresentationParameters.BackBufferWidth;
            screenHeight = device.PresentationParameters.BackBufferHeight;

            player = new Santa();

            elves = new List<Elf>(100);

            elfFactory = new ElfFactory(
                Content.Load<Texture2D>("debug_white_square") //elf_atlas")
            );
            
            var wallTexture = Content.Load<Texture2D>("debug_white_square");//wall");
            walls = new Wall[] {
                new Wall(Wall.Position.Left),
                new Wall(Wall.Position.Right),
                new Wall(Wall.Position.Top),
                new Wall(Wall.Position.Bottom)
            };
        }

        protected override void Update(GameTime gameTime)
        {
            // Handle UI-specific controls
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update(gameTime);

            foreach (Elf elf in elves)
            {
                elf.Update(gameTime);
            }

            // Check collisions between all collidables
            // O(n^2)
            foreach (Elf elf in elves)
            {
                // Check player-to-elf in same iteration
                player.ResolveIfColliding(elf); 

                elf.ResolveIfColliding(player);

                foreach (Elf target in elves)
                {
                    if (target != elf)
                    {
                        elf.ResolveIfColliding(target);
                    }
                }

                foreach (Wall target in walls)
                {
                    elf.ResolveIfColliding(target);
                }
            }
            foreach (Wall wall in walls)
            {
                player.ResolveIfColliding(wall);
            }
            // Wall-to-collidable need not be resolved, as they are static
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            
            foreach (Elf elf in elves)
            {
                elf.Draw(spriteBatch);
            }

            player.Draw(spriteBatch);

            foreach (Wall wall in walls)
            {
                wall.Draw(spriteBatch);
            }

            //var textureOrigin = new Vector2(128f); 
            //float a = textureOrigin.Length();
            //float b = new Vector2(256f).Length() + 128f;

            //spriteBatch.Draw(
            //    Content.Load<Texture2D>("debug_white_square"), 
            //    new Vector2(a),
            //    null, 
            //    Color.Red, 
            //    (float)(Math.PI/4.0), 
            //    textureOrigin, 
            //    1, 
            //    SpriteEffects.None,
            //    0
            //);

            //spriteBatch.Draw(
            //    Content.Load<Texture2D>("debug_white_circle"), 
            //    new Vector2(b, a),
            //    null, 
            //    Color.Blue, 
            //    0,
            //    textureOrigin,
            //    1, 
            //    SpriteEffects.None,
            //    0
            //);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
