using Microsoft.Xna.Framework;

namespace Joulurauhaa2020
{
    /// <summary>
    /// Collection of gameobject-events to do, when specific collisions occur
    /// </summary>
    public static class Collisions
    {
        public static void Handle(Santa santa, Elf elf)
        {
            santa.AddProjectile(elf.AsProjectile());
            elf.Die();
        }
 
        public static void Handle(Santa santa, Projectile projectile)
        {
            santa.AddProjectile(projectile);
        }
   
        public static void Handle(Santa santa, Wall wall)
        {
            wall.PushAway(santa.body);
        }

        public static void Handle(Elf elf1, Elf elf2)
        {
            elf1.direction = Vector2.Reflect(
                elf1.direction, 
                Vector2.Normalize(elf1.body.position - elf2.body.position));
            elf2.direction = Vector2.Reflect(
                elf2.direction, 
                Vector2.Normalize(elf2.body.position - elf1.body.position));
        }

        public static void Handle(Elf elf, Projectile projectile)
        {
            elf.Die(); 
            projectile.Break();
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
