using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Joulurauhaa2020
{
    /// <summary>
    /// Collection of gameobject-events to do, when specific collisions occur
    /// </summary>
    public static class Collisions
    {
        public static void Handle(Santa santa, Elf elf, List<Elf> toBeRemoved)
        {
            if (elf.alive)
            {
                if (toBeRemoved == null) // Stupid flag for melee vs pickup
                {
                    elf.Hurt(santa.meleeDamage);
                }
                else
                {
                    santa.AddProjectile(elf.AsProjectile());
                    // Set elf to be removed because it's attached to player
                    toBeRemoved.Add(elf);
                }
            }
        }
 
        public static void Handle(Santa santa, Projectile projectile)
        {
            if (projectile.flying)
            {
                if (projectile.tag == Tag.Elf && projectile.bounced)
                {
                    santa.AddProjectile(projectile);
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
                // Push away from each other
                elf1.direction = Vector2.Reflect(
                    elf1.direction, 
                    Vector2.Normalize(elf1.body.position - elf2.body.position));
                elf2.direction = Vector2.Reflect(
                    elf2.direction, 
                    Vector2.Normalize(elf2.body.position - elf1.body.position));
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

        public static void Handle(Elf elf, Projectile projectile)
        {
            if (elf.alive)
            {
                if (projectile.flying)
                {
                    projectile.Bounce(Vector2.Normalize(
                        projectile.body.position - elf.body.position));
                    elf.Die(); 
                }
                else if (projectile.tag == Tag.Elf)
                {
                    elf.SlowDown();
                }
                else if (projectile.tag == Tag.Bottle)
                {
                    // Walking over broken bottle hurts
                    // NOTE Set to 1 because I can't be arsed to implement
                    // hitbox timers and such
                    //elf.health = 1;
                    elf.Die();
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
            projectile1.Bounce(
                projectile1.body.position - projectile2.body.position);
            projectile2.Bounce(
                projectile2.body.position - projectile1.body.position);
        }
 
        public static void Handle(Projectile projectile, Wall wall)
        {
            projectile.Bounce(wall.direction);
        }
    }
}
