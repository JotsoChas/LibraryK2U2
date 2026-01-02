-- View that displays all returned loans in the library system.
-- The view includes loan, book and member information
-- and only shows loans that have been returned.
CREATE VIEW ReturnedLoans AS
SELECT 
l.LoanId,
b.Title AS BookTitle,
m.FirstName + ' ' + m.LastName AS MemberName,
l.LoanDate,
l.DueDate,
l.ReturnDate
FROM Loan l
INNER JOIN Book b ON b.BookId = l.BookId
INNER JOIN Member m ON m.MemberId = l.MemberId
WHERE l.ReturnDate IS NOT NULL;