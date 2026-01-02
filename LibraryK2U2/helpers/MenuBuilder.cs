using System;
using System.Collections.Generic;

namespace LibraryK2U2.helpers
{
    public class MenuBuilder
    {
        private readonly string title;
        private readonly List<(string Label, Action Action)> items = new();

        private (string Label, Action? Action)? back;
        private (string Label, Func<bool>? Action)? exit;

        private bool closeAfterSelection = false;

        public MenuBuilder(string title)
        {
            this.title = title;
        }

        public MenuBuilder Add(string label, Action action)
        {
            items.Add((label, action));
            return this;
        }

        public MenuBuilder Back(string label = "Back", Action? action = null)
        {
            back = (label, action);
            return this;
        }

        public MenuBuilder Exit(string label = "Exit", Func<bool>? action = null)
        {
            exit = (label, action);
            return this;
        }

        public MenuBuilder CloseAfterSelection()
        {
            closeAfterSelection = true;
            return this;
        }

        // Clears buffered key input
        private void ClearKeyBuffer()
        {
            while (Console.KeyAvailable)
                Console.ReadKey(true);
        }

        // Checks console bounds
        private bool CanWriteAt(int top)
        {
            return top >= 0 && top < Console.BufferHeight;
        }

        // Draws a single menu row
        private void WriteMenuLine(int top, int index, bool selected)
        {
            if (!CanWriteAt(top))
                return;

            Console.SetCursorPosition(0, top);

            int width = Console.WindowWidth > 0 ? Console.WindowWidth : 120;
            Console.Write(new string(' ', width - 1));
            Console.SetCursorPosition(0, top);

            var parts = items[index].Label.Split(" by ", StringSplitOptions.RemoveEmptyEntries);
            string titlePart = parts[0];
            string authorPart = parts.Length > 1 ? parts[1] : string.Empty;

            var indexText = (index + 1).ToString().PadLeft(2);

            if (selected)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"> {indexText}) {titlePart}");
            }
            else
            {
                Console.ResetColor();
                Console.Write($"  {indexText}) {titlePart}");
            }

            if (!string.IsNullOrWhiteSpace(authorPart))
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"   by {authorPart}");
            }

            Console.ResetColor();
        }

        public void Run()
        {
            ClearKeyBuffer();
            Console.CursorVisible = false;

            int selectedIndex = 0;

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n========== {title.ToUpper()} ==========\n");
            Console.ResetColor();

            int menuTop = Console.CursorTop;

            for (int i = 0; i < items.Count; i++)
                WriteMenuLine(menuTop + i, i, i == selectedIndex);

            DrawFooter(menuTop);

            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                {
                    Console.CursorVisible = true;

                    if (exit.HasValue)
                    {
                        var shouldExit = exit.Value.Action?.Invoke() ?? true;
                        if (shouldExit)
                            return;

                        Console.CursorVisible = false;
                        RedrawMenu(ref menuTop, selectedIndex);
                        continue;
                    }

                    back?.Action?.Invoke();
                    return;
                }

                if (key.Key == ConsoleKey.UpArrow)
                {
                    int previous = selectedIndex;
                    selectedIndex = (selectedIndex - 1 + items.Count) % items.Count;

                    WriteMenuLine(menuTop + previous, previous, false);
                    WriteMenuLine(menuTop + selectedIndex, selectedIndex, true);
                    continue;
                }

                if (key.Key == ConsoleKey.DownArrow)
                {
                    int previous = selectedIndex;
                    selectedIndex = (selectedIndex + 1) % items.Count;

                    WriteMenuLine(menuTop + previous, previous, false);
                    WriteMenuLine(menuTop + selectedIndex, selectedIndex, true);
                    continue;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.CursorVisible = true;
                    items[selectedIndex].Action.Invoke();

                    if (closeAfterSelection)
                        return;

                    selectedIndex = 0;
                    Console.CursorVisible = false;

                    RedrawMenu(ref menuTop, selectedIndex);
                }

                if (key.KeyChar >= '1' && key.KeyChar <= '9')
                {
                    int idx = key.KeyChar - '1';
                    if (idx >= 0 && idx < items.Count)
                    {
                        Console.CursorVisible = true;
                        items[idx].Action.Invoke();

                        if (closeAfterSelection)
                            return;

                        selectedIndex = 0;
                        Console.CursorVisible = false;

                        RedrawMenu(ref menuTop, selectedIndex);
                    }
                }
            }
        }

        // Redraws menu after action
        private void RedrawMenu(ref int menuTop, int selectedIndex)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n========== {title.ToUpper()} ==========\n");
            Console.ResetColor();

            menuTop = Console.CursorTop;

            for (int i = 0; i < items.Count; i++)
                WriteMenuLine(menuTop + i, i, i == selectedIndex);

            DrawFooter(menuTop);
        }

        // Draws footer text
        private void DrawFooter(int menuTop)
        {
            int footerTop = menuTop + items.Count + 1;

            if (!CanWriteAt(footerTop))
                return;

            Console.SetCursorPosition(0, footerTop);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("--------------------------------------");
            Console.ResetColor();

            if (back.HasValue)
                Console.WriteLine("ESC) Back");

            if (exit.HasValue)
                Console.WriteLine("ESC) Exit");
        }
    }
}
