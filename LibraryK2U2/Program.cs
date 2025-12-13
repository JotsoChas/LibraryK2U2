using LibraryK2U2.data;
using LibraryK2U2.models;
using System;
using System.Linq;

namespace LibraryK2U2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var db = new LibraryDBContext();

            var books = db.Books.ToList();

            Console.WriteLine("All books:");
            Console.WriteLine("-----------------------------");

            foreach (var b in books)
            {
                Console.WriteLine($"{b.BookId}: {b.Title} - {b.Author} - {b.Category}");
            }

            Console.WriteLine("-----------------------------");
            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
