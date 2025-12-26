using LibraryK2U2.helpers;
using LibraryK2U2.infrastructure;
using LibraryK2U2.menus;
using LibraryK2U2.services;
using System.Text;

namespace LibraryK2U2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Console setup
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Library Management System";

            // Dev helpers (disabled in production)
            // LibraryK2U2.dev.DbConnectionTest.Run();
            // LibraryK2U2.dev.KeyTest.Run();

            // Intro screen (optional)
            IntroScreen.Show();

            var auth = new AuthService(new JsonUserRepository());

            // Main application loop
            while (true)
            {
                var user = auth.Login();
                if (user == null)
                    continue;

                // Route user by role
                if (user.IsAdmin())
                    new AdminMenu().DrawUI();
                else
                    new Menu().DrawUI();
            }
        }
    }
}
