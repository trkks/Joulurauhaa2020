using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Joulurauhaa2020
{
    public static class Utilities
    {
        /* Populate the animation frames with rectangles and timings */
        public static void PopulateAnimation(Point spriteSize, 
            Point atlasSize, uint[] timings,
            ref (Rectangle, uint)[] targetCollection)
        {
            var atlasMask = new Rectangle(Point.Zero, spriteSize);
            for (int i = 0; i < timings.Length; i++)
            {
                // Add rectangle (the "sprite") and its timing to collection
                targetCollection[i] = (
                    new Rectangle(atlasMask.Location, atlasMask.Size),
                    timings[i]
                );                

                // Sample through the atlas by sprite size:
                // NOTE that the atlas needs to be filled left to right, top to
                // bottom in order!

                // Decide from where on atlas to pick the rectangle
                int newFrameX = atlasMask.X + atlasMask.Width;
                if (newFrameX >= atlasSize.X)
                {
                    // Reset column and move by one row
                    int newFrameY = atlasMask.Y + atlasMask.Height;
                    if (newFrameY > atlasSize.Y)
                    {
                        // TODO This should never be reached with correct data
                        // ie. Area(spriteSize, timings.Length) == atlasSize;
                        // Throw Exception?
                        Console.WriteLine(
                            "Sampling sprite from 'outside' of atlas: "+
                            $"{newFrameY} >= {atlasSize.Y}");
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
        }
    }
}
