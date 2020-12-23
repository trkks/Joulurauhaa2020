using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Joulurauhaa2020
{
    /// <summary>
    /// Collection of gameobject-events to do, when specific collisions occur
    /// </summary>
    public static class Collisions
    {
        public static void Handle(Santa santa, Elf elf, List<Elf> toBeRemoved,
                                  ref uint points)
        {
            if (elf.alive)
            {
                if (toBeRemoved == null) // NOTE Stupid flag for melee vs pickup
                {
                    bool died = elf.Hurt(santa.meleeDamage);
                    if (died)
                    {
                        points += 20;
                    }
                    Console.WriteLine($"MELEE HIT: {santa.meleeDamage}");
                }
                else
                {
                    if (santa.AddProjectile(elf.AsProjectile()))
                    {
                        // Set elf to be removed, as it's attached to player
                        toBeRemoved.Add(elf);
                    }
                }
            }
        }
 
        public static void Handle(Santa santa, Projectile projectile,
                                  List<Projectile> toBeRemoved)
        {
            //Console.WriteLine($"Santa to {projectile.tag} collision");
            if (projectile.StateIs(Projectile.State.Pickup))
            {
                if (santa.AddProjectile(projectile))
                {
                    toBeRemoved.Add(projectile);
                }
            }
        }
   
        public static void Handle(Santa santa, Wall wall)
        {
            wall.PushAway(santa.body);
        }

        public static void Handle(Elf elf1, Elf elf2)
        {
            if (elf1.alive && elf2.alive)
            {
                // TODO Fix elflerp -> set direction to reflect here
                Vector2 away = Vector2.Normalize(
                    elf1.body.position - elf2.body.position) * 
                    (elf1.body.radius);
                // Push away from the other
                elf1.body.position += away;
            }
            else if (elf1.alive && !elf2.alive)
            {
                elf1.SlowDown();
            }
            else if (!elf1.alive && elf2.alive)
            {
                elf2.SlowDown();
            }
        }

        public static void Handle(Elf elf, Projectile projectile, 
                                  ref uint points)
        {
            //System.Console.WriteLine("Collision: Elf to projectile");
            if (elf.alive)
            {
                if (projectile.StateIs(Projectile.State.Flying))
                {
                    projectile.Bounce(Vector2.Normalize(
                        projectile.body.position - elf.body.position));
                    bool died = elf.Hurt(projectile.damage);
                    if (died)
                    {
                        points += 50;
                    }
                    //elf.Die(); 
                }
                else if (projectile.tag == Tag.Elf)
                {
                    elf.SlowDown();
                }
                else if (projectile.tag == Tag.Bottle && 
                         projectile.StateIs(Projectile.State.Broken))
                {
                    // Broken bottles hurt elves once when walking over
                    //NOTE Set to 1 cos I cant be arsed with the time I have rn
                    elf.Health = 1;
                }
            }
        }

        public static void Handle(Elf elf, Wall wall)
        {
            wall.PushAway(elf.body);
        }

        public static void Handle(Projectile projectile1,
                                  Projectile projectile2)
        {
            //System.Console.WriteLine("Collision: Projectile to projectile");
            projectile1.Bounce(
                projectile1.body.position - projectile2.body.position);
            projectile2.Bounce(
                projectile2.body.position - projectile1.body.position);
        }
 
        public static void Handle(Projectile projectile, Wall wall)
        {
            //System.Console.WriteLine("Collision: Projectile to wall");
            if (projectile.StateIs(Projectile.State.Flying))
            {
                projectile.Bounce(wall.direction);
            }
            else
            {
                wall.PushAway(projectile.body);
            }
        }
    }
}
