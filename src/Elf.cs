using System;
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

        // updateAction kinda mimicking the strategy pattern
        public Action<GameTime> updateAction;
        public Color color;
        public Rectangle spriteRect;
        public Texture2D spriteAtlas;
        public Vector2 direction;
        public Vector2 spriteOrigin;

        public CircleBody Body { get; set; }

        public Elf(Vector2 position, Texture2D spriteAtlas)
        {
            this.Body = new CircleBody(128, position);
            this.updateAction = gameTime => { return; };
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
            if (!this.Body.isActive) 
                return;

            switch (target)
            {
            case Santa santa:
                if (this.Body.Colliding(santa.Body))
                {
                    //System.Console.WriteLine("Elf collision to Santa");
                    this.color = Color.LightGreen;

                    // Do not check for collisions when attached to santa
                    this.Body.isActive = false;
                    this.updateAction = CreateAttach(santa, this.Body.position);
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
                //System.Console.WriteLine($"{this} at {target}");
                break;
            };
        }

        public void Update(GameTime gameTime)
        {
            this.updateAction(gameTime);
        }

        /// <summary>
        /// Create an action that updates elf's position according to collPos
        /// so that the elf seems to be attached to santa
        /// </summary>
        private Action<GameTime> CreateAttach(Santa santa, Vector2 collPos)
        {
            var v = Vector2.Normalize(this.Body.position - santa.Body.position);
            v *= santa.Body.radius;
            return (gameTime) => {
                //Console.WriteLine($"Elf attached at {santa.Body.position + v}");
                // TODO Attach to santa where first collided with correct angle
                // something change to santa's basis something matrix transform
                this.Body.position = santa.Body.position + v;
            };
        }
    }
}
