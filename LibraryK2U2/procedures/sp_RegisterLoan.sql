-- Registers a new loan for a book.
-- The procedure checks if the book is already loaned out before creating a new loan.
CREATE PROCEDURE RegisterLoan
    @BookId INT,
    @MemberId INT
AS
BEGIN

-- Check if the selected book already has an active loan
IF EXISTS (SELECT 1 FROM Loan WHERE BookId = @BookId AND ReturnDate IS NULL)
BEGIN
PRINT 'The book is already loaned out.';
RETURN;
END

-- Insert a new loan with loan date set to today and due date set to 14 days ahead
INSERT INTO Loan (BookId, MemberId, LoanDate, DueDate)
VALUES (@BookId, @MemberId, GETDATE(), DATEADD(DAY, 14, GETDATE()));

PRINT 'Loan registered.';
END;