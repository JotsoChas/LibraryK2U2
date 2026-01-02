-- Show all active loans (not yet returned)
SELECT
l.LoanId,
b.Title AS BookTitle,
m.FirstName + ' ' + m.LastName AS Member,
l.LoanDate,
l.DueDate
FROM Loan l
INNER JOIN Book b ON b.BookId = l.BookId
INNER JOIN Member m ON m.MemberId = l.MemberId
WHERE l.ReturnDate IS NULL;

-- Show all returned loans
SELECT
l.LoanId,
b.Title AS BookTitle,
m.FirstName + ' ' + m.LastName AS Member,
l.LoanDate,
l.DueDate,
l.ReturnDate
FROM Loan l
INNER JOIN Book b ON b.BookId = l.BookId
INNER JOIN Member m ON m.MemberId = l.MemberId
WHERE l.ReturnDate IS NOT NULL;

-- Mark a loan as returned
UPDATE Loan
SET ReturnDate = GETDATE()
WHERE LoanId = 3;

-- Show all overdue loans (returned late)
SELECT
b.Title AS BookTitle,
m.FirstName + ' ' + m.LastName AS Member,
l.DueDate,
l.ReturnDate,
DATEDIFF(DAY, l.DueDate, l.ReturnDate) AS DaysOverdue
FROM Loan l
INNER JOIN Book b ON b.BookId = l.BookId
INNER JOIN Member m ON m.MemberId = l.MemberId
WHERE l.ReturnDate IS NOT NULL
AND l.ReturnDate > l.DueDate;

-- Show which books Johanna has borrowed
SELECT DISTINCT
b.Title
FROM Loan l
INNER JOIN Member m ON m.MemberId = l.MemberId
INNER JOIN Book b ON b.BookId = l.BookId
WHERE m.FirstName = 'Johanna';

-- Show all book information
SELECT
BookId,
Title,
Author,
ISBN,
Category
FROM Book;

-- Show all members
SELECT
MemberId,
FirstName,
LastName
FROM Member;

-- Show number of loans per member
SELECT
m.FirstName + ' ' + m.LastName AS Member,
COUNT(*) AS LoanCount
FROM Loan l
INNER JOIN Member m ON m.MemberId = l.MemberId
GROUP BY m.FirstName, m.LastName
ORDER BY LoanCount DESC;

-- Show all loans with book and member information
SELECT
l.LoanId,
b.Title AS BookTitle,
m.FirstName + ' ' + m.LastName AS Member,
l.LoanDate,
l.DueDate,
l.ReturnDate
FROM Loan l
INNER JOIN Book b ON b.BookId = l.BookId
INNER JOIN Member m ON m.MemberId = l.MemberId
ORDER BY l.LoanDate DESC;

-- Show Johanna's overdue books
SELECT
b.Title AS BookTitle,
l.DueDate,
l.ReturnDate,
DATEDIFF(DAY, l.DueDate, l.ReturnDate) AS DaysOverdue
FROM Loan l
INNER JOIN Member m ON m.MemberId = l.MemberId
INNER JOIN Book b ON b.BookId = l.BookId
WHERE m.FirstName = 'Johanna'
AND l.ReturnDate IS NOT NULL
AND l.ReturnDate > l.DueDate;

-- Show basic information about all books, grouped by category
SELECT
Title,
Author,
ISBN,
Category
FROM Book
ORDER BY Category;

-- Retrieves a list of all loans, including both active and returned books.
-- The query joins Book, Loan and Member to show which member borrowed which book.
SELECT Book.Title, Member.FirstName, Member.LastName, Loan.LoanDate, Loan.ReturnDate
FROM   Book INNER JOIN
Loan ON Book.BookId = Loan.BookId INNER JOIN
Member ON Loan.MemberId = Member.MemberId;

-- Retrieves basic information about all books in the library.
-- The result is ordered by category to group similar books together.
SELECT Title, Author, ISBN, Category
FROM   Book
ORDER BY Category;

-- Index strategy 1:
-- Nonclustered index on ReturnDate to improve performance for queries
-- that filter or sort loans based on return status.
CREATE NONCLUSTERED INDEX IX_Loan_ReturnDate
ON Loan (ReturnDate);

-- Index strategy 2:
-- Filtered nonclustered index that only includes active loans.
-- This index is optimized for queries that frequently search for
-- loans where ReturnDate IS NULL.
CREATE NONCLUSTERED INDEX IX_Loan_ReturnDate_Null
ON Loan (ReturnDate)
WHERE ReturnDate IS NULL;

-- Adds a column that marks whether a member is blocked or not.
-- Default value 0 means the member is not blocked.
ALTER TABLE Member
ADD IsBlocked BIT NOT NULL
DEFAULT 0;

-- Shows all members and their blocked status.
SELECT MemberId, FirstName, LastName, IsBlocked
FROM Member;