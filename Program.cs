using System;

namespace LD10
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameContainer game = new GameContainer()) {
                game.Run();
            }
        }
    }
}

