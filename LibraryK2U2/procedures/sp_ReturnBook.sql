-- Registers the return of a borrowed book.
-- The procedure verifies that the loan exists and has not already been returned.
CREATE PROCEDURE ReturnBook
@LoanId INT
AS
BEGIN

-- Check that the loan exists and is still active (not yet returned)
IF NOT EXISTS (SELECT 1 FROM Loan WHERE LoanId = @LoanId AND ReturnDate IS NULL)
BEGIN
PRINT 'Loan not found or already returned.';
RETURN;
END

-- Update the loan by setting the return date to the current date
UPDATE Loan
SET ReturnDate = GETDATE()
WHERE LoanId = @LoanId;

-- Confirmation message after successful return
PRINT 'Book returned successfully.';
END;
GO