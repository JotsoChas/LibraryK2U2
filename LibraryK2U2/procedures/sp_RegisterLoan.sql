CREATE PROCEDURE RegisterLoan
    @BookId INT,
    @MemberId INT
AS
BEGIN
IF EXISTS (SELECT 1 FROM Loan WHERE BookId = @BookId AND ReturnDate IS NULL)
BEGIN
PRINT 'The book is already loaned out.';
RETURN;
END

INSERT INTO Loan (BookId, MemberId, LoanDate, DueDate)
VALUES (@BookId, @MemberId, GETDATE(), DATEADD(DAY, 14, GETDATE()));

PRINT 'Loan registered.';
END;
