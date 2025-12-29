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

        public AuthService(IUserRepository repository)
        {
            repo = repository;
        }

        // Handles user login
        public User? Login()
        {
            while (true)
            {
                ConsoleHelper.WriteHeader("LOGIN");

                var username = ConsoleHelper.ReadInputWithBack("Username");
                if (username == null)
                    return null;

                var user = repo.Get(username);

                if (user == null)
                {
                    ConsoleHelper.Error("User not found");
                    ConsoleHelper.Pause();
                    continue;
                }

                if (user.IsBlocked)
                {
                    ShowBlockedScreen(user);
                    return null;
                }

                while (user.FailedAttempts < user.MaxAttempts)
                {
                    var pin = ReadPin("PIN");

                    if (user.ValidatePin(pin))
                    {
                        user.ResetAttempts();
                        repo.Update(user);
                        repo.Save();

                        ConsoleHelper.Success($"Welcome {user.DisplayName}");
                        ConsoleHelper.Pause();
                        return user;
                    }

                    user.FailedAttempts++;
                    repo.Update(user);
                    repo.Save();

                    int left = user.MaxAttempts - user.FailedAttempts;
                    ConsoleHelper.Warning($"Wrong PIN ({left} attempts left)");
                }

                repo.Save();
                ShowBlockedScreen(user);
                return null;
            }
        }

        // Registers a new user
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

        // Admin helpers
        public List<User> GetAllUsers() => repo.GetAll();

        public enum UnlockUserResult
        {
            UserNotFound,
            NotBlocked,
            Unlocked
        }

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

        // Shows blocked account information
        private void ShowBlockedScreen(User user)
        {
            Console.Clear();
            ConsoleHelper.WriteHeader("ACCOUNT LOCKED");

            Console.WriteLine();
            ConsoleHelper.Warning($"User '{user.Username}' is blocked");

            Console.WriteLine();
            ConsoleHelper.Info("Too many incorrect PIN attempts.");
            ConsoleHelper.Info("Contact an administrator to unlock the account.");

            Console.WriteLine();
            Console.WriteLine("Phone : 0200 112 233");
            Console.WriteLine("Email : support@libraryAdmin.com");
            Console.WriteLine("Hours : Mon-Fri 08-17");

            Console.WriteLine();
            ConsoleHelper.Info(
                "After your account has been unlocked by an administrator,\n" +
                "restart the application for the change to take effect."
            );

            Console.WriteLine();
            ConsoleHelper.Warning("You cannot log in until the account is unlocked.");

            ConsoleHelper.Pause();
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
