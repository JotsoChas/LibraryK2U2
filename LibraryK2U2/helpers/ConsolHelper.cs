using System;
using System.Linq;

namespace LibraryK2U2.helpers
{
    public static class ConsoleHelper
    {
        public static void WriteHeader(string title)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n========== {title.ToUpper()} ==========\n");
            Console.ResetColor();
        }

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

        public static string ReadInput(string label)
        {
            Console.Write($"{label}: ");
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        // Waits for Enter before continuing
        public static void Pause()
        {
            while (Console.KeyAvailable)
                Console.ReadKey(true);

            Console.WriteLine("\nPress ENTER to return to the menu...");

            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    return;
            }
        }

        public static void PrintTable(string[] headers, params string[][] rows)
        {
            int columnCount = headers.Length;
            int[] widths = new int[columnCount];

            for (int i = 0; i < columnCount; i++)
            {
                widths[i] = headers[i].Length;

                foreach (var row in rows)
                {
                    if (row.Length > i && row[i] != null)
                        widths[i] = Math.Max(widths[i], row[i].Length);
                }
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;

            for (int i = 0; i < columnCount; i++)
                Console.Write(headers[i].PadRight(widths[i] + 4));

            Console.ResetColor();
            Console.WriteLine("\n" + new string('-', widths.Sum() + columnCount * 4));

            foreach (var row in rows)
            {
                for (int i = 0; i < columnCount; i++)
                {
                    string cell = row.Length > i ? row[i] ?? "" : "";
                    Console.Write(cell.PadRight(widths[i] + 4));
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        // Allows ESC to cancel input
        public static string? ReadInputWithBack(string label)
        {
            Console.Write($"{label}: ");
            var input = string.Empty;

            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine();
                    return null;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input.Trim();
                }

                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                    Console.Write("\b \b");
                    continue;
                }

                if (!char.IsControl(key.KeyChar))
                {
                    input += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }
        }

        public static string FormatBookMenuRow(
            int bookId,
            string title,
            string author)
        {
            const int menuOffset = 6;
            const int idWidth = 4;
            const int titleWidth = 35;

            var safeTitle = title.Length > titleWidth
                ? title[..(titleWidth - 1)] + "…"
                : title;

            return
                new string(' ', menuOffset) +
                bookId.ToString().PadRight(idWidth) +
                safeTitle.PadRight(titleWidth) +
                author;
        }

        public static string FormatMemberMenuRow(
            int memberId,
            string firstName,
            string lastName)
        {
            const int menuOffset = 6;
            const int idWidth = 4;
            const int firstNameWidth = 15;

            return
                new string(' ', menuOffset) +
                memberId.ToString().PadRight(idWidth) +
                firstName.PadRight(firstNameWidth) +
                lastName;
        }

        public static string FormatLoanMenuRow(
            int loanId,
            string firstName,
            string lastName,
            string bookTitle,
            DateOnly dueDate)
        {
            const int menuOffset = 6;
            const int idWidth = 4;
            const int nameWidth = 22;
            const int bookWidth = 45;

            var name = $"{firstName} {lastName}";
            if (name.Length > nameWidth)
                name = name[..(nameWidth - 1)] + "…";

            var book = bookTitle.Length > bookWidth
                ? bookTitle[..(bookWidth - 1)] + "…"
                : bookTitle;

            return
                new string(' ', menuOffset) +
                loanId.ToString().PadRight(idWidth) +
                name.PadRight(nameWidth) +
                book.PadRight(bookWidth) +
                $"Due: {dueDate}";
        }

        // Requires Y or N followed by Enter
        public static bool Confirm(string message)
        {
            while (true)
            {
                Console.Write($"{message} (Y/N): ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                input = input.Trim();

                if (input.Equals("y", StringComparison.OrdinalIgnoreCase))
                    return true;

                if (input.Equals("n", StringComparison.OrdinalIgnoreCase))
                    return false;

                Console.WriteLine("Please enter Y or N and press Enter.");
            }
        }

        // Confirms logout before exit
        public static bool ConfirmLogout()
        {
            Console.Clear();

            if (!Confirm("Are you sure you want to log out"))
            {
                Console.Clear();
                return false;
            }

            return true;
        }
    }
}
