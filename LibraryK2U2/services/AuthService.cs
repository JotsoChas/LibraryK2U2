using System;
using System.Collections.Generic;
using System.Linq;
using LibraryK2U2.helpers;
using LibraryK2U2.interfaces;
using LibraryK2U2.models;

namespace LibraryK2U2.services
{
    public class AuthService
    {
        private readonly IUserRepository repo;

        // Inject user repository
        public AuthService(IUserRepository repository)
        {
            repo = repository;
        }

        // Handles the full login flow
        public User? Login()
        {
            while (true)
            {
                ConsoleHelper.WriteHeader("LOGIN");
                ShowLoginHeader();

                int consoleWidth = Console.WindowWidth;
                int formWidth = "Username: ".Length + 10;
                int leftPadding = Math.Max((consoleWidth - formWidth) / 2, 0);

                Console.SetCursorPosition(leftPadding, Console.CursorTop);

                // Read username with ESC support
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Username");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                var username = ConsoleHelper.ReadInputWithBack("");
                Console.ResetColor();

                if (username == null)
                    return null;

                // Fetch user from repository
                var user = repo.Get(username);

                if (user == null)
                {
                    ConsoleHelper.Error("User not found");
                    ConsoleHelper.Pause();
                    continue;
                }

                // Blocked users cannot proceed
                if (user.IsBlocked)
                {
                    ShowBlockedScreen(user);
                    return null;
                }

                // PIN validation loop
                while (user.FailedAttempts < user.MaxAttempts)
                {
                    Console.SetCursorPosition(leftPadding, Console.CursorTop);
                    Console.Write(new string(' ', 30));
                    Console.SetCursorPosition(leftPadding, Console.CursorTop);

                    var pin = ReadPin("PIN");

                    if (user.ValidatePin(pin))
                    {
                        user.ResetAttempts();
                        repo.Update(user);
                        repo.Save();

                        Console.CursorVisible = false;

                        Console.WriteLine();
                        Console.SetCursorPosition(leftPadding, Console.CursorTop);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"✔ Welcome {user.DisplayName}");
                        Console.ResetColor();

                        Console.WriteLine();
                        Console.SetCursorPosition(leftPadding, Console.CursorTop);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("Press ENTER to proceed");
                        Console.ResetColor();

                        Console.ReadKey(true);
                        Console.CursorVisible = true;

                        return user;
                    }

                    user.FailedAttempts++;
                    repo.Update(user);
                    repo.Save();

                    int left = user.MaxAttempts - user.FailedAttempts;
                    Console.WriteLine();
                    Console.SetCursorPosition(leftPadding, Console.CursorTop);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚠  Wrong PIN ({left} attempts left)");
                    Console.ResetColor();
                }

                // Lock account after max attempts
                repo.Save();
                ShowBlockedScreen(user);
                return null;
            }
        }

        // Registers a new user account
        public void RegisterUser(bool isAdmin = false)
        {
            while (true)
            {
                ConsoleHelper.WriteHeader("REGISTER USER");

                var username = ConsoleHelper.ReadInputWithBack("Username");
                if (username == null)
                    return;

                if (string.IsNullOrWhiteSpace(username))
                {
                    ConsoleHelper.Warning("Username cannot be empty");
                    ConsoleHelper.Pause();
                    continue;
                }

                // Prevent duplicate usernames
                if (repo.Get(username) != null)
                {
                    ConsoleHelper.Error("User already exists");
                    ConsoleHelper.Pause();
                    continue;
                }

                var displayName = ConsoleHelper.ReadInputWithBack("Display name");
                if (displayName == null)
                    return;

                var pin = ReadPin("PIN (4 digits)");

                // Validate PIN format
                if (pin.Length != 4 || !pin.All(char.IsDigit))
                {
                    ConsoleHelper.Warning("PIN must be exactly 4 digits");
                    ConsoleHelper.Pause();
                    continue;
                }

                repo.Add(new User(username, displayName, pin, isAdmin));
                repo.Save();

                ConsoleHelper.Success("User created");
                ConsoleHelper.Pause();
                return;
            }
        }

        // Returns all users (admin use)
        public List<User> GetAllUsers() => repo.GetAll();

        public enum UnlockUserResult
        {
            UserNotFound,
            NotBlocked,
            Unlocked
        }

        // Unlocks a blocked user account
        public UnlockUserResult UnlockUser(string username)
        {
            var user = repo.Get(username);
            if (user == null)
                return UnlockUserResult.UserNotFound;

            if (!user.IsBlocked)
                return UnlockUserResult.NotBlocked;

            user.ResetAttempts();
            repo.Update(user);
            repo.Save();

            return UnlockUserResult.Unlocked;
        }

        // Resets PIN for non-admin users
        public bool ResetPin(string username, string newPin)
        {
            var user = repo.Get(username);
            if (user == null)
                return false;

            if (user.IsAdmin())
                return false;

            user.PIN = newPin;
            user.ResetAttempts();
            repo.Update(user);
            repo.Save();
            return true;
        }

        // Deletes a non-admin user
        public bool DeleteUser(string username)
        {
            var user = repo.Get(username);
            if (user == null)
                return false;

            if (user.IsAdmin())
                return false;

            repo.Delete(username);
            repo.Save();
            return true;
        }

        // Draws centered login ASCII header
        private void ShowLoginHeader()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;

            string[] logo =
            {
                "██╗      ██████╗  ██████╗ ██╗███╗   ██╗",
                "██║     ██╔═══██╗██╔════╝ ██║████╗  ██║",
                "██║     ██║   ██║██║  ███╗██║██╔██╗ ██║",
                "██║     ██║   ██║██║   ██║██║██║╚██╗██║",
                "███████╗╚██████╔╝╚██████╔╝██║██║ ╚████║",
                "╚══════╝ ╚═════╝  ╚═════╝ ╚═╝╚═╝  ╚═══╝"
            };

            int consoleWidth = Console.WindowWidth;
            int logoWidth = logo[0].Length;
            int leftPadding = Math.Max((consoleWidth - logoWidth) / 2, 0);

            foreach (var line in logo)
            {
                Console.WriteLine(new string(' ', leftPadding) + line);
            }

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Gray;

            string subtitle1 = "Library Management System";
            string subtitle2 = "Secure access required";

            Console.WriteLine(new string(' ', (consoleWidth - subtitle1.Length) / 2) + subtitle1);
            Console.WriteLine(new string(' ', (consoleWidth - subtitle2.Length) / 2) + subtitle2);

            Console.WriteLine();
            Console.ResetColor();
        }

        // Displays locked account information
        private void ShowBlockedScreen(User user)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.CursorVisible = false;

            int consoleWidth = Console.WindowWidth;
            int consoleHeight = Console.WindowHeight;

            string[] lines =
            {
                "ACCOUNT LOCKED",
                "",
                $"User '{user.Username}' is blocked",
                "",
                "Too many incorrect PIN attempts.",
                "Contact an administrator to unlock the account.",
                "",
                "Phone : 0200 112 233",
                "Email : support@libraryAdmin.com",
                "Hours : Mon-Fri 08-17",
                "",
                "After your account has been unlocked by an administrator,",
                "restart the application for the change to take effect.",
                "",
                "You cannot log in until the account is unlocked."
            };

            int blockHeight = lines.Length + 2;
            int startTop = Math.Max((consoleHeight - blockHeight) / 2, 0);

            Console.SetCursorPosition(0, startTop);

            foreach (var line in lines)
            {
                int leftPadding = Math.Max((consoleWidth - line.Length) / 2, 0);

                Console.SetCursorPosition(leftPadding, Console.CursorTop);

                if (line.Contains("User"))
                    ConsoleHelper.Warning(line);
                else if (line.Contains("You cannot"))
                    ConsoleHelper.Warning(line);
                else if (line.Contains("Too many") || line.Contains("Contact"))
                    ConsoleHelper.Info(line);
                else
                    Console.WriteLine(line);

            }
            Console.ReadKey();
            Console.CursorVisible = true;
        }

        // Reads masked PIN input
        private string ReadPin(string label)
        {
            Console.Write($"{label}: ");
            var pin = string.Empty;

            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return pin;
                }

                if (key.Key == ConsoleKey.Backspace && pin.Length > 0)
                {
                    pin = pin[..^1];
                    Console.Write("\b \b");
                    continue;
                }

                if (char.IsDigit(key.KeyChar))
                {
                    pin += key.KeyChar;
                    Console.Write("*");
                }
            }
        }
    }
}
