using LibraryK2U2.infrastructure;
using LibraryK2U2.interfaces;
using LibraryK2U2.models;
using LibraryK2U2.services;

namespace LibraryK2U2.helpers
{
    public static class MenuHelper
    {
        private static User? currentUser;

        private static readonly AuthService auth =
            new AuthService(new JsonUserRepository());

        // Entry point menu
        public static void MainMenu()
        {
            while (true)
            {
                currentUser = null;

                new MenuBuilder("WELCOME")
                    .Add("Log in", Login)
                    .Exit("Exit program")
                    .Run();
            }
        }

        private static void Login()
        {
            currentUser = auth.Login();
            if (currentUser == null)
                return;

            ShowRoleMenu();
        }

        // Routes user based on role
        private static void ShowRoleMenu()
        {
            if (currentUser!.IsAdmin())
                AdminMenu();
            else
                UserMenu();
        }

        private static void UserMenu()
        {
            new MenuBuilder($"USER PANEL ({currentUser!.DisplayName})")
                .Add("Daily tasks", DailyTasksMenu)
                .Back("Logout", Logout)
                .Exit("Exit application")
                .Run();
        }

        private static void AdminMenu()
        {
            new MenuBuilder($"ADMIN PANEL ({currentUser!.DisplayName})")
                .Add("Daily tasks", DailyTasksMenu)
                .Add("User management", UserManagementMenu)
                .Back("Logout", Logout)
                .Exit("Exit application")
                .Run();
        }

        // Shared daily tasks menu
        private static void DailyTasksMenu()
        {
            new MenuBuilder("DAILY TASKS")
                .Add("Register loan", () => ConsoleHelper.Info("Hook to LoanService.RegisterLoan"))
                .Add("Register return", () => ConsoleHelper.Info("Hook to LoanService.RegisterReturn"))
                .Add("Show active loans", () => ConsoleHelper.Info("Hook to LoanService.ShowActiveLoans"))
                .Add("Search books", () => ConsoleHelper.Info("Hook to BookService.SearchBooks"))
                .Back("Back")
                .Run();
        }

        // Admin user management
        private static void UserManagementMenu()
        {
            new MenuBuilder("USER MANAGEMENT")
                .Add("List users", ListUsers)
                .Add("Unlock user", UnlockUser)
                .Add("Reset PIN", ResetPin)
                .Add("Delete user", DeleteUser)
                .Back("Back")
                .Run();
        }

        private static void ListUsers()
        {
            ConsoleHelper.WriteHeader("ALL USERS");

            var users = auth.GetAllUsers();

            ConsoleHelper.PrintTable(
                new[] { "Username", "Display name", "Status" },
                users.Select(u => new[]
                {
                    u.Username,
                    u.DisplayName,
                    u.IsBlocked ? "Blocked" : "Active"
                }).ToArray()
            );

            ConsoleHelper.Pause();
        }

        private static void UnlockUser()
        {
            ConsoleHelper.WriteHeader("UNLOCK USER");

            string username = ConsoleHelper.ReadInput("Username");

            var result = auth.UnlockUser(username);

            switch (result)
            {
                case AuthService.UnlockUserResult.Unlocked:
                    ConsoleHelper.Success("User unlocked");
                    break;

                case AuthService.UnlockUserResult.NotBlocked:
                    ConsoleHelper.Info("User is not blocked");
                    break;

                case AuthService.UnlockUserResult.UserNotFound:
                    ConsoleHelper.Error("User not found");
                    break;
            }

            ConsoleHelper.Pause();
        }

        private static void ResetPin()
        {
            ConsoleHelper.WriteHeader("RESET PIN");

            string username = ConsoleHelper.ReadInput("Username");
            string newPin = ConsoleHelper.ReadInput("New PIN (4 digits)");

            if (newPin.Length != 4 || !newPin.All(char.IsDigit))
            {
                ConsoleHelper.Error("PIN must be exactly 4 digits");
                ConsoleHelper.Pause();
                return;
            }

            if (auth.ResetPin(username, newPin))
                ConsoleHelper.Success("PIN updated");
            else
                ConsoleHelper.Error("User not found");

            ConsoleHelper.Pause();
        }

        private static void DeleteUser()
        {
            ConsoleHelper.WriteHeader("DELETE USER");

            string username = ConsoleHelper.ReadInput("Username");

            if (username.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                ConsoleHelper.Error("The admin account cannot be deleted");
                ConsoleHelper.Pause();
                return;
            }

            if (auth.DeleteUser(username))
                ConsoleHelper.Success("User deleted");
            else
                ConsoleHelper.Error("User not found");

            ConsoleHelper.Pause();
        }

        // Clears current session
        private static void Logout()
        {
            currentUser = null;
            ConsoleHelper.Info("Logged out");
            ConsoleHelper.Pause();
        }
    }
}