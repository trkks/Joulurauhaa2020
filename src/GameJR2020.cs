using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Joulurauhaa2020
{
    public class GameJR2020 : Game
    {
        // Primitive fields
        private int screenWidth;
        private int screenHeight;

        // Object fields
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GraphicsDevice device; // "The hardware graphical device"
        private static Random random = new Random();
        private static Vector2 playerStartPosition = new Vector2(400, 400);

        // Gameobjects
        private List<Elf> elves;
        private Wall[] walls;
        private Santa player;

        private List<ICollidable> collidables;
        private List<IDrawable> drawables;
        private List<IUpdatable> updatables;

        public GameJR2020()
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

            player = new Santa(playerStartPosition, 
                Content.Load<Texture2D>("santa_atlas")
                //Content.Load<Texture2D>("white_square_and_circle")
            );

            elves = new List<Elf>(100);

            for (int i = 0; i < 10; ++i)
            {
                var elfPosition = new Vector2(
                    (float)random.NextDouble() * (float)screenWidth,
                    (float)random.NextDouble() * (float)screenHeight
                );

                elves.Add(new Elf(elfPosition,
                        Content.Load<Texture2D>("elf_atlas"))
                );
            }

            var wallTexture = Content.Load<Texture2D>("debug_white_square");//wall");
            walls = new Wall[] {
                new Wall(Wall.Edge.Left),
                new Wall(Wall.Edge.Right),
                new Wall(Wall.Edge.Top),
                new Wall(Wall.Edge.Bottom)
            };

            // Simplify updates by adding into generic collections
            
            collidables = new List<ICollidable>(105);
            collidables.Add(player);
            collidables.AddRange(elves);
            collidables.AddRange(walls);

            drawables = new List<IDrawable>(105);
            drawables.Add(player);
            drawables.AddRange(elves);
            drawables.AddRange(walls);

            updatables = new List<IUpdatable>(101);
            updatables.Add(player);
            updatables.AddRange(elves);
        }

        protected override void Update(GameTime gameTime)
        {
            // Handle UI-specific controls
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Update all updatables
            foreach (IUpdatable updatable in updatables)
            {
                updatable.Update(deltaTime);
            }

            // Check collisions between all collidables
            // O(n^2)
            foreach (ICollidable collidable in collidables)
            {
                foreach (ICollidable target in collidables)
                {
                    if (collidable != target)
                    {
                        collidable.ResolveIfColliding(target);
                    }
                }
            }
           
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            
            foreach (IDrawable drawable in drawables)
            {
                drawable.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
