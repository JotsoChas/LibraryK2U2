-- View that displays all active loans in the library system.
-- The view combines loan, book and member information
-- and only includes loans that have not yet been returned.
CREATE VIEW ActiveLoans AS
SELECT 
l.LoanId,
b.Title AS BookTitle,
m.FirstName + ' ' + m.LastName AS MemberName,
l.LoanDate,
l.DueDate
FROM Loan l
INNER JOIN Book b ON b.BookId = l.BookId
INNER JOIN Member m ON m.MemberId = l.MemberId
WHERE l.ReturnDate IS NULL;