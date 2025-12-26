using System;
using System.Threading;

namespace LibraryK2U2.helpers
{
    public static class ExitScreen
    {
        public static void Show()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine(" ██╗     ██╗██████╗ ██████╗  █████╗ ██████╗ ██╗   ██╗");
            Console.WriteLine(" ██║     ██║██╔══██╗██╔══██╗██╔══██╗██╔══██╗╚██╗ ██╔╝");
            Console.WriteLine(" ██║     ██║██████╔╝██████╔╝███████║██████╔╝ ╚████╔╝ ");
            Console.WriteLine(" ██║     ██║██╔══██╗██╔══██╗██╔══██║██╔══██╗  ╚██╔╝  ");
            Console.WriteLine(" ███████╗██║██████╔╝██║  ██║██║  ██║██║  ██║   ██║   ");
            Console.WriteLine(" ╚══════╝╚═╝╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   ");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("System shutting down ");

            // Simple spinner animation
            var spinner = new[] { '/', '-', '\\', '|' };

            for (int i = 0; i < 20; i++)
            {
                Console.Write(spinner[i % spinner.Length]);
                Thread.Sleep(80);
                Console.Write('\b');
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✔ Goodbye!");
            Console.ResetColor();

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
