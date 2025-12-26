using System;
using LibraryK2U2.helpers;
using LibraryK2U2.services;

namespace LibraryK2U2.menus
{
    public class Menu
    {
        private readonly BookService bookService;
        private readonly MemberService memberService;
        private readonly LoanService loanService;

        public Menu()
        {
            bookService = new BookService();
            memberService = new MemberService();
            loanService = new LoanService();
        }

        public void DrawUI()
        {
            bool running = true;

            while (running)
            {
                ConsoleHelper.WriteHeader("Library System");

                Console.WriteLine("1. Register new book");
                Console.WriteLine("2. Register new member");
                Console.WriteLine("3. Register loan");
                Console.WriteLine("4. Register return");
                Console.WriteLine("5. Show active loans");
                Console.WriteLine("6. Search books");
                Console.WriteLine("7. More");
                Console.WriteLine("0. Exit");

                Console.WriteLine();
                var choice = ConsoleHelper.ReadInput("Choose option");

                switch (choice)
                {
                    case "1":
                        bookService.RegisterBook();
                        break;

                    case "2":
                        memberService.RegisterMember();
                        break;

                    case "3":
                        loanService.RegisterLoan();
                        break;

                    case "4":
                        loanService.RegisterReturn();
                        break;

                    case "5":
                        loanService.ShowActiveLoans();
                        break;

                    case "6":
                        bookService.SearchBooks();
                        break;

                    case "7":
                        new AdminMenu().DrawUI();
                        break;

                    case "0":
                        running = false;
                        ConsoleHelper.Success("Goodbye!");
                        break;

                    default:
                        ConsoleHelper.Warning("Invalid choice");
                        ConsoleHelper.Pause();
                        break;
                }
            }
        }
    }
}
