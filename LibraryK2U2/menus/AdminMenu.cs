using LibraryK2U2.helpers;
using LibraryK2U2.infrastructure;
using LibraryK2U2.services;

namespace LibraryK2U2.menus
{
    public class AdminMenu
    {
        private readonly AuthService auth;
        private readonly AdminLibraryMenu libraryMenu = new();

        public AdminMenu(AuthService authService)
        {
            auth = authService;
        }

        private void DrawUserAdminMenu()
        {
            var userMenu = new AdminUserMenu(auth);
            userMenu.DrawUI();
        }

        public void DrawUI()
        {
            new MenuBuilder("ADMIN MENU")
                .Add("Library system", libraryMenu.DrawUI)
                .Add("Administration", DrawAdminSections)
                .Exit("Exit system", ConsoleHelper.ConfirmLogout)
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
                .Add("Block member", memberService.BlockMember)
                .Add("Unblock member", memberService.UnblockMember)
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
                .Add("Historical overdue loans", loanService.ShowHistoricalOverdueLoans)
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
    }
}