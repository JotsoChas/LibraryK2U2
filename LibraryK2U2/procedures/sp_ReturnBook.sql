CREATE PROCEDURE ReturnBook
@LoanId INT
AS
BEGIN
IF NOT EXISTS (SELECT 1 FROM Loan WHERE LoanId = @LoanId AND ReturnDate IS NULL)
BEGIN
PRINT 'Loan not found or already returned.';
RETURN;
END

UPDATE Loan
SET ReturnDate = GETDATE()
WHERE LoanId = @LoanId;

PRINT 'Book returned successfully.';
END;
GO
