using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Joulurauhaa2020
{
    public class AnimatedTexture2D
    {
        public Color color;

        private bool animating;
        private bool playingOnce;
        private float scale;

        private Rectangle spriteRect;
        private Texture2D spriteAtlas;
        private Vector2 spriteOrigin;

        public AnimatedTexture2D(Texture2D spriteAtlas, Vector2 dimensions,
                                 Vector2 origin, 
                                 Color? color=null, float scale=1f)
        {
            this.spriteAtlas = spriteAtlas;
            this.spriteRect = new Rectangle(Point.Zero, dimensions.ToPoint());
            this.spriteOrigin = origin;
            this.color = color ?? Color.White;
            this.scale = scale;

            this.animating = false;
            this.playingOnce = false;
        }

        // TODO Add GameTime as a parameter
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float angle)
        {
            // TODO Add timings of each animation frame
            if (animating)
            {
                // Decide from where on atlas to render the rectangle
                // NOTE This means that a sprite atlas needs to be a rectangle!
                int newFrameX = spriteRect.X + spriteRect.Width;
                if (newFrameX > spriteAtlas.Width)
                {
                    // Reset column and move by one row

                    int newFrameY = spriteRect.Y + spriteRect.Height;
                    if (newFrameY > spriteAtlas.Height)
                    {
                        // Also reset row (ie. start animation from beginning)
                        spriteRect.Y = 0;

                        if (playingOnce)
                        {
                            animating = false;
                            playingOnce = false;
                        }
                    }
                    else 
                    {
                        spriteRect.Y = newFrameY;
                    }
                    spriteRect.X = 0;
                }
                else
                {
                    // Move by one column
                    spriteRect.X = newFrameX;
                }
            }
            spriteBatch.Draw(
                this.spriteAtlas,
                position,
                this.spriteRect,
                this.color,
                angle,
                this.spriteOrigin,
                this.scale,
                SpriteEffects.None,
                0
            );
        }

        public void PlayOnce()
        {
            // FIXME animation is jittery
            if (!playingOnce)
            {
                animating = true;
                playingOnce = true;
            }
        }
    }
}
