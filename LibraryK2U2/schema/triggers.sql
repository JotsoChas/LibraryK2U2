CREATE TRIGGER trg_OnReturnBook
ON Loan
AFTER UPDATE
AS
BEGIN
IF UPDATE(ReturnDate)
BEGIN
PRINT 'Book updated as returned.';
END
END;
GO

