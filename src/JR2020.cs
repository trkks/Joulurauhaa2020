using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Joulurauhaa2020
{
    public class JR2020 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // Device is the hardware graphical device
        private GraphicsDevice device;
        private Dictionary<string, Texture2D> textures; //TODO string -> int

        private int screenWidth;
        private int screenHeight;

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

            string[] textureNames = {
                "debug_white_square", "debug_white_circle"
            };
            textures = new Dictionary<string, Texture2D>();

            foreach (var name in textureNames)
            {
                textures.Add(name, Content.Load<Texture2D>(name));
            }

            screenWidth = device.PresentationParameters.BackBufferWidth;
            screenHeight = device.PresentationParameters.BackBufferHeight;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var textureOrigin = new Vector2(128f); 
            float a = textureOrigin.Length();
            float b = new Vector2(256f).Length() + 128f;

            spriteBatch.Begin();
            
            spriteBatch.Draw(
                textures["debug_white_square"], 
                new Vector2(a),
                null, 
                Color.Red, 
                (float)(Math.PI/4.0), 
                textureOrigin, 
                1, 
                SpriteEffects.None,
                0
            );

            spriteBatch.Draw(
                textures["debug_white_circle"], 
                new Vector2(b, a),
                null, 
                Color.Blue, 
                0,
                textureOrigin,
                1, 
                SpriteEffects.None,
                0
            );

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
