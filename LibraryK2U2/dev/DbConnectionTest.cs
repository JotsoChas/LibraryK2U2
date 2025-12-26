using System;
using System.Linq;
using LibraryK2U2.data;

namespace LibraryK2U2.dev
{
    internal static class DbConnectionTest
    {
        public static void Run()
        {
            using var db = new LibraryDBContext();

            // BOOKS
            Console.WriteLine("BOOKS");
            Console.WriteLine("-----------------------------");
            foreach (var b in db.Books.ToList())
            {
                Console.WriteLine($"{b.BookId}: {b.Title} - {b.Author} - {b.Category}");
            }

            // MEMBERS
            Console.WriteLine();
            Console.WriteLine("MEMBERS");
            Console.WriteLine("-----------------------------");
            foreach (var m in db.Members.ToList())
            {
                Console.WriteLine($"{m.MemberId}: {m.FirstName} {m.LastName}");
            }

            // LOANS
            Console.WriteLine();
            Console.WriteLine("LOANS");
            Console.WriteLine("-----------------------------");
            foreach (var l in db.Loans.ToList())
            {
                Console.WriteLine(
                    $"LoanId: {l.LoanId}, BookId: {l.BookId}, MemberId: {l.MemberId}, " +
                    $"LoanDate: {l.LoanDate}, DueDate: {l.DueDate}, ReturnDate: {l.ReturnDate}"
                );
            }

            // ACTIVE LOANS (VIEW)
            Console.WriteLine();
            Console.WriteLine("ACTIVE LOANS (VIEW)");
            Console.WriteLine("-----------------------------");
            foreach (var a in db.ActiveLoans.ToList())
            {
                Console.WriteLine($"{a.MemberName} -> {a.BookTitle}");
            }

            // RETURNED LOANS (VIEW)
            Console.WriteLine();
            Console.WriteLine("RETURNED LOANS (VIEW)");
            Console.WriteLine("-----------------------------");
            foreach (var r in db.ReturnedLoans.ToList())
            {
                Console.WriteLine($"{r.MemberName} -> {r.BookTitle}");
            }

            Console.WriteLine();
            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
