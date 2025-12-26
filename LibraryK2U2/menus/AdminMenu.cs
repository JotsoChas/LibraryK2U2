using LibraryK2U2.helpers;
using LibraryK2U2.infrastructure;
using LibraryK2U2.services;

namespace LibraryK2U2.menus
{
    public class AdminMenu
    {
        private readonly Menu libraryMenu = new();

        private void DrawUserAdminMenu()
        {
            var authService = new AuthService(new JsonUserRepository());
            var userMenu = new AdminUserMenu(authService);
            userMenu.DrawUI();
        }

        public void DrawUI()
        {
            new MenuBuilder("ADMIN MENU")
                .Add("Library system", libraryMenu.DrawUI)
                .Add("Administration", DrawAdminSections)
                .Exit("Exit system", ConfirmLogout)
                .Run();
        }

        private void DrawAdminSections()
        {
            new MenuBuilder("ADMINISTRATION")
                .Add("Books", DrawBookMenu)
                .Add("Members", DrawMemberMenu)
                .Add("Loans", DrawLoanMenu)
                .Add("Statistics", DrawStatisticsMenu)
                .Add("User administration", DrawUserAdminMenu)
                .Back("Back")
                .Run();
        }

        private void DrawBookMenu()
        {
            var bookService = new BookService();

            new MenuBuilder("BOOKS")
                .Add("List books", bookService.ListAllBooks)
                .Add("Delete book", bookService.DeleteBook)
                .Add("Rename category", bookService.RenameCategory)
                .Add("Delete category", bookService.DeleteCategory)
                .Back("Back")
                .CloseAfterSelection()
                .Run();
        }

        private void DrawMemberMenu()
        {
            var memberService = new MemberService();

            new MenuBuilder("MEMBERS")
                .Add("List all members", memberService.ListAllMembers)
                .Add("Edit member", memberService.EditMember)
                .Add("Delete member", memberService.DeleteMember)
                .Back("Back")
                .CloseAfterSelection()
                .Run();
        }

        private void DrawLoanMenu()
        {
            var loanService = new LoanService();

            new MenuBuilder("LOANS")
                .Add("Show all loans", loanService.ShowAllLoans)
                .Add("Force return loan", loanService.ForceReturn)
                .Add("Change due date", loanService.ChangeDueDate)
                .Add("Blacklist (overdue loans)", loanService.ShowBlacklist)
                .Back("Back")
                .CloseAfterSelection()
                .Run();
        }

        private void DrawStatisticsMenu()
        {
            var bookService = new BookService();

            new MenuBuilder("STATISTICS")
                .Add("Category overview", bookService.ShowCategoryStatistics)
                .Add("Never borrowed books", bookService.ShowNeverBorrowedBooks)
                .Add("Most borrowed books", bookService.ShowMostBorrowedBooks)
                .Back("Back")
                .CloseAfterSelection()
                .Run();
        }

        private void ExitApplication()
        {
            ExitScreen.Show();
            Environment.Exit(0);
        }

        // Confirms logout before exiting
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
    }
}
