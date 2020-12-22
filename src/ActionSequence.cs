using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Joulurauhaa2020
{
    /// <summary>
    /// Collection of actions (eg. activating hitbox, playing sound etc.)
    /// and endings of their active time as unsigned integers
    /// </summary>    
    public class ActionSequence
    {
        public static ActionSequence None => new ActionSequence();

        public bool Active { get; set; }

        private (uint, Action)[] actions;
        private int index;
        private int time;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="actions"> 
        /// Timings and their actions to be executed. 
        /// Note that times should be in ascending order and unique. 
        /// </param>
        public ActionSequence((uint, Action)[] actions=null)
        {
            // TODO Check that times are ascending and unique

            this.Active = false;
            this.actions = actions ?? new (uint, Action)[]{ (0, ()=>{}) };
            this.index = 0;
            this.time = 0;
        }

        public void Update()
        {
            if (Active)
            {
                time++;
                if (actions[index].Item1 <= time)
                {
                    actions[index].Item2();
                    index++;
                    // Sequence is executed once at each time activated
                    if (index >= actions.Length)
                    {
                        // Reset
                        Active = false;
                        index = 0;
                        time = 0;
                    }
                    //Console.WriteLine("Sequence updated: index="+ index +
                    //                  " when time == "+ time);
                }
            }
        }
    }
}
