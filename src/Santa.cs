using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Joulurauhaa2020
{
    public class Santa : ICollidable, IDrawable, IUpdatable
    {
        // TODO bunch these fields up into classes for composition
        // animations and drawing  -> AnimatedSprite
        // position and collisions -> CollisionBody
        // NOTE avoid too much indirection eg.
        // CheckCollision() -> Santa -> Circle -> Body -> Circle ...
        public bool alive;
        public float speed;
        public float angle;
        public int spriteIndex;

        public Color color;
        public Rectangle spriteRect;
        public Texture2D spriteAtlas;
        public Vector2 direction;
        public Vector2 spriteOrigin;

        public CircleBody Body { get; set; }

        public Santa(Vector2 position, Texture2D spriteAtlas)
        {
            this.Body = new CircleBody(256, position);
            this.color = Color.Red;
            this.spriteRect = new Rectangle(0,0, 256,256);
            this.spriteAtlas = spriteAtlas;
            this.spriteOrigin = new Vector2(128,128);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                this.spriteAtlas,
                this.Body.position,
                new Rectangle(257,0, 256,256), // Circle
                Color.White,
                0f,// angle,
                spriteOrigin,
                this.Body.radius/128, //scale
                SpriteEffects.None,
                0
            );            
            spriteBatch.Draw(
                this.spriteAtlas,
                this.Body.position,
                new Rectangle(0,0, 256,256),// spriteRect,
                this.color,
                (float)(Math.PI/4.0),// angle,
                this.spriteOrigin,
                1f,
                SpriteEffects.None,
                0
            );
        }

        public void ResolveIfColliding(ICollidable target)
        { 
            switch (target)
            {
                case Elf elf:
                    if (this.Body.Colliding(elf.Body))
                    {
                        //System.Console.WriteLine("Santa collision to Elf");
                        this.color = Color.Pink;
                    }
                    else
                    {
                        this.color = Color.Red;
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
 
        public void Update(GameTime gameTime)
        { 
            var mState = Mouse.GetState();
            
            if (mState.LeftButton == ButtonState.Pressed)
                this.Body.position = new Vector2((float)mState.Position.X, 
                                                 (float)mState.Position.Y);
        }
    }
}
