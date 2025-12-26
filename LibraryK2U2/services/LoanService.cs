using LibraryK2U2.data;
using LibraryK2U2.helpers;
using LibraryK2U2.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LibraryK2U2.services
{
    public class LoanService
    {
        public void RegisterLoan()
        {
            ConsoleHelper.WriteHeader("Register Loan");

            var bookIdInput = ConsoleHelper.ReadInput("Book ID");
            var memberIdInput = ConsoleHelper.ReadInput("Member ID");

            if (!int.TryParse(bookIdInput, out int bookId) ||
                !int.TryParse(memberIdInput, out int memberId))
            {
                ConsoleHelper.Warning("Invalid Book ID or Member ID");
                ConsoleHelper.Pause();
                return;
            }

            using var db = new LibraryDBContext();
            using var transaction = db.Database.BeginTransaction();

            try
            {
                // Check if book already has an active loan
                bool bookIsLoaned = db.Loans
                    .Any(l => l.BookId == bookId && l.ReturnDate == null);

                if (bookIsLoaned)
                {
                    ConsoleHelper.Warning("Book is already loaned");
                    transaction.Rollback();
                    ConsoleHelper.Pause();
                    return;
                }

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

                transaction.Commit();

                ConsoleHelper.Success("Loan registered successfully");
            }
            catch
            {
                transaction.Rollback();
                ConsoleHelper.Error("Failed to register loan");
            }

            ConsoleHelper.Pause();
        }

        public void RegisterReturn()
        {
            ConsoleHelper.WriteHeader("Register Return");

            var loanIdInput = ConsoleHelper.ReadInput("Loan ID");

            if (!int.TryParse(loanIdInput, out int loanId))
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

            if (loan.ReturnDate != null)
            {
                ConsoleHelper.Warning("Book already returned");
                ConsoleHelper.Pause();
                return;
            }

            loan.ReturnDate = DateOnly.FromDateTime(DateTime.Today);
            db.SaveChanges();

            ConsoleHelper.Success("Book returned successfully");
            ConsoleHelper.Pause();
        }

        public void ShowActiveLoans()
        {
            ConsoleHelper.WriteHeader("Active Loans");

            using var db = new LibraryDBContext();

            var loans = db.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .Where(l => l.ReturnDate == null)
                .Select(l => new
                {
                    l.LoanId,
                    BookTitle = l.Book.Title,
                    MemberName = l.Member.FirstName + " " + l.Member.LastName,
                    l.LoanDate,
                    l.DueDate
                })
                .ToList();

            if (!loans.Any())
            {
                ConsoleHelper.Info("No active loans");
                ConsoleHelper.Pause();
                return;
            }

            var headers = new[]
            {
                "Loan ID",
                "Book",
                "Member",
                "Loan date",
                "Due date"
            };

            var rows = loans
                .Select(l => new[]
                {
                    l.LoanId.ToString(),
                    l.BookTitle,
                    l.MemberName,
                    l.LoanDate.ToString(),
                    l.DueDate.ToString()
                })
                .ToArray();

            ConsoleHelper.PrintTable(headers, rows);
            ConsoleHelper.Pause();
        }
    }
}
