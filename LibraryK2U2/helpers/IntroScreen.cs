using System;
using System.Threading;

namespace LibraryK2U2.helpers
{
    public static class IntroScreen
    {
        public static void Show()
        {
            Console.Clear();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            DrawLogo();

            Console.WriteLine();

            SpinnerLine("Initializing system");
            SpinnerLine("Loading modules");
            SpinnerLine("Preparing interface");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✔ System ready");
            Console.ResetColor();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Press any key to continue");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        // Prints ASCII logo
        private static void DrawLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine(" ██╗     ██╗██████╗ ██████╗  █████╗ ██████╗ ██╗   ██╗");
            Console.WriteLine(" ██║     ██║██╔══██╗██╔══██╗██╔══██╗██╔══██╗╚██╗ ██╔╝");
            Console.WriteLine(" ██║     ██║██████╔╝██████╔╝███████║██████╔╝ ╚████╔╝ ");
            Console.WriteLine(" ██║     ██║██╔══██╗██╔══██╗██╔══██║██╔══██╗  ╚██╔╝  ");
            Console.WriteLine(" ███████╗██║██████╔╝██║  ██║██║  ██║██║  ██║   ██║   ");
            Console.WriteLine(" ╚══════╝╚═╝╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   ");

            Console.ResetColor();
        }

        // Shows a short spinner animation
        private static void SpinnerLine(string text, int durationMs = 1200)
        {
            char[] spinner = { '|', '/', '-', '\\' };
            int index = 0;
            int elapsed = 0;

            while (elapsed < durationMs)
            {
                Console.Write($"\r{text} {spinner[index++ % spinner.Length]}");
                Thread.Sleep(120);
                elapsed += 120;
            }

            Console.WriteLine($"\r{text} ✓");
        }
    }
}
