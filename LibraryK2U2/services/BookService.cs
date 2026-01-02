using LibraryK2U2.data;
using LibraryK2U2.helpers;
using LibraryK2U2.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LibraryK2U2.services
{
    public class BookService
    {
        public void RegisterBook()
        {
            ConsoleHelper.WriteHeader("REGISTER NEW BOOK");

            // Title
            var title = ConsoleHelper.ReadInputWithBack("Title");
            if (title == null)
                return;

            if (string.IsNullOrWhiteSpace(title))
            {
                ConsoleHelper.Warning("Title is required");
                ConsoleHelper.Pause();
                return;
            }

            // Author
            var author = ConsoleHelper.ReadInputWithBack("Author");
            if (author == null)
                return;

            if (string.IsNullOrWhiteSpace(author))
            {
                ConsoleHelper.Warning("Author is required");
                ConsoleHelper.Pause();
                return;
            }

            // ISBN (optional)
            var isbn = ConsoleHelper.ReadInput("ISBN (optional)");

            if (!string.IsNullOrWhiteSpace(isbn))
            {
                var cleaned = isbn.Replace("-", "").ToUpper();

                bool valid =
                    cleaned.Length == 10 &&
                    cleaned[..9].All(char.IsDigit) &&
                    (char.IsDigit(cleaned[9]) || cleaned[9] == 'X')
                    ||
                    cleaned.Length == 13 &&
                    cleaned.All(char.IsDigit);

                if (!valid)
                {
                    ConsoleHelper.Warning("Invalid ISBN format (10 or 13 digits)");
                    ConsoleHelper.Pause();
                    return;
                }
            }

            using var db = new LibraryDBContext();

            // Existing categories
            var categories = db.Books
                .Select(b => b.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .Select(c => new
                {
                    Category = c!,
                    BookCount = db.Books.Count(b => b.Category == c)
                })
                .OrderBy(c => c.Category)
                .ToList();

            string? selectedCategory = null;
            bool backToMenu = false;

            var categoryMenu = new MenuBuilder("SELECT CATEGORY");

            foreach (var c in categories)
            {
                var status = c.BookCount > 0
                    ? $"IN USE ({c.BookCount} books)"
                    : "OK";

                var label = $"{c.Category.PadRight(30)} {status}";

                categoryMenu.Add(label, () =>
                {
                    selectedCategory = c.Category;
                });
            }

            categoryMenu.Add("Create new category", () =>
            {
                Console.Clear();
                ConsoleHelper.WriteHeader("CREATE NEW CATEGORY");

                var newCategory = ConsoleHelper.ReadInputWithBack("New category name");

                if (newCategory == null)
                {
                    backToMenu = true;
                    return;
                }

                if (string.IsNullOrWhiteSpace(newCategory))
                {
                    ConsoleHelper.Warning("Category name is required");
                    ConsoleHelper.Pause();
                    return;
                }

                selectedCategory = newCategory.Trim();
            });

            categoryMenu
                .Back("Back", () => backToMenu = true)
                .CloseAfterSelection()
                .Run();

            if (backToMenu)
                return;

            if (string.IsNullOrWhiteSpace(selectedCategory))
                return;

            var book = new Book
            {
                Title = title.Trim(),
                Author = author.Trim(),
                Isbn = string.IsNullOrWhiteSpace(isbn) ? null : isbn.Trim(),
                Category = selectedCategory
            };

            db.Books.Add(book);
            db.SaveChanges();

            Console.Clear();
            ConsoleHelper.Success("Book registered successfully\n");

            Console.WriteLine($"Title:    {book.Title}");
            Console.WriteLine($"Author:   {book.Author}");
            Console.WriteLine($"Category: {book.Category}");

            ConsoleHelper.Pause();
        }

        public void SearchBooks()
        {
            int searchChoice = 1;

            new MenuBuilder("SEARCH BY")
                .Add("Title", () => searchChoice = 1)
                .Add("Author", () => searchChoice = 2)
                .Add("Category", () => searchChoice = 3)
                .Add("ISBN", () => searchChoice = 4)
                .CloseAfterSelection()
                .Run();

            Console.Clear();

            string searchLabel = searchChoice switch
            {
                2 => "AUTHOR",
                3 => "CATEGORY",
                4 => "ISBN",
                _ => "TITLE"
            };

            ConsoleHelper.WriteHeader($"SEARCH BY {searchLabel}");

            var searchText = ConsoleHelper.ReadInputWithBack("Enter search text");
            if (searchText == null)
                return;

            searchText = searchText.Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                ConsoleHelper.Warning("Search text cannot be empty");
                ConsoleHelper.Pause();
                return;
            }

            using var db = new LibraryDBContext();

            IQueryable<Book> query = db.Books;

            switch (searchChoice)
            {
                case 2:
                    query = query
                        .Where(b => b.Author.Contains(searchText))
                        .OrderBy(b => b.Author)
                        .ThenBy(b => b.Title);
                    break;

                case 3:
                    query = query
                        .Where(b => b.Category.Contains(searchText))
                        .OrderBy(b => b.Category)
                        .ThenBy(b => b.Title);
                    break;

                case 4:
                    query = query
                        .Where(b => b.Isbn != null && b.Isbn.Contains(searchText))
                        .OrderBy(b => b.Isbn);
                    break;

                default:
                    query = query
                        .Where(b => b.Title.Contains(searchText))
                        .OrderBy(b => b.Title)
                        .ThenBy(b => b.Author);
                    break;
            }

            var results = query
                .Select(b => new
                {
                    b.BookId,
                    b.Title,
                    b.Author,
                    b.Category,
                    b.Isbn,
                    HasActiveLoan = db.Loans.Any(l =>
                        l.BookId == b.BookId && l.ReturnDate == null)
                })
                .ToList();

            if (!results.Any())
            {
                ConsoleHelper.Info("No books found");
                ConsoleHelper.Pause();
                return;
            }

            var headers = new[] { "ID", "Title", "Author", "Category", "ISBN", "Active loan" };

            var rows = results.Select(b => new[]
            {
                b.BookId.ToString(),
                b.Title,
                b.Author,
                b.Category,
                b.Isbn ?? "-",
                b.HasActiveLoan ? "YES" : "NO"
            }).ToArray();

            ConsoleHelper.PrintTable(headers, rows);
            ConsoleHelper.Pause();
        }

        public void DeleteBook()
        {
            using var db = new LibraryDBContext();

            var books = db.Books
                .Select(b => new
                {
                    b.BookId,
                    Label = b.Title,
                    HasLoans = db.Loans.Any(l =>
                        l.BookId == b.BookId && l.ReturnDate == null)
                })
                .ToList();

            if (!books.Any())
            {
                ConsoleHelper.Info("No books found");
                ConsoleHelper.Pause();
                return;
            }

            bool exitMenu = false;
            var menu = new MenuBuilder("DELETE BOOK");

            foreach (var b in books.OrderBy(b => b.Label))
            {
                var status = b.HasLoans ? "HAS LOANS" : "OK";

                menu.Add(
                    ConsoleHelper.FormatBookMenuRow(
                        b.BookId,
                        b.Label,
                        $"[{status}]"
                    ),
                    () =>
                    {
                        if (b.HasLoans)
                        {
                            Console.Clear();
                            ConsoleHelper.WriteHeader("DELETE BOOK");
                            ConsoleHelper.Warning("Book has active loans and cannot be deleted\n");
                            Console.WriteLine($"Book ID: {b.BookId}");
                            Console.WriteLine($"Title:   {b.Label}");
                            ConsoleHelper.Pause();
                            return;
                        }

                        var book = db.Books.FirstOrDefault(x => x.BookId == b.BookId);
                        if (book == null)
                            return;

                        db.Books.Remove(book);
                        db.SaveChanges();

                        Console.Clear();
                        ConsoleHelper.WriteHeader("BOOK DELETED");
                        ConsoleHelper.Success("Book deleted successfully\n");
                        Console.WriteLine($"Book ID: {b.BookId}");
                        Console.WriteLine($"Title:   {b.Label}");
                        ConsoleHelper.Pause();

                        exitMenu = true;
                    });
            }

            menu
                .Back("Back", () => exitMenu = true)
                .CloseAfterSelection()
                .Run();
        }

        public void ListAllBooks()
        {
            ConsoleHelper.WriteHeader("BOOKS LIST");

            var input = ConsoleHelper.ReadInputWithBack(
                "How many books to list (Enter = 10, type 'all' for all)"
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

            new MenuBuilder("SORT BOOKS BY")
                .Add("Book ID (newest first)", () => sortChoice = 1)
                .Add("Title (A-Ö)", () => sortChoice = 2)
                .Add("Author (A-Ö)", () => sortChoice = 3)
                .Add("Category (A-Ö)", () => sortChoice = 4)
                .CloseAfterSelection()
                .Run();

            Console.Clear();
            ConsoleHelper.WriteHeader("BOOKS LIST");

            using var db = new LibraryDBContext();

            IQueryable<Book> query = db.Books;

            switch (sortChoice)
            {
                case 2:
                    query = query.OrderBy(b => b.Title);
                    break;
                case 3:
                    query = query.OrderBy(b => b.Author);
                    break;
                case 4:
                    query = query.OrderBy(b => b.Category);
                    break;
                default:
                    query = query.OrderByDescending(b => b.BookId);
                    break;
            }

            if (limit.HasValue)
                query = query.Take(limit.Value);

            var books = query
                .Select(b => new
                {
                    b.BookId,
                    b.Title,
                    b.Author,
                    b.Category,
                    b.Isbn,
                    HasActiveLoan = db.Loans.Any(l =>
                        l.BookId == b.BookId && l.ReturnDate == null)
                })
                .ToList();

            if (!books.Any())
            {
                ConsoleHelper.Info("No books found");
                ConsoleHelper.Pause();
                return;
            }

            var headers = new[] { "ID", "Title", "Author", "Category", "ISBN", "Active loan" };

            var rows = books.Select(b => new[]
            {
                b.BookId.ToString(),
                b.Title,
                b.Author,
                b.Category,
                b.Isbn ?? "-",
                b.HasActiveLoan ? "YES" : "NO"
            }).ToArray();

            ConsoleHelper.PrintTable(headers, rows);
            ConsoleHelper.Pause();
        }

        public void DeleteCategory()
        {
            using var db = new LibraryDBContext();

            var categories = db.Books
                .Where(b => !string.IsNullOrWhiteSpace(b.Category))
                .GroupBy(b => b.Category!)
                .Select(g => new
                {
                    Category = g.Key,
                    BookCount = g.Count()
                })
                .OrderBy(c => c.Category)
                .ToList();

            ConsoleHelper.WriteHeader("DELETE CATEGORY");

            if (!categories.Any())
            {
                ConsoleHelper.Info("No categories found");
                ConsoleHelper.Pause();
                return;
            }

            bool exitMenu = false;
            var menu = new MenuBuilder("DELETE CATEGORY");

            foreach (var c in categories)
            {
                var status = c.BookCount > 0
                    ? $"IN USE ({c.BookCount} books)"
                    : "OK";

                menu.Add(
                    $"{c.Category.PadRight(30)} {status}",
                    () =>
                    {
                        Console.Clear();
                        ConsoleHelper.WriteHeader("DELETE CATEGORY");

                        if (c.BookCount > 0)
                        {
                            var booksUsingCategory = db.Books
                                .Where(b => b.Category == c.Category)
                                .Select(b => new
                                {
                                    b.BookId,
                                    b.Title
                                })
                                .OrderBy(b => b.Title)
                                .ToList();

                            ConsoleHelper.Warning("Category cannot be deleted\n");

                            Console.WriteLine($"Category: {c.Category}");
                            Console.WriteLine("Reason:   Category is used by existing books");
                            Console.WriteLine($"Books using this category: {booksUsingCategory.Count}\n");

                            foreach (var book in booksUsingCategory)
                                Console.WriteLine($"- [{book.BookId}] {book.Title}");

                            ConsoleHelper.Pause();
                            return;
                        }

                        db.Books
                            .Where(b => b.Category == c.Category)
                            .ToList()
                            .ForEach(b => b.Category = "Uncategorized");

                        db.SaveChanges();

                        Console.Clear();
                        ConsoleHelper.WriteHeader("CATEGORY DELETED");
                        ConsoleHelper.Success("Category deleted successfully\n");
                        Console.WriteLine($"Category: {c.Category}");

                        ConsoleHelper.Pause();
                        exitMenu = true;
                    });
            }

            menu
                .Back()
                .CloseAfterSelection()
                .Run();
        }

        public void RenameCategory()
        {
            using var db = new LibraryDBContext();

            var categories = db.Books
                .Where(b => !string.IsNullOrWhiteSpace(b.Category))
                .Select(b => b.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            ConsoleHelper.WriteHeader("RENAME CATEGORY");

            if (!categories.Any())
            {
                ConsoleHelper.Info("No categories found");
                ConsoleHelper.Pause();
                return;
            }

            bool exitMenu = false;
            var menu = new MenuBuilder("SELECT CATEGORY");

            foreach (var category in categories)
            {
                menu.Add(category, () =>
                {
                    Console.Clear();
                    ConsoleHelper.WriteHeader($"RENAME CATEGORY: {category}");

                    var newName = ConsoleHelper.ReadInputWithBack("New category name");
                    if (newName == null)
                        return;

                    if (string.IsNullOrWhiteSpace(newName))
                    {
                        ConsoleHelper.Warning("Category name is required");
                        ConsoleHelper.Pause();
                        return;
                    }

                    var booksToUpdate = db.Books
                        .Where(b => b.Category == category)
                        .ToList();

                    booksToUpdate.ForEach(b => b.Category = newName.Trim());
                    db.SaveChanges();

                    Console.Clear();
                    ConsoleHelper.WriteHeader("CATEGORY RENAMED");
                    ConsoleHelper.Success(
                        $"'{category}' renamed to '{newName}' ({booksToUpdate.Count} books updated)\n"
                    );

                    ConsoleHelper.Pause();
                    exitMenu = true;
                });
            }

            menu
                .Back("Back", () => exitMenu = true)
                .CloseAfterSelection()
                .Run();
        }

        public void ShowCategoryStatistics()
        {
            ConsoleHelper.WriteHeader("CATEGORY OVERVIEW");

            using var db = new LibraryDBContext();

            var data = db.Books
                .Where(b => !string.IsNullOrWhiteSpace(b.Category))
                .GroupBy(b => b.Category!)
                .Select(g => new
                {
                    Category = g.Key,
                    BookCount = g.Count(),
                    ActiveLoans = db.Loans.Count(l =>
                        g.Select(b => b.BookId).Contains(l.BookId) &&
                        l.ReturnDate == null)
                })
                .OrderBy(x => x.Category)
                .ToList();

            if (!data.Any())
            {
                ConsoleHelper.Info("No categories found");
                ConsoleHelper.Pause();
                return;
            }

            var headers = new[] { "Category", "Books", "Active loans" };

            var rows = data.Select(x => new[]
            {
                x.Category,
                x.BookCount.ToString(),
                x.ActiveLoans.ToString()
            }).ToArray();

            ConsoleHelper.PrintTable(headers, rows);
            ConsoleHelper.Pause();
        }

        public void ShowNeverBorrowedBooks()
        {
            ConsoleHelper.WriteHeader("NEVER BORROWED BOOKS");

            using var db = new LibraryDBContext();

            var books = db.Books
                .Where(b => !db.Loans.Any(l => l.Book.Title == b.Title))
                .OrderBy(b => b.Title)
                .Select(b => new
                {
                    b.BookId,
                    b.Title,
                    b.Author,
                    b.Category
                })
                .ToList();

            if (!books.Any())
            {
                ConsoleHelper.Info("All books have been borrowed at least once");
                ConsoleHelper.Pause();
                return;
            }

            var headers = new[] { "ID", "Title", "Author", "Category" };

            var rows = books.Select(b => new[]
            {
                b.BookId.ToString(),
                b.Title,
                b.Author,
                b.Category
            }).ToArray();

            ConsoleHelper.PrintTable(headers, rows);
            ConsoleHelper.Pause();
        }

        public void ShowMostBorrowedBooks()
        {
            ConsoleHelper.WriteHeader("MOST BORROWED BOOKS");

            using var db = new LibraryDBContext();

            var loanData = db.Loans
                .Include(l => l.Book)
                .GroupBy(l => l.Book.Title)
                .Select(g => new
                {
                    Title = g.Key,
                    LoanCount = g
                        .Select(l => l.LoanId)
                        .Distinct()
                        .Count()
                })
                .OrderByDescending(x => x.LoanCount)
                .ThenBy(x => x.Title)
                .Take(10)
                .ToList();

            if (!loanData.Any())
            {
                ConsoleHelper.Info("No loan data available");
                ConsoleHelper.Pause();
                return;
            }

            var headers = new[] { "Rank", "Title", "Loans" };

            var rows = loanData.Select((x, index) => new[]
            {
                (index + 1).ToString(),
                x.Title,
                x.LoanCount.ToString()
            }).ToArray();

            ConsoleHelper.PrintTable(headers, rows);
            ConsoleHelper.Pause();
        }
    }
}