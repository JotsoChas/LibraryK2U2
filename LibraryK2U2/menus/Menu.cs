using LibraryK2U2.helpers;
using LibraryK2U2.services;

namespace LibraryK2U2.menus
{
    public class Menu
    {
        private readonly BookService bookService = new();
        private readonly MemberService memberService = new();
        private readonly LoanService loanService = new();

        public void DrawUI()
        {
            new MenuBuilder("LIBRARY SYSTEM")
                .Add("Register new book", bookService.RegisterBook)
                .Add("Register new member", memberService.RegisterMember)
                .Add("Register loan", loanService.RegisterLoan)
                .Add("Register return", loanService.RegisterReturn)
                .Add("Show active loans", loanService.ShowActiveLoans)
                .Add("Search books", bookService.SearchBooks)
                .Exit("Exit system", ConfirmLogout)
                .Run();
        }

        // Confirms logout before exit
        private bool ConfirmLogout()
        {
            Console.Clear();

            if (!ConsoleHelper.Confirm("Are you sure you want to log out"))
            {
                Console.Clear();
                return false;
            }

            ExitApplication();
            return true;
        }

        private void ExitApplication()
        {
            ExitScreen.Show();
            Environment.Exit(0);
        }
    }
}
