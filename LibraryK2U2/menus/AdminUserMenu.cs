using LibraryK2U2.helpers;
using LibraryK2U2.services;

public class AdminUserMenu
{
    private readonly UserService userService;

    public AdminUserMenu(AuthService auth)
    {
        userService = new UserService(auth);
    }

    public void DrawUI()
    {
        new MenuBuilder("USERS")
            .Add("List users", userService.ListUsers)
            .Add("Create user", userService.CreateUser)
            .Add("Unlock user", userService.UnlockUser)
            .Add("Reset PIN", userService.ResetPin)
            .Add("Delete user", userService.DeleteUser)
            .Back("Back")
            .Run();
    }
}