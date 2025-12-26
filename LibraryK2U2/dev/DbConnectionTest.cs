using System;
using System.Linq;
using LibraryK2U2.data;

namespace LibraryK2U2.dev
{
    // Simple development utility to verify database connection and data access
    internal static class DbConnectionTest
    {
        public static void Run()
        {
            using var db = new LibraryDBContext();

            // Books table
            Console.WriteLine("BOOKS");
            Console.WriteLine("-----------------------------");

            foreach (var b in db.Books.ToList())
            {
                Console.WriteLine($"{b.BookId}: {b.Title} - {b.Author} - {b.Category}");
            }

            // Members table
            Console.WriteLine("\nMEMBERS");
            Console.WriteLine("-----------------------------");

            foreach (var m in db.Members.ToList())
            {
                Console.WriteLine($"{m.MemberId}: {m.FirstName} {m.LastName}");
            }

            // Loans table (raw data)
            Console.WriteLine("\nLOANS");
            Console.WriteLine("-----------------------------");

            foreach (var l in db.Loans.ToList())
            {
                Console.WriteLine(
                    $"LoanId: {l.LoanId}, BookId: {l.BookId}, MemberId: {l.MemberId}, " +
                    $"LoanDate: {l.LoanDate}, DueDate: {l.DueDate}, ReturnDate: {l.ReturnDate}"
                );
            }

            // Active loans view
            Console.WriteLine("\nACTIVE LOANS (VIEW)");
            Console.WriteLine("-----------------------------");

            foreach (var a in db.ActiveLoans.ToList())
            {
                Console.WriteLine($"{a.MemberName} -> {a.BookTitle}");
            }

            // Returned loans view
            Console.WriteLine("\nRETURNED LOANS (VIEW)");
            Console.WriteLine("-----------------------------");

            foreach (var r in db.ReturnedLoans.ToList())
            {
                Console.WriteLine($"{r.MemberName} -> {r.BookTitle}");
            }

            Console.WriteLine("\nDone.");
            Console.ReadKey();
        }
    }
}
