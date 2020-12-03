using System;

namespace Joulurauhaa2020
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameJR2020())
                game.Run();
        }
    }
}
