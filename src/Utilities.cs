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
            (Rectangle, uint)[] targetCollection)
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

        /* Helpers for checking collisions */

        public static bool Colliding(CircleBody[] cbs, CircleBody cb)
        {
            foreach (CircleBody cbi in cbs)
            {
                if (cbi.Colliding(cb))
                {
                    return true;
                }
            }
            return false;
        }    

        public static bool Colliding(CircleBody[] cbs, RectangleBody rb)
        {
            foreach (CircleBody cb in cbs)
            {
                if (cb.Colliding(rb))
                {
                    return true;
                }
            }
            return false;
        }    

        public static bool CollidingRectangleCircle(Vector2 rpos, Vector2 rdim,
                                                    Vector2 cpos, float crad)
        {
            // Check for closest rectangle edge to circle
            float testX = cpos.X;
            float testY = cpos.Y;
            if (cpos.X < rpos.X) // Left
            {
                testX = rpos.X;
            }
            else if (cpos.X > rpos.X+rdim.X) // Right
            {
                testX = rpos.X+rdim.X;
            }

            if (cpos.Y < rpos.Y) // Top
            {
                testY = rpos.Y;
            }
            else if (cpos.Y > rpos.Y+rdim.Y) // Bottom
            {
                testY = rpos.Y+rdim.Y;
            }

            // Collision-check
            var testVector = new Vector2(testX, testY);
            float distance = (cpos-testVector).Length();

            return distance <= crad;
        }

        public static bool CollidingCircles(Vector2 c1pos, float c1rad, 
                                            Vector2 c2pos, float c2rad)
        {
            return Vector2.Distance(c1pos, c2pos) 
                <= c1rad + c2rad;
        }

        public static bool CollidingRectangles(Vector2 r1pos, Vector2 r1dim,
                                               Vector2 r2pos, Vector2 r2dim)
        {
            return r1pos.X + r1dim.X >= r2pos.X &&
                   r1pos.Y + r1dim.Y >= r2pos.Y &&
                   r1pos.X <= r2pos.X + r2dim.X &&
                   r1pos.Y <= r2pos.Y + r2dim.Y; 
        }
    }
}
