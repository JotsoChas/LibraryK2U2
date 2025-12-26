using System;

namespace Joco.ConsoleKit.Helpers
{
    public static class InputHelper
    {
        // Reads input, ESC returns null (acts as Back)
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
