-- Retrieves a list of all loans, including both active and returned books.
-- The query joins Book, Loan and Member to show which member borrowed which book.
SELECT Book.Title, Member.FirstName, Member.LastName, Loan.LoanDate, Loan.ReturnDate
FROM   Book INNER JOIN
Loan ON Book.BookId = Loan.BookId INNER JOIN
Member ON Loan.MemberId = Member.MemberId;