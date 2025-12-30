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
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Library Management System";

            // LibraryK2U2.dev.DbConnectionTest.Run();
            IntroScreen.Show();

            var userRepository = new JsonUserRepository();
            var auth = new AuthService(userRepository);

            while (true)
            {
                var user = auth.Login();

                if (user == null)
                    continue;

                if (user.IsAdmin())
                    new AdminMenu(auth).DrawUI();
                else
                    new Menu().DrawUI();
            }
        }
    }
}
