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
        private int frameIndex;
        private uint frameTime;

        private Texture2D spriteAtlas;
        private Vector2 spriteOrigin;
        private (Rectangle, uint)[] frames;

        public AnimatedTexture2D(Texture2D spriteAtlas, Point spriteSize,
                                 Vector2 origin, uint[] timings,
                                 Color? color=null, float scale=1f)
        {
            this.color = color ?? Color.White;
            this.scale = scale;
            this.spriteAtlas = spriteAtlas;
            this.spriteOrigin = origin;

            this.animating = false;
            this.frameIndex = 0;
            this.frameTime = 0;
            this.frames = new (Rectangle, uint)[timings.Length];
            this.playingOnce = false;
            //Console.WriteLine($"Atlas bounds: {spriteAtlas.Bounds}");

            // Populate the animation frames with timings and rectangles
            var atlasMask = new Rectangle(Point.Zero, spriteSize);
            for (int i = 0; i < timings.Length; i++)
            {
                // Add rectangle (the "sprite") and its timing to collection
                this.frames[i] = (
                    new Rectangle(atlasMask.Location,atlasMask.Size),
                    timings[i]
                );                

                // Sample through the atlas by sprite size:
                // NOTE that the atlas needs to be filled left to right, top to
                // bottom in order!

                // Decide from where on atlas to pick the rectangle
                int newFrameX = atlasMask.X + atlasMask.Width;
                if (newFrameX >= spriteAtlas.Width)
                {
                    // Reset column and move by one row
                    int newFrameY = atlasMask.Y + atlasMask.Height;
                    if (newFrameY > spriteAtlas.Height)
                    {
                        // TODO This should never be reached with correct data
                        // ie. Area(spriteSize, timings.Length) == atlasSize;
                        // Throw Exception?
                        Console.WriteLine(
                            "Sampling sprite from 'outside' of atlas: "+
                            $"{newFrameY} >= {spriteAtlas.Height}");
                    }
                    else 
                    {
                        atlasMask.Y = newFrameY;
                    }
                    atlasMask.X = 0;
                }
                else
                {
                    // Move by one column
                    atlasMask.X = newFrameX;
                }
            }
            //foreach (var (r,t) in frames)
            //    Console.WriteLine($"{r} : {t}");
        }

        // TODO Add GameTime as a parameter and take it into consideration?
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float angle)
        {
            spriteBatch.Draw(
                spriteAtlas,
                position,
                frames[frameIndex].Item1,
                color,
                angle,
                spriteOrigin,
                scale,
                SpriteEffects.None,
                0
            );            

            if (animating)
            {
                //Console.WriteLine($"{this}: frameIndex = {frameIndex}, "+
                //                  $"frameTime = {frameTime}, "+
                //                  $"time set to this frame = {frames[frameIndex].Item2}");
                // Decide what to draw next time
                frameTime++;
                if (frameTime >= frames[frameIndex].Item2)
                {
                    // Current frame's timing has ended so change to next frame
                    frameIndex++;

                    // Keep frameIndex in the bounds of frames
                    if (frameIndex == frames.Length)
                    {
                        frameIndex = 0;

                        // Stop animation after one iteration
                        if (playingOnce)
                        {
                            animating = false;
                            playingOnce = false;
                        }
                    }
                    // Reset frameTime
                    frameTime = 0;
                }
            }
        }

        public void Toggle()
        {
            animating = !animating;
        }

        public void PlayOnce()
        {
            if (!animating)
            {
                animating = true;
                playingOnce = true;
            }
        }
    }
}
