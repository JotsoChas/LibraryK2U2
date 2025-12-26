using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryK2U2.helpers
{
    public static class ConsoleHelper
    {
        // ===============================
        // HEADER / TITLES
        // ===============================

        public static void WriteHeader(string title)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n========== {title.ToUpper()} ==========\n");
            Console.ResetColor();
        }


        // ===============================
        // STATUS MESSAGES
        // ===============================

        public static void Success(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✔ {msg}");
            Console.ResetColor();
        }

        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✖ {msg}");
            Console.ResetColor();
        }

        public static void Warning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ {msg}");
            Console.ResetColor();
        }

        public static void Info(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"ℹ {msg}");
            Console.ResetColor();
        }

        public static void Print(string msg)
        {
            Console.WriteLine(msg);
        }


        // ===============================
        // INPUT (PLAIN)
        // ===============================

        public static string ReadInput(string label)
        {
            Console.Write($"{label}: ");
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        // ESC returns "<ESC>" instead of text
        public static string ReadInputEsc(string label)
        {
            Console.Write($"{label} (ESC to cancel): ");
            var input = Console.ReadLine();

            return input?.ToUpper() == "ESC"
                ? "<ESC>"
                : input?.Trim() ?? string.Empty;
        }


        // ===============================
        // INPUT – PIN WITH MASK + NUMPAD
        // ===============================

        // Reads PIN as 4 digits, masked with '*'
        // Works with top-row digits and NumPad (NumLock on)
        public static string ReadPinMasked(string label)
        {
            Console.Write($"{label}: ");
            var input = string.Empty;

            while (true)
            {
                var key = Console.ReadKey(true);

                // Accept digits from both main row and NumPad
                if (char.IsDigit(key.KeyChar))
                {
                    if (input.Length < 4)
                    {
                        input += key.KeyChar;
                        Console.Write("*");
                    }
                }
                // Backspace support
                else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                    Console.Write("\b \b");
                }
                // Enter finishes input
                else if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input;
                }
            }
        }


        // ===============================
        // PAUSE FUNCTIONS
        // ===============================

        public static void Pause()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(true);
        }

        public static void Pause(string msg)
        {
            Console.WriteLine($"\n{msg}");
            Console.ReadKey(true);
        }


        // ===============================
        // TABLE PRINTING (ADMIN LIST USERS)
        // ===============================

        public static void PrintTable(string[] headers, params string[][] rows)
        {
            int[] widths = new int[headers.Length];

            for (int i = 0; i < headers.Length; i++)
            {
                widths[i] = Math.Max(
                    headers[i].Length,
                    rows.Length > 0 ? rows.Max(r => r[i].Length) : 0
                );
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;

            for (int i = 0; i < headers.Length; i++)
                Console.Write(headers[i].PadRight(widths[i] + 4));

            Console.ResetColor();
            Console.WriteLine("\n" + new string('-', widths.Sum() + widths.Length * 4));

            foreach (var row in rows)
            {
                for (int i = 0; i < row.Length; i++)
                    Console.Write(row[i].PadRight(widths[i] + 4));
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        // ===============================
        // EXTRA UI ELEMENTS
        // ===============================

        // Clear screen safely
        public static void ClearScreen() => Console.Clear();

        public static void PauseWithMessage(string msg = "Press any key...")
        {
            Console.WriteLine($"\n{msg}");
            Console.ReadKey(true);
        }

        // Draw a centered info box
        public static void WriteBox(string text, ConsoleColor color = ConsoleColor.White)
        {
            int width = text.Length + 6;
            string line = new string('■', width);

            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.WriteLine($"■  {text}  ■");
            Console.WriteLine(line);
            Console.ResetColor();
        }

        // Highlight section headings
        public static void WriteHighlight(string text, ConsoleColor color = ConsoleColor.Yellow)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($">> {text}");
            Console.ResetColor();
        }

    }
}
