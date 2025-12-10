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
