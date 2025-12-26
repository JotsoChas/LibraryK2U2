using System;
using LibraryK2U2.auth;
using LibraryK2U2.helpers;

namespace LibraryK2U2.auth
{
    public class AuthService
    {
        private readonly IUserRepository repo;

        public AuthService(IUserRepository repository)
        {
            repo = repository;
        }


        // ==========================================
        // LOGIN
        // ==========================================
        public User? Login()
        {
            ConsoleHelper.WriteHeader("LOGIN 🔐");

            string username = ConsoleHelper.ReadInput("Username");
            var user = repo.Get(username);

            if (user == null)
            {
                ConsoleHelper.Error("User not found");
                ConsoleHelper.Pause();
                return null;
            }

            if (user.IsBlocked)
            {
                ConsoleHelper.Error("Account locked 🔒");
                ConsoleHelper.Warning("Ask admin to unlock");
                ConsoleHelper.Pause();
                return null;
            }

            while (user.FailedAttempts < user.MaxAttempts)
            {
                string pin = ConsoleHelper.ReadPinMasked("PIN");

                if (user.ValidatePin(pin))
                {
                    user.ResetAttempts();
                    repo.Update(user);
                    repo.Save();

                    ConsoleHelper.Success($"Welcome {user.DisplayName} 🎉");
                    ConsoleHelper.Pause();
                    return user;
                }

                user.FailedAttempts++;
                repo.Update(user);
                repo.Save();

                int left = user.MaxAttempts - user.FailedAttempts;
                ConsoleHelper.Warning($"Wrong PIN ({left} attempts left)");
            }

            ConsoleHelper.Error("Too many attempts — account locked 🔒");
            repo.Save();
            ShowBlockedScreen(user);
            return null;
        }


        // ==========================================
        // REGISTER
        // ==========================================
        public void Register()
        {
            ConsoleHelper.WriteHeader("REGISTER USER ➕");

            string username;
            while (true)
            {
                username = ConsoleHelper.ReadInput("Username");

                if (string.IsNullOrWhiteSpace(username))
                {
                    ConsoleHelper.Warning("Username cannot be empty");
                    continue;
                }

                if (repo.Get(username) != null)
                {
                    ConsoleHelper.Error("User already exists");
                    continue;
                }

                break;
            }

            string display = ConsoleHelper.ReadInput("Display name");

            string pin;
            while (true)
            {
                pin = ConsoleHelper.ReadPinMasked("PIN (4 digits)");

                if (pin.Length == 4)
                    break;

                ConsoleHelper.Warning("PIN must be exactly 4 digits");
            }

            repo.Add(new User(username, display, pin));
            repo.Save();

            ConsoleHelper.Success("User registered ✔");
            ConsoleHelper.Pause();
        }


        // ==========================================
        // ADMIN FEATURES
        // ==========================================

        public List<User> GetAllUsers() => repo.GetAll();

        public bool UnlockUser(string username)
        {
            var user = repo.Get(username);
            if (user == null) return false;

            user.ResetAttempts();
            repo.Update(user);
            repo.Save();
            return true;
        }

        public bool DeleteUser(string username)
        {
            if (repo.Get(username) == null)
                return false;

            repo.Delete(username);
            repo.Save();
            return true;
        }

        public bool ResetPIN(string username, string newPin)
        {
            var user = repo.Get(username);
            if (user == null) return false;                                     

            user.PIN = newPin;
            user.ResetAttempts();
            repo.Update(user);
            repo.Save();
            return true;
        }

        private void ShowBlockedScreen(User user)
        {
            ConsoleHelper.ClearScreen();
            ConsoleHelper.WriteHeader("ACCOUNT LOCKED 🔒");

            ConsoleHelper.WriteBox($"User: {user.Username}", ConsoleColor.Cyan);

            Console.WriteLine();
            ConsoleHelper.Warning("Your account has been temporarily locked");
            ConsoleHelper.Info("This happens after several incorrect PIN attempts.");

            Console.WriteLine();
            ConsoleHelper.WriteHighlight("Contact support to unlock:", ConsoleColor.Yellow);

            Console.WriteLine("• Phone: 0200 112 233");
            Console.WriteLine("• Email: support@x.com");
            Console.WriteLine("• Hours: Mon–Fri 08–18");

            Console.WriteLine();
            ConsoleHelper.Warning("For security reasons you cannot log in until unlocked.");

            ConsoleHelper.Pause("Press any key to exit...");
        }

    }
}
