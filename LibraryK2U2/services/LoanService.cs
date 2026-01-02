using System;
using System.Linq;
using LibraryK2U2.data;
using LibraryK2U2.helpers;
using LibraryK2U2.models;
using Microsoft.EntityFrameworkCore;

namespace LibraryK2U2.services
{
    public class LoanService
    {
        public void RegisterLoan()
        {
            using var db = new LibraryDBContext();

            // Books without active loans
            var availableBooks = db.Books
                .Where(b => !db.Loans.Any(l =>
                    l.BookId == b.BookId &&
                    l.ReturnDate == null))
                .Select(b => new
                {
                    b.BookId,
                    Label = ConsoleHelper.FormatBookMenuRow(
                        b.BookId,
                        b.Title,
                        b.Author)
                })
                .ToList();

            if (!availableBooks.Any())
            {
                ConsoleHelper.Info("No available books");
                ConsoleHelper.Pause();
                return;
            }

            int? selectedBookId = null;

            var bookMenu = new MenuBuilder("SELECT BOOK");

            foreach (var b in availableBooks)
            {
                var id = b.BookId;
                bookMenu.Add(b.Label, () => selectedBookId = id);
            }

            bookMenu
                .Back()
                .CloseAfterSelection()
                .Run();

            if (selectedBookId == null)
                return;

            SelectMemberAndCreateLoan(db, selectedBookId.Value);
        }

        // Member selection and loan creation
        private void SelectMemberAndCreateLoan(LibraryDBContext db, int bookId)
        {
            var members = db.Members
                .Select(m => new
                {
                    m.MemberId,
                    Label = ConsoleHelper.FormatMemberMenuRow(
                        m.MemberId,
                        m.FirstName,
                        m.LastName)
                })
                .ToList();

            if (!members.Any())
            {
                ConsoleHelper.Info("No members found");
                ConsoleHelper.Pause();
                return;
            }

            var menu = new MenuBuilder("SELECT MEMBER");

            foreach (var m in members)
            {
                var memberId = m.MemberId;

                menu.Add(m.Label, () =>
                {
                    var member = db.Members.First(x => x.MemberId == memberId);

                    if (member.IsBlocked)
                    {
                        Console.Clear();
                        ConsoleHelper.Error("Member is blocked and cannot borrow books");
                        ConsoleHelper.Pause();
                        return;
                    }

                    using var tx = db.Database.BeginTransaction();

                    try
                    {
                        var loan = new Loan
                        {
                            BookId = bookId,
                            MemberId = memberId,
                            LoanDate = DateOnly.FromDateTime(DateTime.Today),
                            DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(14)),
                            ReturnDate = null
                        };

                        db.Loans.Add(loan);
                        db.SaveChanges();
                        tx.Commit();

                        var book = db.Books.First(x => x.BookId == bookId);

                        Console.Clear();
                        ConsoleHelper.Success("Loan registered successfully\n");
                        Console.WriteLine($"Member: {member.FirstName} {member.LastName}");
                        Console.WriteLine($"Book:   {book.Title}");
                        Console.WriteLine($"Due:    {loan.DueDate}");

                        ConsoleHelper.Pause();
                    }
                    catch
                    {
                        tx.Rollback();
                        ConsoleHelper.Error("Failed to register loan");
                        ConsoleHelper.Pause();
                    }
                });

            }

            menu
                .Back()
                .CloseAfterSelection()
                .Run();
        }

        public void RegisterReturn()
        {
            using var db = new LibraryDBContext();

            var activeLoans = db.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Where(l => l.ReturnDate == null)
                .ToList();

            if (!activeLoans.Any())
            {
                ConsoleHelper.Info("No active loans");
                ConsoleHelper.Pause();
                return;
            }

            var menu = new MenuBuilder("REGISTER RETURN");

            foreach (var loan in activeLoans)
            {
                menu.Add(
                    ConsoleHelper.FormatLoanMenuRow(
                        loan.LoanId,
                        loan.Member.FirstName,
                        loan.Member.LastName,
                        loan.Book.Title,
                        loan.DueDate
                    ),
                    () =>
                    {
                        loan.ReturnDate = DateOnly.FromDateTime(DateTime.Today);
                        db.SaveChanges();

                        Console.Clear();
                        ConsoleHelper.Success("Loan returned successfully\n");
                        Console.WriteLine($"Member: {loan.Member.FirstName} {loan.Member.LastName}");
                        Console.WriteLine($"Book:   {loan.Book.Title}");

                        ConsoleHelper.Pause();
                    });
            }

            menu
                .Back()
                .CloseAfterSelection()
                .Run();
        }

        public void ForceReturn()
        {
            using var db = new LibraryDBContext();

            var activeLoans = db.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Where(l => l.ReturnDate == null)
                .ToList();

            if (!activeLoans.Any())
            {
                ConsoleHelper.Info("No active loans");
                ConsoleHelper.Pause();
                return;
            }

            var menu = new MenuBuilder("FORCE RETURN");

            foreach (var loan in activeLoans)
            {
                menu.Add(
                    ConsoleHelper.FormatLoanMenuRow(
                        loan.LoanId,
                        loan.Member.FirstName,
                        loan.Member.LastName,
                        loan.Book.Title,
                        loan.DueDate
                    ),
                    () =>
                    {
                        loan.ReturnDate = DateOnly.FromDateTime(DateTime.Today);
                        db.SaveChanges();

                        Console.Clear();
                        ConsoleHelper.Success("Loan force-returned successfully\n");
                        Console.WriteLine($"Member: {loan.Member.FirstName} {loan.Member.LastName}");
                        Console.WriteLine($"Book:   {loan.Book.Title}");

                        ConsoleHelper.Pause();
                    });
            }

            menu
                .Back()
                .CloseAfterSelection()
                .Run();
        }

        public void ChangeDueDate()
        {
            using var db = new LibraryDBContext();

            var activeLoans = db.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Where(l => l.ReturnDate == null)
                .ToList();

            if (!activeLoans.Any())
            {
                ConsoleHelper.Info("No active loans");
                ConsoleHelper.Pause();
                return;
            }

            var menu = new MenuBuilder("CHANGE DUE DATE");

            foreach (var loan in activeLoans)
            {
                menu.Add(
                    ConsoleHelper.FormatLoanMenuRow(
                        loan.LoanId,
                        loan.Member.FirstName,
                        loan.Member.LastName,
                        loan.Book.Title,
                        loan.DueDate
                    ),
                    () =>
                    {
                        Console.Clear();
                        ConsoleHelper.WriteHeader("CHANGE DUE DATE");

                        Console.WriteLine($"Member: {loan.Member.FirstName} {loan.Member.LastName}");
                        Console.WriteLine($"Book:   {loan.Book.Title}");
                        Console.WriteLine($"Current due date: {loan.DueDate}\n");

                        var input = ConsoleHelper.ReadInputWithBack("New due date (yyyy-mm-dd)");
                        if (input == null)
                            return;

                        if (!DateOnly.TryParse(input, out var newDate))
                        {
                            ConsoleHelper.Warning("Invalid date");
                            ConsoleHelper.Pause();
                            return;
                        }

                        var oldDate = loan.DueDate;
                        loan.DueDate = newDate;
                        db.SaveChanges();

                        Console.Clear();
                        ConsoleHelper.Success("Due date updated successfully\n");
                        Console.WriteLine($"Member: {loan.Member.FirstName} {loan.Member.LastName}");
                        Console.WriteLine($"Book:   {loan.Book.Title}");
                        Console.WriteLine($"Due:    {oldDate} -> {newDate}");

                        ConsoleHelper.Pause();
                    });
            }

            menu
                .Back()
                .CloseAfterSelection()
                .Run();
        }

        public void ShowActiveLoans()
        {
            ConsoleHelper.WriteHeader("ACTIVE LOANS");

            var input = ConsoleHelper.ReadInputWithBack(
                "How many loans to list (Enter = 10, type 'all' for all)"
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

            new MenuBuilder("SORT LOANS BY")
                .Add("Loan ID (newest first)", () => RenderActiveLoans(1, limit))
                .Add("Due date (earliest first)", () => RenderActiveLoans(2, limit))
                .Add("Due date (latest first)", () => RenderActiveLoans(3, limit))
                .Add("Member name (A-Ö)", () => RenderActiveLoans(4, limit))
                .Add("Book title (A-Ö)", () => RenderActiveLoans(5, limit))
                .CloseAfterSelection()
                .Run();

            ConsoleHelper.Pause();
        }

        private void RenderActiveLoans(int sortChoice, int? limit)
        {
            using var db = new LibraryDBContext();

            IQueryable<Loan> query = db.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Where(l => l.ReturnDate == null);

            query = sortChoice switch
            {
                2 => query.OrderBy(l => l.DueDate),
                3 => query.OrderByDescending(l => l.DueDate),
                4 => query.OrderBy(l => l.Member.FirstName)
                          .ThenBy(l => l.Member.LastName)
                          .ThenBy(l => l.Member.MemberId),
                5 => query.OrderBy(l => l.Book.Title),
                _ => query.OrderByDescending(l => l.LoanId)
            };

            if (limit.HasValue)
                query = query.Take(limit.Value);

            var loans = query
                .Select(l => new
                {
                    l.LoanId,
                    Book = l.Book.Title,
                    MemberId = l.Member.MemberId,
                    MemberName = l.Member.FirstName + " " + l.Member.LastName,
                    l.LoanDate,
                    l.DueDate
                })
                .ToList();

            if (!loans.Any())
            {
                ConsoleHelper.Info("No active loans");
                return;
            }

            var headers = new[]
            {
                "Loan ID",
                "Book",
                "Member ID",
                "Member",
                "Loan date",
                "Due date"
            };

            var rows = loans.Select(l => new[]
            {
                l.LoanId.ToString(),
                l.Book,
                l.MemberId.ToString(),
                l.MemberName,
                l.LoanDate.ToString(),
                l.DueDate.ToString()
            }).ToArray();

            ConsoleHelper.PrintTable(headers, rows);
        }

        public void ShowAllLoans()
        {
            using var db = new LibraryDBContext();

            var loans = db.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .OrderByDescending(l => l.LoanId)
                .Select(l => new
                {
                    l.LoanId,
                    Book = l.Book.Title,
                    MemberId = l.Member.MemberId,
                    MemberName = l.Member.FirstName + " " + l.Member.LastName,
                    l.LoanDate,
                    l.DueDate,
                    Status = l.ReturnDate == null ? "ACTIVE" : "RETURNED"
                })
                .ToList();

            ConsoleHelper.WriteHeader("ALL LOANS");

            if (!loans.Any())
            {
                ConsoleHelper.Info("No loans found");
                ConsoleHelper.Pause();
                return;
            }

            var headers = new[]
            {
                "Loan ID",
                "Book",
                "Member ID",
                "Member",
                "Loan date",
                "Due date",
                "Status"
            };

            var rows = loans.Select(l => new[]
            {
                l.LoanId.ToString(),
                l.Book,
                l.MemberId.ToString(),
                l.MemberName,
                l.LoanDate.ToString(),
                l.DueDate.ToString(),
                l.Status
            }).ToArray();

            ConsoleHelper.PrintTable(headers, rows);
            ConsoleHelper.Pause();
        }

        //Current Overdue loans
        public void ShowBlacklist()
        {
            using var db = new LibraryDBContext();

            var today = DateOnly.FromDateTime(DateTime.Today);

            var overdueLoans = db.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Where(l =>
                    l.ReturnDate == null &&
                    l.DueDate < today)
                .OrderBy(l => l.DueDate)
                .ToList();

            Console.Clear();
            ConsoleHelper.WriteHeader("BLACKLIST (OVERDUE LOANS)");

            if (!overdueLoans.Any())
            {
                ConsoleHelper.Success("No overdue loans");
                ConsoleHelper.Pause();
                return;
            }

            foreach (var loan in overdueLoans)
            {
                int daysLate = today.DayNumber - loan.DueDate.DayNumber;

                var history = db.Loans
                    .Where(l =>
                        l.MemberId == loan.Member.MemberId &&
                        l.ReturnDate != null &&
                        l.ReturnDate > l.DueDate)
                    .Select(l =>
                        l.ReturnDate!.Value.DayNumber - l.DueDate.DayNumber)
                    .ToList();

                int previousOverdues = history.Count;
                int totalDaysLate = history.Sum();

                Console.WriteLine($"Member ID:  {loan.Member.MemberId}");
                Console.WriteLine($"Member:     {loan.Member.FirstName} {loan.Member.LastName}");
                Console.WriteLine($"Book:       {loan.Book.Title}");
                Console.WriteLine($"Due:        {loan.DueDate}");
                Console.WriteLine($"Overdue:    {daysLate} day{(daysLate == 1 ? "" : "s")}");
                Console.WriteLine();
                Console.WriteLine("History:");
                Console.WriteLine($"- Previous overdue loans: {previousOverdues}");
                Console.WriteLine($"- Total overdue days:     {totalDaysLate}");
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine();
            }

            ConsoleHelper.Pause();
        }

        public void ShowHistoricalOverdueLoans()
        {
            using var db = new LibraryDBContext();

            var lateLoans = db.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Where(l =>
                    l.ReturnDate != null &&
                    l.ReturnDate > l.DueDate)
                .OrderByDescending(l => l.ReturnDate)
                .ToList();

            Console.Clear();
            ConsoleHelper.WriteHeader("HISTORICAL OVERDUE LOANS");

            if (!lateLoans.Any())
            {
                ConsoleHelper.Success("No historical overdue loans");
                ConsoleHelper.Pause();
                return;
            }

            foreach (var loan in lateLoans)
            {
                int daysLate =
                    loan.ReturnDate!.Value.DayNumber -
                    loan.DueDate.DayNumber;

                Console.WriteLine($"Member ID:  {loan.Member.MemberId}");
                Console.WriteLine($"Member:     {loan.Member.FirstName} {loan.Member.LastName}");
                Console.WriteLine($"Book:       {loan.Book.Title}");
                Console.WriteLine($"Due:        {loan.DueDate}");
                Console.WriteLine($"Returned:   {loan.ReturnDate}");
                Console.WriteLine($"Late by:    {daysLate} day{(daysLate == 1 ? "" : "s")}");
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine();
            }

            ConsoleHelper.Pause();
        }
    }
}