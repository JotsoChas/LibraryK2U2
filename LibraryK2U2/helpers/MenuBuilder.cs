using System;
using System.Collections.Generic;
using LibraryK2U2.helpers;

namespace LibraryK2U2.helpers
{
    public class MenuBuilder
    {
        private readonly string title;
        private readonly List<(string Label, Action Action)> items = new();

        private (string Label, Action? Action)? back;
        private (string Label, Action? Action)? exit;

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

        public MenuBuilder Exit(string label = "Exit", Action? action = null)
        {
            exit = (label, action);
            return this;
        }

        public void Run()
        {
            while (true)
            {
                ConsoleHelper.WriteHeader(title);

                for (int i = 0; i < items.Count; i++)
                    Console.WriteLine($"{i + 1}) {items[i].Label}");

                if (back.HasValue) Console.WriteLine($"B) {back.Value.Label}");
                if (exit.HasValue) Console.WriteLine($"X) {exit.Value.Label}");

                Console.WriteLine();

                var key = Console.ReadKey(true);

                if (char.IsDigit(key.KeyChar))
                {
                    int idx = key.KeyChar - '1';
                    if (idx >= 0 && idx < items.Count)
                    {
                        items[idx].Action.Invoke();
                        continue;
                    }
                }

                if (key.Key == ConsoleKey.B && back.HasValue)
                {
                    back.Value.Action?.Invoke();
                    return;
                }

                if (key.Key == ConsoleKey.X && exit.HasValue)
                {
                    exit.Value.Action?.Invoke();
                    Environment.Exit(0);
                }

                ConsoleHelper.Warning("Invalid option");
                ConsoleHelper.Pause();
            }
        }
    }
}
