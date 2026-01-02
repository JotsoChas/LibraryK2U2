using System;
using LibraryK2U2.data;
using LibraryK2U2.helpers;
using LibraryK2U2.models;

namespace LibraryK2U2.services
{
    public class MemberService
    {
        public void RegisterMember()
        {
            ConsoleHelper.WriteHeader("REGISTER NEW MEMBER");

            var firstName = ConsoleHelper.ReadInputWithBack("First name");
            if (firstName == null)
                return;

            if (string.IsNullOrWhiteSpace(firstName))
            {
                ConsoleHelper.Warning("First name is required");
                ConsoleHelper.Pause();
                return;
            }

            var lastName = ConsoleHelper.ReadInputWithBack("Last name");
            if (lastName == null)
                return;

            if (string.IsNullOrWhiteSpace(lastName))
            {
                ConsoleHelper.Warning("Last name is required");
                ConsoleHelper.Pause();
                return;
            }

            using var db = new LibraryDBContext();

            var member = new Member
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim()
            };

            db.Members.Add(member);
            db.SaveChanges();

            ConsoleHelper.Success(
                $"New member registered: ID {member.MemberId} - {member.FirstName} {member.LastName}"
            );

            Console.WriteLine("Press ESC to return to menu");

            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Escape)
                return;
        }

        public void DeleteMember()
        {
            ConsoleHelper.WriteHeader("DELETE MEMBER");

            using var db = new LibraryDBContext();

            var members = db.Members
                .Select(m => new
                {
                    m.MemberId,
                    Name = m.FirstName + " " + m.LastName,
                    HasActiveLoans = db.Loans.Any(l =>
                        l.MemberId == m.MemberId && l.ReturnDate == null)
                })
                .OrderBy(m => m.Name)
                .ToList();

            if (!members.Any())
            {
                ConsoleHelper.Info("No members found");
                ConsoleHelper.Pause();
                return;
            }

            // Calculate column widths for aligned menu output
            int idWidth = members.Max(m => m.MemberId.ToString().Length) + 2;
            int nameWidth = members.Max(m => m.Name.Length) + 2;
            int statusWidth = "HAS ACTIVE LOANS".Length;

            bool exitMenu = false;
            var menu = new MenuBuilder("DELETE MEMBER");

            foreach (var m in members)
            {
                var memberId = m.MemberId;

                string idCol = $"[{m.MemberId}]".PadRight(idWidth + 2);
                string nameCol = m.Name.PadRight(nameWidth);
                string statusCol = (m.HasActiveLoans ? "HAS ACTIVE LOANS" : "OK")
                    .PadRight(statusWidth);

                menu.Add($"{idCol}{nameCol}{statusCol}", () =>
                {
                    if (m.HasActiveLoans)
                    {
                        Console.Clear();
                        ConsoleHelper.WriteHeader("DELETE MEMBER");

                        ConsoleHelper.Warning("Member cannot be deleted\n");

                        Console.WriteLine($"Member: {m.Name}");
                        Console.WriteLine("Reason: Active loans exist");

                        ConsoleHelper.Pause();
                        return;
                    }

                    var member = db.Members.FirstOrDefault(x => x.MemberId == memberId);
                    if (member == null)
                        return;

                    db.Members.Remove(member);
                    db.SaveChanges();

                    Console.Clear();
                    ConsoleHelper.WriteHeader("MEMBER DELETED");
                    ConsoleHelper.Success(
                        $"Member deleted: ID {member.MemberId} - {member.FirstName} {member.LastName}"
                    );

                    ConsoleHelper.Pause();
                    exitMenu = true;
                });
            }

            menu
                .Back()
                .CloseAfterSelection()
                .Run();

            if (exitMenu)
                return;
        }

        public void ListAllMembers()
        {
            ConsoleHelper.WriteHeader("MEMBERS LIST");

            var input = ConsoleHelper.ReadInputWithBack(
                "How many members to list (Enter = 10, type 'all' for all)"
            );

            if (input == null)
                return;

            int? limit = 10;

            if (!string.IsNullOrWhiteSpace(input))
            {
                if (input.Equals("all", StringComparison.OrdinalIgnoreCase))
                    limit = null;
                else if (int.TryParse(input, out var parsed) && parsed > 0)
                    limit = parsed;
            }

            int sortChoice = 1;

            new MenuBuilder("SORT MEMBERS BY")
                .Add("Member ID (newest first)", () => sortChoice = 1)
                .Add("First name (A-Ö)", () => sortChoice = 2)
                .Add("Last name (A-Ö)", () => sortChoice = 3)
                .CloseAfterSelection()
                .Run();

            using var db = new LibraryDBContext();

            IQueryable<Member> query = db.Members;

            switch (sortChoice)
            {
                case 2:
                    query = query.OrderBy(m => m.FirstName);
                    break;

                case 3:
                    query = query.OrderBy(m => m.LastName);
                    break;

                default:
                    query = query.OrderByDescending(m => m.MemberId);
                    break;
            }

            if (limit.HasValue)
                query = query.Take(limit.Value);

            var members = query
                .Select(m => new
                {
                    m.MemberId,
                    m.FirstName,
                    m.LastName
                })
                .ToList();

            if (!members.Any())
            {
                ConsoleHelper.Info("No members found");
                ConsoleHelper.Pause();
                return;
            }

            var headers = new[]
            {
                "Member ID",
                "First name",
                "Last name"
            };

            var rows = members
                .Select(m => new[]
                {
                    m.MemberId.ToString(),
                    m.FirstName,
                    m.LastName
                })
                .ToArray();

            ConsoleHelper.PrintTable(headers, rows);
            ConsoleHelper.Pause();
        }

        public void EditMember()
        {
            using var db = new LibraryDBContext();

            var members = db.Members
                .OrderBy(m => m.LastName)
                .ThenBy(m => m.FirstName)
                .Select(m => new
                {
                    m.MemberId,
                    m.FirstName,
                    m.LastName
                })
                .ToList();

            if (!members.Any())
            {
                ConsoleHelper.Info("No members found");
                ConsoleHelper.Pause();
                return;
            }

            var menu = new MenuBuilder("EDIT MEMBER");

            foreach (var m in members)
            {
                var label = ConsoleHelper.FormatMemberMenuRow(
                    m.MemberId,
                    m.FirstName,
                    m.LastName
                );

                menu.Add(label, () =>
                {
                    Console.Clear();
                    ConsoleHelper.WriteHeader("EDIT MEMBER");

                    Console.WriteLine($"Member ID: {m.MemberId}");
                    Console.WriteLine($"Current name: {m.FirstName} {m.LastName}\n");

                    var newFirstName = ConsoleHelper.ReadInputWithBack("New first name");
                    if (newFirstName == null)
                        return;

                    var newLastName = ConsoleHelper.ReadInputWithBack("New last name");
                    if (newLastName == null)
                        return;

                    if (string.IsNullOrWhiteSpace(newFirstName) ||
                        string.IsNullOrWhiteSpace(newLastName))
                    {
                        ConsoleHelper.Warning("First name and last name are required");
                        ConsoleHelper.Pause();
                        return;
                    }

                    var member = db.Members.First(x => x.MemberId == m.MemberId);

                    member.FirstName = newFirstName.Trim();
                    member.LastName = newLastName.Trim();

                    db.SaveChanges();

                    Console.Clear();
                    ConsoleHelper.Success("Member updated successfully\n");
                    Console.WriteLine($"Member ID: {member.MemberId}");
                    Console.WriteLine($"Name: {member.FirstName} {member.LastName}");

                    ConsoleHelper.Pause();
                });
            }

            menu
                .Back("Back")
                .CloseAfterSelection()
                .Run();
        }
        public void BlockMember()
        {
            using var db = new LibraryDBContext();

            var members = db.Members
                .OrderBy(m => m.MemberId)
                .ToList();

            if (!members.Any())
            {
                ConsoleHelper.Info("No members found");
                ConsoleHelper.Pause();
                return;
            }

            var menu = new MenuBuilder("BLOCK MEMBER");

            foreach (var m in members)
            {
                if (m.IsBlocked)
                    continue;

                var memberId = m.MemberId;

                menu.Add(
                    $"{m.MemberId,3} | {m.FirstName} {m.LastName}",
                    () =>
                    {
                        m.IsBlocked = true;
                        db.SaveChanges();

                        Console.Clear();
                        ConsoleHelper.Success("Member blocked");
                        Console.WriteLine($"Member: {m.FirstName} {m.LastName}");
                        ConsoleHelper.Pause();
                    });
            }

            menu.Back().Run();
        }

        public void UnblockMember()
        {
            using var db = new LibraryDBContext();

            var blockedMembers = db.Members
                .Where(m => m.IsBlocked)
                .OrderBy(m => m.MemberId)
                .ToList();

            if (!blockedMembers.Any())
            {
                ConsoleHelper.Info("No blocked members");
                ConsoleHelper.Pause();
                return;
            }

            var menu = new MenuBuilder("UNBLOCK MEMBER");

            foreach (var m in blockedMembers)
            {
                var memberId = m.MemberId;

                menu.Add(
                    $"{m.MemberId} | {m.FirstName} {m.LastName}",
                    () =>
                    {
                        m.IsBlocked = false;
                        db.SaveChanges();

                        Console.Clear();
                        ConsoleHelper.Success("Member unblocked");
                        Console.WriteLine($"Member: {m.FirstName} {m.LastName}");
                        ConsoleHelper.Pause();
                    });
            }

            menu.Back().Run();
        }
    }
}
