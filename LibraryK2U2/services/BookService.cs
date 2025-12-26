using LibraryK2U2.data;
using LibraryK2U2.helpers;
using LibraryK2U2.models;
using System.Linq;

namespace LibraryK2U2.services
{
    public class BookService
    {
        public void RegisterBook()
        {
            ConsoleHelper.WriteHeader("Register New Book");

            var title = ConsoleHelper.ReadInput("Title");
            var author = ConsoleHelper.ReadInput("Author");
            var isbn = ConsoleHelper.ReadInput("ISBN");
            var category = ConsoleHelper.ReadInput("Category");

            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(author) ||
                string.IsNullOrWhiteSpace(category))
            {
                ConsoleHelper.Warning("Title, author and category are required");
                ConsoleHelper.Pause();
                return;
            }

            using var db = new LibraryDBContext();

            var book = new Book
            {
                Title = title,
                Author = author,
                Isbn = string.IsNullOrWhiteSpace(isbn) ? null : isbn,
                Category = category
            };

            db.Books.Add(book);
            db.SaveChanges();

            ConsoleHelper.Success("Book registered successfully");
            ConsoleHelper.Pause();
        }

        public void SearchBooks()
        {
            ConsoleHelper.WriteHeader("Search Books");

            var search = ConsoleHelper.ReadInput("Search by title or author");

            using var db = new LibraryDBContext();

            var results = db.Books
                .Where(b =>
                    b.Title.Contains(search) ||
                    b.Author.Contains(search))
                .Select(b => new
                {
                    b.BookId,
                    b.Title,
                    b.Author,
                    b.Category
                })
                .ToList();

            if (!results.Any())
            {
                ConsoleHelper.Info("No books found");
                ConsoleHelper.Pause();
                return;
            }

            var headers = new[] { "ID", "Title", "Author", "Category" };
            var rows = results
                .Select(b => new[]
                {
                    b.BookId.ToString(),
                    b.Title,
                    b.Author,
                    b.Category
                })
                .ToArray();

            ConsoleHelper.PrintTable(headers, rows);
            ConsoleHelper.Pause();
        }
    }
}
