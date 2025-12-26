using System;

namespace LibraryK2U2.helpers
{
    public static class InputHelper
    {
        // Reads a single character, ESC returns null
        public static string? Read(string label)
        {
            Console.Write($"{label}: ");
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Escape)
                return null;

            Console.Write(key.KeyChar);
            Console.WriteLine();
            return key.KeyChar.ToString();
        }

        // Reads a full line of text
        public static string ReadLine(string label)
        {
            Console.Write($"{label}: ");
            return Console.ReadLine()!;
        }
    }
}
