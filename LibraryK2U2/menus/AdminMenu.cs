using LibraryK2U2.helpers;
using LibraryK2U2.services;

namespace LibraryK2U2.menus
{
    public class AdminMenu
    {
        private readonly AdminService adminService = new();

        public void DrawUI()
        {
            bool running = true;

            while (running)
            {
                new MenuBuilder("ADMIN MENU")
                    .Add("Show all loans", adminService.ShowAllLoans)
                    .Add("Force return loan", adminService.ForceReturn)
                    .Add("Change due date", adminService.ChangeDueDate)
                    .Add("Delete book", adminService.DeleteBook)
                    .Add("Delete member", adminService.DeleteMember)
                    .Back("Back", () => running = false)
                    .Run();
            }
        }
    }
}
