using System;
using System.Linq;
using LibraryK2U2.data;
using LibraryK2U2.helpers;
using LibraryK2U2.models;

namespace LibraryK2U2.services
{
    public class AdminService
    {
        public void ShowAllLoans()
        {
            ConsoleHelper.WriteHeader("All Loans");

            using var db = new LibraryDBContext();

            var loans = db.Loans
                .Select(l => new
                {
                    l.LoanId,
                    l.BookId,
                    l.MemberId,
                    l.LoanDate,
                    l.DueDate,
                    l.ReturnDate
                })
                .ToList();

            if (!loans.Any())
            {
                ConsoleHelper.Info("No loans found");
                ConsoleHelper.Pause();
                return;
            }

            foreach (var l in loans)
            {
                Console.WriteLine(
                    $"LoanId: {l.LoanId}, BookId: {l.BookId}, MemberId: {l.MemberId}, " +
                    $"LoanDate: {l.LoanDate}, DueDate: {l.DueDate}, ReturnDate: {l.ReturnDate}"
                );
            }

            ConsoleHelper.Pause();
        }

        public void ForceReturn()
        {
            ConsoleHelper.WriteHeader("Force Return Loan");

            var input = ConsoleHelper.ReadInput("Loan ID");

            if (!int.TryParse(input, out int loanId))
            {
                ConsoleHelper.Warning("Invalid Loan ID");
                ConsoleHelper.Pause();
                return;
            }

            using var db = new LibraryDBContext();

            var loan = db.Loans.FirstOrDefault(l => l.LoanId == loanId);

            if (loan == null)
            {
                ConsoleHelper.Warning("Loan not found");
                ConsoleHelper.Pause();
                return;
            }

            loan.ReturnDate = DateOnly.FromDateTime(DateTime.Today);
            db.SaveChanges();

            ConsoleHelper.Success("Loan was force returned");
            ConsoleHelper.Pause();
        }

        public void ChangeDueDate()
        {
            ConsoleHelper.WriteHeader("Change Due Date");

            var loanInput = ConsoleHelper.ReadInput("Loan ID");
            var dateInput = ConsoleHelper.ReadInput("New due date (yyyy-mm-dd)");

            if (!int.TryParse(loanInput, out int loanId) ||
                !DateOnly.TryParse(dateInput, out DateOnly newDate))
            {
                ConsoleHelper.Warning("Invalid input");
                ConsoleHelper.Pause();
                return;
            }

            using var db = new LibraryDBContext();

            var loan = db.Loans.FirstOrDefault(l => l.LoanId == loanId);

            if (loan == null)
            {
                ConsoleHelper.Warning("Loan not found");
                ConsoleHelper.Pause();
                return;
            }

            loan.DueDate = newDate;
            db.SaveChanges();

            ConsoleHelper.Success("Due date updated");
            ConsoleHelper.Pause();
        }

        public void DeleteBook()
        {
            ConsoleHelper.WriteHeader("Delete Book");

            var input = ConsoleHelper.ReadInput("Book ID");

            if (!int.TryParse(input, out int bookId))
            {
                ConsoleHelper.Warning("Invalid Book ID");
                ConsoleHelper.Pause();
                return;
            }

            using var db = new LibraryDBContext();

            bool hasLoans = db.Loans.Any(l => l.BookId == bookId);

            if (hasLoans)
            {
                ConsoleHelper.Warning("Book has loans and cannot be deleted");
                ConsoleHelper.Pause();
                return;
            }

            var book = db.Books.FirstOrDefault(b => b.BookId == bookId);

            if (book == null)
            {
                ConsoleHelper.Warning("Book not found");
                ConsoleHelper.Pause();
                return;
            }

            db.Books.Remove(book);
            db.SaveChanges();

            ConsoleHelper.Success("Book deleted");
            ConsoleHelper.Pause();
        }

        public void DeleteMember()
        {
            ConsoleHelper.WriteHeader("Delete Member");

            var input = ConsoleHelper.ReadInput("Member ID");

            if (!int.TryParse(input, out int memberId))
            {
                ConsoleHelper.Warning("Invalid Member ID");
                ConsoleHelper.Pause();
                return;
            }

            using var db = new LibraryDBContext();

            bool hasActiveLoans = db.Loans.Any(l =>
                l.MemberId == memberId && l.ReturnDate == null);

            if (hasActiveLoans)
            {
                ConsoleHelper.Warning("Member has active loans");
                ConsoleHelper.Pause();
                return;
            }

            var member = db.Members.FirstOrDefault(m => m.MemberId == memberId);

            if (member == null)
            {
                ConsoleHelper.Warning("Member not found");
                ConsoleHelper.Pause();
                return;
            }

            db.Members.Remove(member);
            db.SaveChanges();

            ConsoleHelper.Success("Member deleted");
            ConsoleHelper.Pause();
        }
    }
}
