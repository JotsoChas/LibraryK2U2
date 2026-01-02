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
            Console.CursorVisible = false;

            DrawLogo();

            Console.WriteLine();

            SpinnerLine("Initializing system");
            SpinnerLine("Loading modules");
            SpinnerLine("Preparing interface");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            WriteCentered("System ready ✔");
            Console.ResetColor();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteCentered("Press any key to continue");
            Console.ResetColor();

            Console.ReadKey(true);
            Console.CursorVisible = true;
        }

        // Prints centered ASCII logo
        private static void DrawLogo()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;

            string[] logo =
            {
                "██╗     ██╗██████╗ ██████╗  █████╗ ██████╗ ██╗   ██╗",
                "██║     ██║██╔══██╗██╔══██╗██╔══██╗██╔══██╗╚██╗ ██╔╝",
                "██║     ██║██████╔╝██████╔╝███████║██████╔╝ ╚████╔╝ ",
                "██║     ██║██╔══██╗██╔══██╗██╔══██║██╔══██╗  ╚██╔╝  ",
                "███████╗██║██████╔╝██║  ██║██║  ██║██║  ██║   ██║   ",
                "╚══════╝╚═╝╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝   "
            };

            int consoleWidth = Console.WindowWidth;
            int logoWidth = logo[0].Length;
            int leftPadding = Math.Max((consoleWidth - logoWidth) / 2, 0);

            foreach (var line in logo)
            {
                Console.WriteLine(new string(' ', leftPadding) + line);
            }

            Console.ResetColor();
        }

        // Shows a centered spinner animation line
        private static void SpinnerLine(string text, int durationMs = 1200)
        {
            char[] spinner = { '|', '/', '-', '\\' };
            int index = 0;
            int elapsed = 0;

            int consoleWidth = Console.WindowWidth;
            int baseLength = text.Length + 2;
            int leftPadding = Math.Max((consoleWidth - baseLength) / 2, 0);

            while (elapsed < durationMs)
            {
                Console.Write("\r" + new string(' ', leftPadding) +
                              text + " " + spinner[index++ % spinner.Length]);
                Thread.Sleep(120);
                elapsed += 120;
            }

            Console.WriteLine("\r" + new string(' ', leftPadding) + text + " ✔");
        }

        // Helper for centered single-line text
        private static void WriteCentered(string text)
        {
            int consoleWidth = Console.WindowWidth;
            int leftPadding = Math.Max((consoleWidth - text.Length) / 2, 0);
            Console.WriteLine(new string(' ', leftPadding) + text);
        }
    }
}
