using LibraryK2U2.helpers;
using System.Linq;
using static LibraryK2U2.services.AuthService;

namespace LibraryK2U2.services
{
    internal class UserService
    {
        private readonly AuthService auth;

        public UserService(AuthService authService)
        {
            auth = authService;
        }

        public void ListUsers()
        {
            ConsoleHelper.WriteHeader("ALL USERS");

            var users = auth.GetAllUsers()
                .OrderBy(u => u.Username)
                .ToList();

            if (!users.Any())
            {
                ConsoleHelper.Info("No users found");
                ConsoleHelper.Pause();
                return;
            }

            var headers = new[]
            {
                "Username",
                "Display name",
                "Role",
                "Status",
                "Failed attempts"
            };

            var rows = users.Select(u => new[]
            {
                u.Username,
                u.DisplayName,
                u.IsAdmin() ? "Admin" : "User",
                u.IsBlocked ? "Blocked" : "Active",
                $"{u.FailedAttempts}/{u.MaxAttempts}"
            }).ToArray();

            ConsoleHelper.PrintTable(headers, rows);
            ConsoleHelper.Pause();
        }

        public void CreateUser()
        {
            ConsoleHelper.WriteHeader("CREATE USER");
            auth.RegisterUser(isAdmin: false);
        }

        public void UnlockUser()
        {
            ConsoleHelper.WriteHeader("UNLOCK USER");

            var username = ConsoleHelper.ReadInputWithBack("Username");
            if (username == null)
                return;

            if (!ConsoleHelper.Confirm($"Are you sure you want to unlock user '{username}'"))
            {
                ConsoleHelper.Info("Operation cancelled");
                ConsoleHelper.Pause();
                return;
            }

            var result = auth.UnlockUser(username);

            switch (result)
            {
                case UnlockUserResult.Unlocked:
                    ConsoleHelper.Success("User unlocked");
                    break;
                case UnlockUserResult.NotBlocked:
                    ConsoleHelper.Info("User is not blocked");
                    break;
                case UnlockUserResult.UserNotFound:
                    ConsoleHelper.Error("User not found");
                    break;
            }

            ConsoleHelper.Pause();
        }

        public void ResetPin()
        {
            ConsoleHelper.WriteHeader("RESET PIN");

            var username = ConsoleHelper.ReadInputWithBack("Username");
            if (username == null)
                return;

            var newPin = ConsoleHelper.ReadInputWithBack("New PIN (4 digits)");
            if (newPin == null)
                return;

            if (newPin.Length != 4 || !newPin.All(char.IsDigit))
            {
                ConsoleHelper.Error("PIN must be exactly 4 digits");
                ConsoleHelper.Pause();
                return;
            }

            if (!ConsoleHelper.Confirm($"Are you sure you want to reset PIN for '{username}'"))
            {
                ConsoleHelper.Info("Operation cancelled");
                ConsoleHelper.Pause();
                return;
            }

            if (auth.ResetPin(username, newPin))
                ConsoleHelper.Success("PIN updated");
            else
                ConsoleHelper.Error("User not found");

            ConsoleHelper.Pause();
        }

        public void DeleteUser()
        {
            ConsoleHelper.WriteHeader("DELETE USER");

            var username = ConsoleHelper.ReadInputWithBack("Username");
            if (username == null)
                return;

            if (username.Equals("admin", System.StringComparison.OrdinalIgnoreCase))
            {
                ConsoleHelper.Error("The admin account cannot be deleted");
                ConsoleHelper.Pause();
                return;
            }

            if (!ConsoleHelper.Confirm($"Are you sure you want to delete user '{username}'"))
            {
                ConsoleHelper.Info("Operation cancelled");
                ConsoleHelper.Pause();
                return;
            }

            if (auth.DeleteUser(username))
                ConsoleHelper.Success("User deleted");
            else
                ConsoleHelper.Error("User not found");

            ConsoleHelper.Pause();
        }
    }
}