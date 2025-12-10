SELECT Book.Title, Member.FirstName, Member.LastName, Loan.LoanDate, Loan.ReturnDate
FROM   Book INNER JOIN
Loan ON Book.BookId = Loan.BookId INNER JOIN
Member ON Loan.MemberId = Member.MemberId;