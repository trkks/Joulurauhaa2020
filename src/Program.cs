using System;

namespace Joulurauhaa2020
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new JR2020())
                game.Run();
        }
    }
}
