-- Trigger that runs after an update on the Loan table
-- Used to detect when a book is returned
CREATE TRIGGER trg_OnReturnBook
ON Loan
AFTER UPDATE
AS
BEGIN
 -- Check if the ReturnDate column was updated
IF UPDATE(ReturnDate)
BEGIN
-- Confirmation message when a book is marked as returned
PRINT 'Book updated as returned.';
END
END;
GO