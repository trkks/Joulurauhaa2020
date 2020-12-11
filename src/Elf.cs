using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Joulurauhaa2020
{
    public class Elf : IProjectile, ICollidable, IDrawable, IUpdatable
    {
        public bool alive;
        public float speed;

        // updateAction kinda mimicking the strategy pattern
        public Action<float> updateAction;
        public AnimatedTexture2D animation;
        public CircleBody body;
        public Vector2 direction;
        public Vector2 spriteOrigin;

        public float Angle 
        { 
            get => body.angle; 
            set => body.angle = value;
        }

        public float Speed 
        {
            get => speed;
            set => speed = value;
        }

        public Vector2 Direction 
        {
            get => direction;
            set => direction = value;
        }


        public Elf(Vector2 position, Texture2D spriteAtlas)
        {
            this.body = new CircleBody(128, position);
            this.animation = new AnimatedTexture2D(spriteAtlas, 
                new Point(32,32), new Vector2(16,16),
                new uint[4] { 5, 3, 5, 3 });
            this.updateAction = deltaTime => { return; };
            // Start animation
            this.animation.Toggle();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            this.animation.Draw(spriteBatch, this.body.position, 
                this.body.angle);

            // NOTE debug shapes:
            //spriteBatch.Draw(
            //    this.spriteAtlas,
            //    this.body.position,
            //    new Rectangle(257,0, 256,256),// spriteRect,
            //    Color.White,
            //    0f,// angle,
            //    spriteOrigin,
            //    this.body.radius/128, //scale
            //    SpriteEffects.None,
            //    0
            //); 
            //spriteBatch.Draw(
            //    this.spriteAtlas,
            //    this.body.position,
            //    new Rectangle(0,0, 256,256),// spriteRect,
            //    this.color,
            //    this.angle,
            //    this.spriteOrigin,
            //    0.5f,
            //    SpriteEffects.None,
            //    0
            //);
        }

        public void ResolveIfColliding(ICollidable target)
        { 
            if (!this.body.isActive) 
                return;

            switch (target)
            {
            case Santa santa:
                if (this.body.Colliding(santa.body))
                {
                    //System.Console.WriteLine("Elf collision to Santa");
                    this.animation.color = Color.Pink;

                    // Do not check for collisions when attached to santa
                    this.body.isActive = false;
                    // TODO difference between "X as I" and "(I)X" ???
                    //santa.projectiles.Push(this);
                    this.updateAction = CreateAttach(santa, this.body.position);
                }
                else
                {
                    this.animation.color = Color.White;
                }
                break;

            case Elf elf:
                // simple reflect? how does this affect different updateactions?
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

        public void Update(float deltaTime)
        {
            this.updateAction(deltaTime);
        }

        /// <summary>
        /// Create an action that updates elf's position according to collPos
        /// so that the elf seems to be attached to santa
        /// </summary>
        private Action<float> CreateAttach(Santa santa, Vector2 collPos)
        {
            Vector2 towardsSanta = Vector2.Normalize(
                santa.body.position - this.body.position);
            Vector2 santaOffset = -towardsSanta * santa.body.radius;

            this.body.angle = (float)Math.Atan2(towardsSanta.Y, towardsSanta.X);

            return (deltaTime) => {
                //Console.WriteLine($"Elf attached at {santa.body.position + v}");
                // TODO Attach to santa where first collided with correct angle
                // something change to santa's basis something matrix transform
                this.body.position = santa.body.position + santaOffset;
            };
        }

        public void SetDefaultUpdate()
        {
            this.updateAction = (deltaTime) => 
            {
                this.body.position += this.direction * deltaTime;

            };
        }
    }
}
