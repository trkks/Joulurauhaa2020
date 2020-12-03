using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Joulurauhaa2020
{
    public class Elf : ICollidable, IDrawable, IUpdatable
    {
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

        public Elf(Vector2 position, Texture2D spriteAtlas)
        {
            this.Body = new CircleBody(128, position);
            this.color = Color.Green;
            this.spriteRect = new Rectangle(0,0, 256,256);
            this.spriteAtlas = spriteAtlas;
            this.spriteOrigin = new Vector2(128, 128);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                this.spriteAtlas,
                this.Body.position,
                new Rectangle(257,0, 256,256),// spriteRect,
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
                this.angle,
                this.spriteOrigin,
                0.5f,
                SpriteEffects.None,
                0
            );
        }

        public void ResolveIfColliding(ICollidable target)
        { 
            switch (target)
            {
            case Santa santa:
                if (this.Body.Colliding(santa.Body))
                {
                    System.Console.WriteLine("Elf collision to Santa");
                    this.color = Color.LightGreen;

                    // TODO maybe google for a more efficient way for this?
                    do
                    {
                        this.Body.position -= Vector2.Normalize( 
                            santa.Body.position - this.Body.position);

                    } while (this.Body.Colliding(santa.Body));
                }
                else
                {
                    this.color = Color.Green;
                }
                break;

            case Wall wall:
                /* do nothing */
                break;

            default:
                /* Actual default implementation not needed */
                System.Console.WriteLine($"{this} at {target}");
                break;
            };
        }

        public void Update(GameTime gameTime)
        {
            var mState = Mouse.GetState();
            
            if (mState.RightButton == ButtonState.Pressed)
            this.Body.position = new Vector2((float)mState.Position.X, 
                                             (float)mState.Position.Y);
        }
    }
}
