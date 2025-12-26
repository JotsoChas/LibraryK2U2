using LibraryK2U2.data;
using LibraryK2U2.helpers;
using LibraryK2U2.models;

namespace LibraryK2U2.services
{
    public class MemberService
    {
        public void RegisterMember()
        {
            ConsoleHelper.WriteHeader("Register New Member");

            var firstName = ConsoleHelper.ReadInput("First name");
            var lastName = ConsoleHelper.ReadInput("Last name");

            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName))
            {
                ConsoleHelper.Warning("First name and last name are required");
                ConsoleHelper.Pause();
                return;
            }

            using var db = new LibraryDBContext();

            var member = new Member
            {
                FirstName = firstName,
                LastName = lastName
            };

            db.Members.Add(member);
            db.SaveChanges();

            ConsoleHelper.Success("Member registered successfully");
            ConsoleHelper.Pause();
        }
    }
}
